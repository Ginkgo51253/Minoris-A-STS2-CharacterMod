
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD070_ISIS_REBIRTH
中文名称: 伊西斯的重生
英文名称: Isis Rebirth
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 2
卡牌效果: 获得 {RegenPower:diff()} 层再生。
卡牌描述(ZHS): 获得 {RegenPower:diff()} 层再生。
卡牌描述(ENG): Gain {RegenPower:diff()} Regen.
升级效果: 获得层再生+2
*/
public class Card070_IsisRebirth() : MinorisCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RegenPower>(5)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RegenPower>(Owner.Creature, DynamicVars["RegenPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RegenPower"].UpgradeValueBy(2m);
    }
}









