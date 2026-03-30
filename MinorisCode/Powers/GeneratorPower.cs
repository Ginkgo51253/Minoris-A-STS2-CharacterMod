
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 发电机
能力英文名称: Generator
能力描述(ZHS): 在你的回合中，每当你打出一张攻击牌时，你的下回合额外获得[blue]{Amount}[/blue]点能量。
能力描述(ENG): During your turn, whenever you play an Attack, gain [blue]{Amount}[/blue] additional Energy next turn.
相关卡牌（本地键）: MINORIS-CARD065_GENERATOR
*/
public class GeneratorPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private int _currentTurnEnergyToAdd;
    private int _pendingNextTurnEnergy;

    public override int DisplayAmount => _pendingNextTurnEnergy + _currentTurnEnergyToAdd;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (CombatState.CurrentSide != Owner.Side) return;
        if (playedCard.Type == CardType.Attack)
        {
            var energyPerAttack = (int)Amount;
            if (energyPerAttack > 0)
            {
                _currentTurnEnergyToAdd += energyPerAttack;
                InvokeDisplayAmountChanged();
            }
        }
        await Task.CompletedTask;
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        _pendingNextTurnEnergy += _currentTurnEnergyToAdd;
        _currentTurnEnergyToAdd = 0;
        InvokeDisplayAmountChanged();
        await Task.CompletedTask;
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player.PlayerCombatState == null) return;
        if (_pendingNextTurnEnergy > 0)
        {
            Owner.Player.PlayerCombatState.GainEnergy(_pendingNextTurnEnergy);
            _pendingNextTurnEnergy = 0;
            Flash();
            InvokeDisplayAmountChanged();
        }
        await Task.CompletedTask;
    }
}














