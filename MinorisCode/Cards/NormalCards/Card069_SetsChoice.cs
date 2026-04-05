﻿﻿
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD069_SETS_CHOICE
中文名称: 塞特的抉择
英文名称: Set's Choice
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 1
卡牌效果: 从沙暴、雷闪、背叛中选择获得一张
卡牌描述(ZHS): 从沙暴、雷闪、背叛中选择获得一张
卡牌描述(ENG): Choose 1 of: Sandstorm, Thunder Flash, Betrayal. Add it to your hand
升级效果: 费用-1；获得固有
*/
public class Card069_SetsChoice() : MinorisCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<Card069_1_Sandstorm>(),
         HoverTipFactory.FromCard<Card069_2_ThunderFlash>(),
         HoverTipFactory.FromCard<Card069_3_Betrayal>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var options = new List<CardModel>
        {
            CombatState.CreateCard<Card069_1_Sandstorm>(Owner),
            CombatState.CreateCard<Card069_2_ThunderFlash>(Owner),
            CombatState.CreateCard<Card069_3_Betrayal>(Owner)
        };
        if (CombatManager.Instance.IsEnding) return;
        var pick = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
        if (pick != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(pick, PileType.Hand, true, CardPilePosition.Top));
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Innate);
    }
}









