
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 透特石板-ⅩⅢ
能力英文名称: Thoth Slate - XIII
能力描述(ZHS): 在你的回合结束时，触发[blue]{Amount}[/blue]次以下效果：获得5点格挡；对一名随机敌人造成5点伤害；获得1点能量；恢复2点生命值；你可以选择一张手牌消耗；获得1点力量；抽1张牌，并在本场战斗中保留抽到的这张牌；获得1点敏捷；获得4点活力；获得1层人工制品。
能力描述(ENG): At the end of your turn, trigger the following effect [blue]{Amount}[/blue] time(s): Gain 5 Block; Deal 5 damage to a random enemy; Gain 1 Energy; Heal 2 HP; You may choose a card in your hand to Exhaust; Gain 1 Strength; Draw 1 card and it gains Retain this combat; Gain 1 Dexterity; Gain 4 Vigor; Gain 1 Artifact.
相关卡牌（本地键）: MINORIS-CARD056_12_TOTH_SLATE_XIII
*/
public class TothSlateXIIIPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        for (var i = 0; i < Amount; i++)
        {
            await CreatureCmd.GainBlock(Owner, 5m, ValueProp.Unpowered, null);

            var enemies = CombatState.GetOpponentsOf(Owner).Where(c => c.IsAlive).ToList();
            if (enemies.Count > 0 && Owner.Player != null)
            {
                var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies);
                await CreatureCmd.Damage(choiceContext, target, 5m, ValueProp.Unpowered, Owner);
                await CheckWinConditionIfCombatEnding();
                if (!CombatManager.Instance.IsInProgress) return;
            }

            if (Owner.Player != null) await PlayerCmd.GainEnergy(1, Owner.Player);
            await CreatureCmd.Heal(Owner, 2m);

            if (Owner.Player != null)
            {
                var hand = PileType.Hand.GetPile(Owner.Player).Cards.ToList();
                if (hand.Count > 0)
                {
                    var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
                    var pick = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner.Player, prefs)).FirstOrDefault();
                    if (pick != null) await CardCmd.Exhaust(choiceContext, pick);
                }

                await PowerCmd.Apply<StrengthPower>(Owner, 1, Owner, null);

                var drawn = await CardPileCmd.Draw(choiceContext, Owner.Player);
                if (drawn != null) CardCmd.ApplyKeyword(drawn, CardKeyword.Retain);

                await PowerCmd.Apply<DexterityPower>(Owner, 1, Owner, null);
                await PowerCmd.Apply<VigorPower>(Owner, 4, Owner, null);
                await PowerCmd.Apply<ArtifactPower>(Owner, 1, Owner, null);
            }
        }

        Flash();
    }
}














