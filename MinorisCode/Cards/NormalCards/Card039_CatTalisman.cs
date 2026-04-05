
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD039_CAT_TALISMAN
中文名称: 猫猫护身符
英文名称: Cat Talisman
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 0
卡牌效果: 回复 1 点生命。获得 1 点能量。抽 1 张牌。
卡牌描述(ZHS): 回复 1 点生命。获得 1 点能量。抽 1 张牌。
卡牌描述(ENG): Heal 1 HP. Gain 1 Energy. Draw 1 card.
升级效果: 获得固有
*/
public class Card039_CatTalisman() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
         HoverTipFactory.ForEnergy(this)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, 1m);
        Owner.PlayerCombatState!.GainEnergy(1);
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}








