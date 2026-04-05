
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD052_MOON_STEP
中文名称: 月之步伐
英文名称: Moon Step
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 0
卡牌效果: 获得 2 点能量。抽 2 张牌。本回合中，这张牌每打出一次耗能增加 1 点。
卡牌描述(ZHS): 获得 2 点能量。抽 2 张牌。本回合中，这张牌每打出一次耗能增加 1 点。
卡牌描述(ENG): Gain 2 Energy. Draw 2 cards. This turn, this costs 1 more each time you play it.
升级效果: 移除虚无
*/
public class Card052_MoonStep() : MinorisCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    public override bool ShouldReceiveCombatHooks => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
         HoverTipFactory.ForEnergy(this)];

    private int _playsThisTurn;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Owner.PlayerCombatState!.GainEnergy(2);
        await CardPileCmd.Draw(choiceContext, 2, Owner);
        _playsThisTurn++;
        EnergyCost.SetThisCombat(_playsThisTurn);
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        _playsThisTurn = 0;
        EnergyCost.SetThisCombat(0);
        await Task.CompletedTask;
    }
}








