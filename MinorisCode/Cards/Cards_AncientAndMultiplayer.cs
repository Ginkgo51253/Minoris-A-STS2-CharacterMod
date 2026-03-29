using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Minoris.MinorisCode.Cards;

public class Card074_WildScratch() : MinorisCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new IntVar(HitsKey, 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var hits = DynamicVars[HitsKey].IntValue;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitCount(hits)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(2m);
    }
}

public class Card075_WildForm() : MinorisCard(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Minoris.MinorisCode.Powers.WildFormPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}

public class Card076_CommandTheHunt() : MinorisCard(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Minoris.MinorisCode.Powers.CommandTheHuntPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Minoris.MinorisCode.Powers.CommandTheHuntPower>(Owner.Creature, DynamicVars["CommandTheHuntPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CommandTheHuntPower"].UpgradeValueBy(1m);
    }
}

public class Card077_HelloPartner() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new HealVar(3m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var draw = (uint)DynamicVars.Cards.IntValue;
        var heal = DynamicVars.Heal.BaseValue;
        var players = CombatState.Players.Distinct().ToList();
        foreach (var p in players)
        {
            await PlayerCmd.GainEnergy(3, p);
            await CardPileCmd.Draw(choiceContext, draw, p);
            await CreatureCmd.Heal(p.Creature, heal);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
        DynamicVars.Heal.UpgradeValueBy(2m);
    }
}
