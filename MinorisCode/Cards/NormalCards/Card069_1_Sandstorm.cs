namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD069_1_SANDSTORM
中文名称: 沙暴
英文名称: Sandstorm
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 1
卡牌效果: 你本回合只会受到50%的伤害
卡牌描述(ZHS): 你本回合只会受到50%的伤害
卡牌描述(ENG): This turn, you only take 50% damage
升级效果: 费用-1
*/
[Pool(typeof(TokenCardPool))]
public class Card069_1_Sandstorm() : MinorisCard(1, CardType.Skill, CardRarity.Token, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.SandstormPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

