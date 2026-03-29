using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Minoris.MinorisCode.Cards;
using Minoris.MinorisCode.Character;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Minoris.MinorisCode.Extensions;

namespace Minoris.MinorisCode.Relics;

public class FreezeDried : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner?.Creature == null) return;
        if (player != Owner) return;
        await PowerCmd.Apply<VigorPower>(Owner.Creature, 4, Owner.Creature, null);
        Flash();
    }
}

[Pool(typeof(DeprecatedRelicPool))]
public class SmallBowTie : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (Owner?.Creature == null) return;
        if (side != Owner.Creature.Side) return;
        if (combatState.RoundNumber > 1) return;
        var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
        if (hand.Count == 0) return;
        var pick = hand[GD.RandRange(0, hand.Count - 1)];
        CardCmd.Upgrade(pick);
        Flash();
        await Task.CompletedTask;
    }
}

[Pool(typeof(DeprecatedRelicPool))]
public class TidySmallBowTie : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (Owner?.Creature == null) return;
        if (side != Owner.Creature.Side) return;
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList();
        if (hand.Count == 0) return;
        var pick = hand[(int)(GD.Randi() % (uint)hand.Count)];
        var deck = PileType.Deck.GetPile(Owner);
        var deckCard = deck?.Cards.FirstOrDefault(c => c.Id == pick.Id);
        if (deckCard != null && !ReferenceEquals(deckCard, pick) && deckCard.IsUpgradable)
        {
            CardCmd.Upgrade(deckCard, CardPreviewStyle.None);
        }
        CardCmd.Upgrade(pick, CardPreviewStyle.None);
        Flash();
        await Task.CompletedTask;
    }
}

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic.Id == ModelDb.Relic<SmallBowTie>().Id) __result = ModelDb.Relic<TidySmallBowTie>().ToMutable();
    }
}

public class BandageRoll : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _isOwnersTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        _isOwnersTurn = player == Owner;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Enemy)
        {
            _isOwnersTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner?.Creature == null) return;
        if (target != Owner.Creature) return;
        if (!_isOwnersTurn) return;
        if (result.UnblockedDamage <= 0) return;
        await CreatureCmd.Heal(Owner.Creature, 1m);
        Flash();
    }
}

public class GildedSphinx : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (Owner?.Creature == null) return;
        if (card.Owner != Owner) return;
        if (Owner.Creature.IsDead) return;
        CombatState? combat = Owner.Creature.CombatState;
        if (combat == null) return;

        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .Where(c => !c.EnergyCost.CostsX)
            .Where(c => c.EnergyCost.GetWithModifiers(CostModifiers.All) == 0)
            .ToList();
        if (candidates.Count == 0) return;

        CardModel? pickedModel = Owner.RunState.Rng.CombatCardGeneration.NextItem(candidates);
        if (pickedModel == null) return;

        Flash();
        CardModel generated = combat.CreateCard(pickedModel, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: true, CardPilePosition.Top);
    }
}

[HarmonyPatch(typeof(CharacterModel), "IconOutlineTexturePath", MethodType.Getter)]
public static class MinorisIconOutlineTexturePathPatch
{
    private static bool Prefix(CharacterModel __instance, ref string? __result)
    {
        if (__instance is not Minoris.MinorisCode.Character.Minoris)
            return true;

        var outlinePath = "character_icon_minoris_outline.png".CharacterUiPath();
        __result = ResourceLoader.Exists(outlinePath) ? outlinePath : "character_icon_minoris.png".CharacterUiPath();
        return false;
    }
}
