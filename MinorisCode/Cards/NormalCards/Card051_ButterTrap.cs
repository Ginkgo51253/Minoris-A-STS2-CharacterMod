
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD051_BUTTER_TRAP
中文名称: 黄油陷阱
英文名称: Butter Trap
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 2
卡牌效果: 获得12点格挡。本回合中，你每受到一次攻击，都给予攻击者1层虚弱与1层易伤
卡牌描述(ZHS): 获得12点格挡。本回合中，你每受到一次攻击，都给予攻击者1层虚弱与1层易伤
卡牌描述(ENG): Gain 12 Block. This turn, whenever you are attacked, apply 1 Weak and 1 Vulnerable to the attacker
升级效果: 格挡+4
*/
public class Card051_ButterTrap() : MinorisCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<Powers.ButterTrapRetaliationPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}









