﻿﻿
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_12_TOTH_SLATE_XIII
中文名称: 透特石板-XIII
英文名称: Toth Slate-XIII
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 真理。
在你的回合结束时，触发3次以下效果：获得5点格挡；对一名随机敌人造成5点伤害；获得1点能量；恢复2点生命值；你可以选择一张手牌消耗；获得1点力量；抽1张牌，并在本场战斗中保留抽到的这张牌；获得1点敏捷；获得4点活力；获得1层人工制品。
卡牌描述(ZHS): 真理。
在你的回合结束时，触发3次以下效果：获得5点格挡；对一名随机敌人造成5点伤害；获得1点能量；恢复2点生命值；你可以选择一张手牌消耗；获得1点力量；抽1张牌，并在本场战斗中保留抽到的这张牌；获得1点敏捷；获得4点活力；获得1层人工制品。
卡牌描述(ENG): At the end of your turn, trigger the following effects 3 times: Gain 5 Block; deal 5 damage to a random enemy; gain 1 Energy; heal 2 HP; you may Exhaust 1 card in your hand; gain 1 Strength; draw 1 card and it is Retained this combat; gain 1 Dexterity; gain 4 Vigor; gain 1 Artifact.
升级效果: 费用-1
*/
[Pool(typeof(TokenCardPool))]
public class Card056_12_TothSlateXiii() : MinorisCard(3, CardType.Power, CardRarity.Token, TargetType.Self)
{
    private const string AmountKey = "Amount";
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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(AmountKey, 3),
        new IntVar(BlockKey, 5),
        new IntVar(DamageKey, 5),
        new EnergyVar(EnergyKey, 1),
        new IntVar(HealKey, 2),
        new IntVar(ExhaustKey, 1),
        new IntVar(StrengthKey, 1),
        new IntVar(DrawKey, 1),
        new IntVar(DexterityKey, 1),
        new IntVar(VigorKey, 4),
        new IntVar(ArtifactKey, 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.TothSlateXiiiPower>(Owner.Creature, DynamicVars[AmountKey].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}







