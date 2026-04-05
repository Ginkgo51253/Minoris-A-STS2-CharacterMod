
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD061_BRONZE_GAUNTLET
中文名称: 铜护手
英文名称: Bronze Gauntlet
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 2
卡牌效果: 在你每次攻击造成伤害后，获得1点格挡
卡牌描述(ZHS): 在你每次攻击造成伤害后，获得1点格挡
卡牌描述(ENG): Whenever your Attacks deal damage, gain 1 Block
升级效果: 获得固有
*/
public class Card061_BronzeGauntlet() : MinorisCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.BronzeGauntletPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.BronzeGauntletPower>(Owner.Creature, DynamicVars["BronzeGauntletPower"].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}









