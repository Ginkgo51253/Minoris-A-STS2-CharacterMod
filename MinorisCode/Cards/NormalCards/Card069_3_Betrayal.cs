namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD069_3_BETRAYAL
中文名称: 背叛
英文名称: Betrayal
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 2
卡牌效果: 在你的回合开始时，从其他职业的卡牌中选择获得一张{IfUpgraded:show:[gold]升级过[/gold]的|}牌，其费用在打出前为0费
卡牌描述(ZHS): 在你的回合开始时，从其他职业的卡牌中选择获得一张{IfUpgraded:show:[gold]升级过[/gold]的|}牌，其费用在打出前为0费
卡牌描述(ENG): At the start of your turn, choose a{IfUpgraded:show: [gold]Upgraded[/gold]|} card from other classes. It costs 0 until played
升级效果: BetrayalPower+1
*/
[Pool(typeof(TokenCardPool))]
public class Card069_3_Betrayal() : MinorisCard(2, CardType.Power, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.BetrayalPower>(1)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<Powers.BetrayalPowerPlus>(Owner.Creature, DynamicVars["BetrayalPower"].IntValue, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<Powers.BetrayalPower>(Owner.Creature, DynamicVars["BetrayalPower"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BetrayalPower"].UpgradeValueBy(1m);
    }
}

