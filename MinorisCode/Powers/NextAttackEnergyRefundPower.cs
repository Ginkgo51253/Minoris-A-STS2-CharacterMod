
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 攻击返能
能力英文名称: Attack Energy Refund
能力描述(ZHS): 本回合中，你下一张攻击牌打出后获得1点能量。然后移除。
能力描述(ENG): This turn, the next Attack you play gives you 1 Energy. Then remove this.
相关卡牌（本地键）: 
*/
public class NextAttackEnergyRefundPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        Owner.Player?.PlayerCombatState?.GainEnergy(1);
        RemoveInternal();
        await Task.CompletedTask;
    }
}














