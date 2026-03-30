
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD073_APOPHIS_ARRIVES
中文名称: 阿波菲斯降临
英文名称: Apophis Arrives
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 在你的回合结束时，对所有敌人造成 {ApophisArrivesPower:diff()} 点伤害。每 3 个回合，在你的回合开始时，对你造成 {ApophisArrivesPower:diff()} 点伤害。
卡牌描述(ZHS): 在你的回合结束时，对所有敌人造成 {ApophisArrivesPower:diff()} 点伤害。每 3 个回合，在你的回合开始时，对你造成 {ApophisArrivesPower:diff()} 点伤害。
卡牌描述(ENG): At the end of your turn, deal {ApophisArrivesPower:diff()} damage to ALL enemies. Every 3 turns, at the start of your turn, deal {ApophisArrivesPower:diff()} damage to yourself.
升级效果: 在你的回合结束时，对所有敌人造成伤害+10
*/
public class Card073_ApophisArrives() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.ApophisArrivesPower>(20)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.ApophisArrivesPower>(Owner.Creature, DynamicVars["ApophisArrivesPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ApophisArrivesPower"].UpgradeValueBy(10m);
    }
}









