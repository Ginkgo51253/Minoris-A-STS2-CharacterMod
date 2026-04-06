﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 应激反应
能力英文名称: Stress Response
能力描述(ZHS): 本回合中，每当你打出一张牌时，失去生命：{HpLoss}\n获得格挡：{Block}。
能力描述(ENG): This turn, whenever you play a card, lose 1 HP and gain [blue]{Amount}[/blue] Block. Remove at end of turn.
相关卡牌（本地键）: MINORIS-CARD049_STRESS_RESPONSE
*/
public class StressResponsePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
    private int _stackCount;
    public int StackCount => _stackCount;
    public void IncrementStack()
    {
        _stackCount++;
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-STRESS_RESPONSE").ToString() ?? string.Empty)
            .Replace("{HpLoss}", _stackCount.ToString())
            .Replace("{Block}", Amount.ToString()), isSmart: false)
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (_stackCount > 0)
        {
            await CreatureCmd.Damage(choiceContext, Owner, _stackCount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        }
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}











