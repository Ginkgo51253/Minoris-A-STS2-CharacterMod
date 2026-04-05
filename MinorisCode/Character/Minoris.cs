using System.Collections.Generic;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using Minoris.MinorisCode.Extensions;
using Minoris.MinorisCode.Cards;
using Minoris.MinorisCode.Relics;

namespace Minoris.MinorisCode.Character;

public class Minoris : PlaceholderCharacterModel
{
    public const string CharacterId = "MINORIS";

    public static readonly Color Color = new("ffe100");
    public static readonly Color EnergyOutlineColor = new("4B4511FF");
    public static readonly Color EnergyLabelOutline = new("4B4511FF");
    public static readonly Color Dialogue = new("7A4E00FF");
    public static readonly Color MapDrawing = new("FFFACDFF");
    public static readonly Color RemoteTargetingLine = new("FFE07AFF");
    public static readonly Color RemoteTargetingOutline = new("4B4511FF");
    private const string DefaultCharUiIcon = "char_select_minoris.png";

    public override Color NameColor => Color;
    public override Color EnergyLabelOutlineColor => EnergyLabelOutline;
    public override Color DialogueColor => Dialogue;
    public override Color MapDrawingColor => MapDrawing;
    public override Color RemoteTargetingLineColor => RemoteTargetingLine;
    public override Color RemoteTargetingLineOutline => RemoteTargetingOutline;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;

    public override CustomEnergyCounter? CustomEnergyCounter =>
        new CustomEnergyCounter(EnergyCounterLayerPath, EnergyOutlineColor, Color);
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<Card001_Strike>(),
        ModelDb.Card<Card001_Strike>(),
        ModelDb.Card<Card001_Strike>(),
        ModelDb.Card<Card001_Strike>(),
        ModelDb.Card<Card002_Defend>(),
        ModelDb.Card<Card002_Defend>(),
        ModelDb.Card<Card002_Defend>(),
        ModelDb.Card<Card002_Defend>(),
        ModelDb.Card<Card003_InstinctScratch>(),
        ModelDb.Card<Card004_IntuitiveDodge>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SmallBowTie>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<MinorisCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MinorisRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MinorisPotionPool>();

    public override string CustomCharacterSelectBg =>
        ResourceLoader.Exists("res://Minoris/scenes/screens/char_select/char_select_bg_minoris.tscn")
            ? "res://Minoris/scenes/screens/char_select/char_select_bg_minoris.tscn"
            : base.CustomCharacterSelectBg;

    public override string CustomIconPath =>
        ResourceLoader.Exists("res://Minoris/scenes/ui/character_icons/minoris_icon.tscn")
            ? "res://Minoris/scenes/ui/character_icons/minoris_icon.tscn"
            : base.CustomIconPath;

    public override string? CustomIconTexturePath => ResourceLoader.Exists("character_icon_minoris.png".CharacterUiPath()) ? "character_icon_minoris.png".CharacterUiPath() : base.CustomIconTexturePath;
    public override string? CustomCharacterSelectIconPath => ResourceLoader.Exists("char_select_minoris.png".CharacterUiPath()) ? "char_select_minoris.png".CharacterUiPath() : base.CustomCharacterSelectIconPath;
    public override string? CustomCharacterSelectLockedIconPath => ResourceLoader.Exists("char_select_minoris_locked.png".CharacterUiPath()) ? "char_select_minoris_locked.png".CharacterUiPath() : base.CustomCharacterSelectLockedIconPath;
    public override string? CustomMapMarkerPath => ResourceLoader.Exists("map_marker_minoris.png".CharacterUiPath()) ? "map_marker_minoris.png".CharacterUiPath() : base.CustomMapMarkerPath;
    
    public override string CustomArmPointingTexturePath => ResourceLoader.Exists("multiplayer_hand_minoris_point.png".CharacterUiPath()) ? "multiplayer_hand_minoris_point.png".CharacterUiPath() : base.CustomArmPointingTexturePath;
    public override string CustomArmRockTexturePath => ResourceLoader.Exists("multiplayer_hand_minoris_rock.png".CharacterUiPath()) ? "multiplayer_hand_minoris_rock.png".CharacterUiPath() : base.CustomArmRockTexturePath;
    public override string CustomArmPaperTexturePath => ResourceLoader.Exists("multiplayer_hand_minoris_paper.png".CharacterUiPath()) ? "multiplayer_hand_minoris_paper.png".CharacterUiPath() : base.CustomArmPaperTexturePath;
    public override string CustomArmScissorsTexturePath => ResourceLoader.Exists("multiplayer_hand_minoris_scissors.png".CharacterUiPath()) ? "multiplayer_hand_minoris_scissors.png".CharacterUiPath() : base.CustomArmScissorsTexturePath;

    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];

    private string EnergyCounterLayerPath(int layer)
    {
        var actualLayer = layer == 4 ? 5 : layer;
        return $"ui/combat/energy_counters/minoris_orb_layer_{actualLayer}.png".ImagePath();
    }
}
