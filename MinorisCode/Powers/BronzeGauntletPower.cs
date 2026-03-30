
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 铜护手
能力英文名称: Bronze Gauntlet
能力描述(ZHS): 每当你的攻击造成未被格挡的伤害时，获得[blue]{Amount}[/blue]点格挡。
能力描述(ENG): Whenever your Attacks deal unblocked damage, gain [blue]{Amount}[/blue] Block.
相关卡牌（本地键）: MINORIS-CARD061_BRONZE_GAUNTLET
*/
public class BronzeGauntletPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner) return;
        if (cardSource == null) return;
        if (cardSource.Type != CardType.Attack) return;
        if (result.UnblockedDamage <= 0) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}














