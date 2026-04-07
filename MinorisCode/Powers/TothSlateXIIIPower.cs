
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 透特石板-ⅩⅢ
能力英文名称: Thoth Slate - XIII
能力描述(ZHS): 在你的回合结束时，触发[blue]{Amount}[/blue]次以下效果：获得5点格挡；对一名随机敌人造成5点伤害；获得1点能量；恢复2点生命值；你可以选择一张手牌消耗；获得1点力量；抽1张牌，并在本场战斗中保留抽到的这张牌；获得1点敏捷；获得4点活力；获得1层人工制品。
能力描述(ENG): At the end of your turn, trigger the following effect [blue]{Amount}[/blue] time(s): Gain 5 Block; Deal 5 damage to a random enemy; Gain 1 Energy; Heal 2 HP; You may choose a card in your hand to Exhaust; Gain 1 Strength; Draw 1 card and it gains Retain this combat; Gain 1 Dexterity; Gain 4 Vigor; Gain 1 Artifact.
相关卡牌（本地键）: MINORIS-CARD056_12_TOTH_SLATE_XIII
*/
public class TothSlateXiiiPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private const string BlockKey = "Block";
    private const string DamageKey = "Damage";
    private const string EnergyKey = "Energy";
    private const string HealKey = "Heal";
    private const string ExhaustKey = "Exhaust";
    private const string StrengthKey = "Strength";
    private const string DrawKey = "Draw";
    private const string DexterityKey = "Dexterity";
    private const string VigorKey = "Vigor";
    private const string ArtifactKey = "Artifact";
    private int _block = 5;
    private int _damage = 5;
    private int _energy = 1;
    private int _heal = 2;
    private int _exhaust = 1;
    private int _strength = 1;
    private int _draw = 1;
    private int _dexterity = 1;
    private int _vigor = 4;
    private int _artifact = 1;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource != null)
        {
            if (cardSource.DynamicVars.TryGetValue(BlockKey, out var v1)) _block = v1.IntValue;
            if (cardSource.DynamicVars.TryGetValue(DamageKey, out var v2)) _damage = v2.IntValue;
            if (cardSource.DynamicVars.TryGetValue(EnergyKey, out var v3)) _energy = v3.IntValue;
            if (cardSource.DynamicVars.TryGetValue(HealKey, out var v4)) _heal = v4.IntValue;
            if (cardSource.DynamicVars.TryGetValue(ExhaustKey, out var v5)) _exhaust = v5.IntValue;
            if (cardSource.DynamicVars.TryGetValue(StrengthKey, out var v6)) _strength = v6.IntValue;
            if (cardSource.DynamicVars.TryGetValue(DrawKey, out var v7)) _draw = v7.IntValue;
            if (cardSource.DynamicVars.TryGetValue(DexterityKey, out var v8)) _dexterity = v8.IntValue;
            if (cardSource.DynamicVars.TryGetValue(VigorKey, out var v9)) _vigor = v9.IntValue;
            if (cardSource.DynamicVars.TryGetValue(ArtifactKey, out var v10)) _artifact = v10.IntValue;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        for (var i = 0; i < Amount; i++)
        {
            await CreatureCmd.GainBlock(Owner, _block, ValueProp.Move, null);

            var enemies = CombatState.GetOpponentsOf(Owner).Where(c => c.IsAlive).ToList();
            if (enemies.Count > 0 && Owner.Player != null)
            {
                var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies);
                await CreatureCmd.Damage(choiceContext, target, _damage, ValueProp.Move, Owner);
                await CheckWinConditionIfCombatEnding();
                if (!CombatManager.Instance.IsInProgress) return;
            }

            if (Owner.Player != null) await PlayerCmd.GainEnergy(_energy, Owner.Player);
            await CreatureCmd.Heal(Owner, _heal);

            if (Owner.Player != null)
            {
                var hand = PileType.Hand.GetPile(Owner.Player).Cards.ToList();
                if (hand.Count > 0)
                {
                    if (_exhaust > 0)
                    {
                        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, _exhaust);
                        var picks = await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner.Player, prefs);
                        foreach (var p in picks.Take(_exhaust))
                        {
                            if (p != null) await CardCmd.Exhaust(choiceContext, p);
                        }
                    }
                }

                if (_strength != 0) await PowerCmd.Apply<StrengthPower>(Owner, _strength, Owner, null);

                if (_draw > 0)
                {
                    for (var d = 0; d < _draw; d++)
                    {
                        var drawn = await CardPileCmd.Draw(choiceContext, Owner.Player);
                        if (drawn != null) CardCmd.ApplyKeyword(drawn, CardKeyword.Retain);
                    }
                }

                if (_dexterity != 0) await PowerCmd.Apply<DexterityPower>(Owner, _dexterity, Owner, null);
                if (_vigor != 0) await PowerCmd.Apply<VigorPower>(Owner, _vigor, Owner, null);
                if (_artifact != 0) await PowerCmd.Apply<ArtifactPower>(Owner, _artifact, Owner, null);
            }
        }

        Flash();
    }
}













