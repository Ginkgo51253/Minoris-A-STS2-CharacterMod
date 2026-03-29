
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 过敏原
能力英文名称: Allergen
能力描述(ZHS): 本回合中，当你打出攻击牌时，抽[blue]{Amount}[/blue]张牌并失去[blue]{Amount}[/blue]×2点生命。回合结束时移除。
能力描述(ENG): This turn, whenever you play an Attack, draw [blue]{Amount}[/blue] card(s) and lose [blue]{Amount}[/blue]x2 HP. Remove at end of turn.
相关卡牌（本地键）: MINORIS-CARD042_ALLERGEN
*/
public class AllergenPower : MinorisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        if (Amount > 0)
        {
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
            await CreatureCmd.Damage(choiceContext, Owner, 2m * Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}














