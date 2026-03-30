
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 眼之护符
能力英文名称: Eye Amulet
能力描述(ZHS): 在你的格挡被击破前，你的格挡不会在回合结束时消失。格挡被击破后移除。
能力描述(ENG): Your Block does not disappear at end of turn until it is broken. Remove when your Block is broken.
相关卡牌（本地键）: MINORIS-CARD057_EYE_CHARM
*/
public class BlockPreservePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => true;

    public override bool ShouldClearBlock(Creature creature)
    {
        if (Owner != creature) return true;
        return false;
    }

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (preventer != this) return;
        if (creature != Owner) return;
        Flash();
        await Task.CompletedTask;
    }

    public override async Task AfterBlockBroken(Creature creature)
    {
        if (creature != Owner) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}














