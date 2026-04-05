
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 小猫刀
能力英文名称: Kitten Knife
能力描述(ZHS): 每当你的攻击造成未被格挡的伤害时，对目标额外造成[blue]{Amount}[/blue]点伤害。
能力描述(ENG): Whenever your Attacks deal unblocked damage, deal [blue]{Amount}[/blue] additional damage to the target.
相关卡牌（本地键）: MINORIS-CARD060_KITTEN_KNIFE
*/
public class KittenKnifePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-KITTEN_KNIFE").ToString() ?? string.Empty), isSmart: false)
    ];

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner) return;
        if (cardSource == null) return;
        if (cardSource.Type != CardType.Attack) return;
        if (result.UnblockedDamage <= 0) return;
        await CreatureCmd.Damage(choiceContext, new[] { target }, Amount, ValueProp.Unpowered, Owner);
        await CheckWinConditionIfCombatEnding();
    }
}














