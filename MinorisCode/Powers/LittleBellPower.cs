
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 小铃铛
能力英文名称: Little Bell
能力描述(ZHS): 在你的回合中，每当你失去生命时，获得[blue]{Amount}[/blue]点力量。
能力描述(ENG): During your turn, whenever you lose HP, gain [blue]{Amount}[/blue] Strength.
相关卡牌（本地键）: MINORIS-CARD064_LITTLE_BELL
*/
public class LittleBellPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (result.UnblockedDamage <= 0) return;
        if (CombatState.CurrentSide != Owner.Side) return;
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}














