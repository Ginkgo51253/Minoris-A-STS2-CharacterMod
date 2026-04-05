
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD062_TOY_CLAW
中文名称: 玩具猫爪
英文名称: Toy Claw
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 在你每回合开始时，将 {ToyClawPower:diff()} 张“抓挠”置入你的手牌。
卡牌描述(ZHS): 在你每回合开始时，将 {ToyClawPower:diff()} 张“抓挠”置入你的手牌。
卡牌描述(ENG): At the start of your turn, add {ToyClawPower:diff()} "Scratch" to your hand.
升级效果: 在你每回合开始时，将 1 张“抓挠+”置入你的手牌。
*/
public class Card062_ToyClaw() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.ToyClawPower>(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Card062_1_Scratch>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<Powers.ToyClawPowerPlus>(Owner.Creature, DynamicVars["ToyClawPower"].IntValue, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<Powers.ToyClawPower>(Owner.Creature, DynamicVars["ToyClawPower"].IntValue, Owner.Creature, this);
        }
    }
}








