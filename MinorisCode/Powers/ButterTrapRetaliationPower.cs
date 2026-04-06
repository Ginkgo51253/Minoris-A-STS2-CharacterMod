
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 黄油陷阱
能力英文名称: Butter Trap
能力描述(ZHS): 本回合中，当你受到攻击时，给予攻击者1层虚弱与1层易伤。你的下回合开始时移除。
能力描述(ENG): This turn, whenever you are attacked, apply 1 Weak and 1 Vulnerable to the attacker. Remove at the start of your next turn.
相关卡牌（本地键）: MINORIS-CARD051_BUTTER_TRAP
*/
public class ButterTrapRetaliationPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (dealer == null) return;
        if (CombatState != null && CombatState.CurrentSide != Owner.Side)
        {
            await PowerCmd.Apply<WeakPower>(dealer, 2 * Amount, Owner, null);
            await PowerCmd.Apply<VulnerablePower>(dealer, 2 * Amount, Owner, null);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}














