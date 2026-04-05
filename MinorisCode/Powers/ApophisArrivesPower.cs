﻿﻿﻿
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 阿波菲斯降临
能力英文名称: Apophis Arrives
能力描述(ZHS): 在你的回合结束时，对所有敌人造成[blue]{Amount}[/blue]点伤害。每3个回合，在你的回合开始时，对你造成[blue]{Amount}[/blue]点伤害。
能力描述(ENG): At the end of your turn, deal [blue]{Amount}[/blue] damage to ALL enemies. Every 3 turns, at the start of your turn, deal [blue]{Amount}[/blue] damage to yourself.
相关卡牌（本地键）: MINORIS-CARD073_APOPHIS_ARRIVES
*/
public class ApophisArrivesPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;
    private int _turnsUntilSelfDamage = 3;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-APOPHIS_ARRIVES").ToString() ?? string.Empty)
            .Replace("{Amount}", Amount.ToString())
            .Replace("{Turns}", _turnsUntilSelfDamage.ToString()), isSmart: false)
    ];

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side || CombatState == null) return;
        var enemies = CombatState.GetOpponentsOf(Owner).Where(e => e.IsAlive);
        await CreatureCmd.Damage(choiceContext, enemies, Amount, ValueProp.Unpowered, Owner);
        await CheckWinConditionIfCombatEnding();
        if (!CombatManager.Instance.IsInProgress) return;
        Flash();
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        _turnsUntilSelfDamage = System.Math.Max(0, _turnsUntilSelfDamage - 1);
        if (_turnsUntilSelfDamage == 0)
        {
            await CreatureCmd.Damage(choiceContext, new[] { Owner }, Amount, ValueProp.Unpowered, Owner);
            Flash();
            _turnsUntilSelfDamage = 3;
        }
    }
}














