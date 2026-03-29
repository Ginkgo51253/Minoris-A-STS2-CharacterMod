
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD077_HELLO_PARTNER
中文名称: 你好，伙伴！
英文名称: Hello, Partner!
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 所有的盟友获得 3 点能量，抽 {Cards:diff()} 张牌，回复 {Heal:diff()} 点生命。
卡牌描述(ZHS): 所有的盟友获得 3 点能量，抽 {Cards:diff()} 张牌，回复 {Heal:diff()} 点生命。
卡牌描述(ENG): All allies gain 3 Energy, draw {Cards:diff()} cards, and heal {Heal:diff()} HP.
升级效果: Cards+2；Heal+2
*/
public class Card077_HelloPartner() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new HealVar(3m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var draw = (uint)DynamicVars.Cards.IntValue;
        var heal = DynamicVars.Heal.BaseValue;
        var players = CombatState.Players.Distinct().ToList();
        foreach (var p in players)
        {
            await PlayerCmd.GainEnergy(3, p);
            await CardPileCmd.Draw(choiceContext, draw, p);
            await CreatureCmd.Heal(p.Creature, heal);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
        DynamicVars.Heal.UpgradeValueBy(2m);
    }
}









