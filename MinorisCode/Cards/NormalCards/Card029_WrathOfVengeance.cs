﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD029_WRATH_OF_VENGEANCE
中文名称: 复仇之怒
英文名称: Wrath of Vengeance
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 随机造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。本场战斗中你每受过一次伤，本牌就会重复打出一次。
卡牌描述(ZHS): 随机造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。本场战斗中你每受过一次伤，本牌就会重复打出一次。
卡牌描述(ENG): Deal {Damage:diff()} damage to a random enemy {Hits:diff()} times. Repeat once for each time you were hurt this combat.
升级效果: 伤害+8
*/
public class Card029_WrathOfVengeance() : MinorisCard(3, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    private const string HitsKey = "Hits";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new DamageVar(14m, ValueProp.Move),
         new CalculationBaseVar(1m),
         new CalculationExtraVar(1m),
         new CalculatedVar(HitsKey).WithMultiplier(CalcHitsMultiplier)];
    private int _totalTriggersThisCombat;

    private int GetDamageTakenThisCombat()
    {
        if (Owner?.Creature == null) return 0;
        return CombatManager.Instance.History.Entries
            .OfType<MegaCrit.Sts2.Core.Combat.History.Entries.DamageReceivedEntry>()
            .Count(e => e.Receiver == Owner.Creature && e.Result.UnblockedDamage > 0);
    }

    private static decimal CalcHitsMultiplier(CardModel card, Creature? target)
    {
        if (card.Owner?.Creature == null) return 0m;
        if (!CombatManager.Instance.IsInProgress) return 0m;
        return CombatManager.Instance.History.Entries
            .OfType<MegaCrit.Sts2.Core.Combat.History.Entries.DamageReceivedEntry>()
            .Count(e => e.Receiver == card.Owner.Creature && e.Result.UnblockedDamage > 0);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        async Task HitOnce()
        {
            var enemies = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) return;
            var target = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
            _totalTriggersThisCombat++;
        }
        await HitOnce();
        if (CombatManager.Instance.IsEnding) return;
        var repeats = Math.Max(0, GetDamageTakenThisCombat());
        for (var i = 0; i < repeats; i++)
        {
            if (CombatManager.Instance.IsEnding) return;
            await HitOnce();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
    }
}








