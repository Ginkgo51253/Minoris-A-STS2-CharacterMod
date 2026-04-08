
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 受伤计数
能力英文名称: Damage Taken Counter
能力描述(ZHS): 每当你受到未被格挡的伤害时，层数+1。
能力描述(ENG): Whenever you take unblocked damage, increase this by 1.
相关卡牌（本地键）: 
*/
public class DamageTakenCounterPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => false;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (result.UnblockedDamage <= 0) return;
        SetAmount(Amount + 1);
        await Task.CompletedTask;
    }
}














