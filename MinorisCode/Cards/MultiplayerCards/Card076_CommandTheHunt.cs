
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD076_COMMAND_THE_HUNT
中文名称: 号令狂猎
英文名称: Command the Hunt
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 2
卡牌效果: 每当另一名玩家打出一张攻击牌时，额外打出 {CommandTheHuntPower:diff()} 次。
卡牌描述(ZHS): 每当另一名玩家打出一张攻击牌时，额外打出 {CommandTheHuntPower:diff()} 次。
卡牌描述(ENG): Whenever another player plays an Attack, play it {CommandTheHuntPower:diff()} additional times.
升级效果: 每当另一名玩家打出一张攻击牌时，额外打出次+1
*/
public class Card076_CommandTheHunt() : MinorisCard(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Minoris.MinorisCode.Powers.CommandTheHuntPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Minoris.MinorisCode.Powers.CommandTheHuntPower>(Owner.Creature, DynamicVars["CommandTheHuntPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CommandTheHuntPower"].UpgradeValueBy(1m);
    }
}









