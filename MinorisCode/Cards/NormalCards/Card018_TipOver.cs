
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD018_TIP_OVER
中文名称: 打翻
英文名称: Tip Over
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 造成 {Damage:diff()} 点伤害。抽 {Cards:diff()} 张牌。自动打出其中的攻击牌。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。抽 {Cards:diff()} 张牌。自动打出其中的攻击牌。
卡牌描述(ENG): Deal {Damage:diff()} damage. Draw {Cards:diff()} cards. Auto-play any Attacks drawn.
升级效果: Cards+2
*/
public class Card018_TipOver() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Move),
        new CardsVar(2)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (CombatState == null) return;
        var hand = PileType.Hand.GetPile(Owner);
        var before = hand.Cards.ToHashSet();
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        var after = hand.Cards.ToHashSet();
        var newly = after.Where(c => !before.Contains(c)).ToList();
        foreach (var c in newly.Where(c => c.Type == CardType.Attack && c != this))
        {
            if (CombatState == null) break;
            if (CombatManager.Instance.IsEnding) break;
            var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) break;
            var idx = (int)(GD.Randi() % (uint)enemies.Count);
            await CardCmd.AutoPlay(choiceContext, c, enemies[idx]);
            if (CombatManager.Instance.IsEnding) break;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}









