
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD030_PANICKED_DODGE
中文名称: 仓皇躲闪
英文名称: Panicked Dodge
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 0
卡牌效果: 获得 {Block:diff()} 点格挡。变化你手中的一张随机牌。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。变化你手中的一张随机牌。
卡牌描述(ENG): Gain {Block:diff()} Block. Transform a random card in your hand.
升级效果: 格挡+2
*/
public class Card030_PanickedDodge() : MinorisCard(0, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var candidates = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        if (candidates.Count == 0) return;
        var idx = Owner.RunState.Rng.CombatCardGeneration.NextInt(candidates.Count);
        var options = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token);
        await CardCmd.Transform(new CardTransformation(candidates[idx], options).Yield(), Owner.RunState.Rng.CombatCardSelection);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}









