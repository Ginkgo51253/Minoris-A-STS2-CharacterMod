
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD069_2_THUNDER_FLASH
中文名称: 雷闪
英文名称: Thunder Flash
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 随机造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。
卡牌描述(ZHS): 随机造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。
卡牌描述(ENG): Deal {Damage:diff()} damage {Hits:diff()} times to random enemies.
升级效果: 伤害+2；随机造成{Damage:diff()}点伤害次+2
*/
[Pool(typeof(TokenCardPool))]
public class Card069_2_ThunderFlash() : MinorisCard(3, CardType.Attack, CardRarity.Token, TargetType.RandomEnemy)
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new IntVar(HitsKey, 8)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = DynamicVars[HitsKey].IntValue;
        var dmg = DynamicVars.Damage.BaseValue;
        for (var i = 0; i < hits; i++)
        {
            if (CombatState == null) return;
            var enemies = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) return;
            var idx = (int)(GD.Randi() % (uint)enemies.Count);
            var target = enemies[idx];
            await DamageCmd.Attack(dmg).FromCard(this).Targeting(target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[HitsKey].UpgradeValueBy(2m);
    }
}









