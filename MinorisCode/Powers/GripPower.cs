
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 牵制
能力英文名称: Grip
能力描述(ZHS): 当任意友方角色打出攻击牌时，该目标失去[blue]{Amount}[/blue]点生命。
能力描述(ENG): When a player plays an Attack, this creature loses HP equal to [blue]{Amount}[/blue].
相关卡牌（本地键）: MINORIS-CARD019_SOBEK_STRIKE, MINORIS-CARD035_BIND, MINORIS-CARD054_DEATH_ROLL
*/
public class GripPower : MinorisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Type != CardType.Attack) return;
        var dealer = playedCard.Owner.Creature;
        if (dealer.Side != CombatSide.Player) return;
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, dealer);
        await CheckWinConditionIfCombatEnding();
        if (!CombatManager.Instance.IsInProgress) return;
        Flash();
    }
}














