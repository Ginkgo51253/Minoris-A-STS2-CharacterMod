
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD032_TRAINING
中文名称: 训练
英文名称: Training
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 获得 {Block:diff()} 点格挡。升级你手牌中的 {UpgradeCount:diff()} 张牌。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。升级你手牌中的 {UpgradeCount:diff()} 张牌。
卡牌描述(ENG): Gain {Block:diff()} Block. Upgrade {UpgradeCount:diff()} card(s) in your hand.
升级效果: 升级手牌数量+1
*/
public class Card032_Training() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string UpgradeCountKey = "UpgradeCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new IntVar(UpgradeCountKey, 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var count = DynamicVars[UpgradeCountKey].IntValue;
        if (count <= 0) return;

        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList();
        if (hand.Count == 0) return;

        var prefs = new CardSelectorPrefs(new LocString("gameplay_ui", "CHOOSE_CARD_UPGRADE_HEADER"), count);
        var picks = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, c => c.IsUpgradable, this);
        foreach (var c in picks)
        {
            CardCmd.Upgrade(c);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[UpgradeCountKey].UpgradeValueBy(1m);
    }
}









