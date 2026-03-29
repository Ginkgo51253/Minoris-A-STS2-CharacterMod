
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_11_TOTH_SLATE_XII
中文名称: 透特石板-XII
英文名称: Toth Slate-XII
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 三界智慧，三重至名。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。你可以选择一张手牌消耗。获得1点力量。抽1张牌，并在本场战斗中保留抽到的这张牌。获得1点敏捷。获得4点活力。获得1层人工制品。重放2。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-XIII”。
卡牌描述(ZHS): 三界智慧，三重至名。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。你可以选择一张手牌消耗。获得1点力量。抽1张牌，并在本场战斗中保留抽到的这张牌。获得1点敏捷。获得4点活力。获得1层人工制品。重放2。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-XIII”。
卡牌描述(ENG): Gain 5 Block. Deal 5 damage. Gain 1 Energy. Heal 2 HP. You may Exhaust 1 card in your hand. Gain 1 Strength. Draw 1 card and it is Retained this combat. Gain 1 Dexterity. Gain 4 Vigor. Gain 1 Artifact. Replay 2. After combat, transform this card in your deck into "Toth Slate-XIII".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
[Pool(typeof(TokenCardPool))]
public class Card056_11_TothSlateXii() : TothSlateBase<Card056_12_TothSlateXiii>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 12;

    public override void AfterCreated()
    {
        base.AfterCreated();
        BaseReplayCount = 2;
    }

    protected override void AfterDeserialized()
    {
        base.AfterDeserialized();
        BaseReplayCount = 2;
    }

    public override void AfterTransformedTo()
    {
        base.AfterTransformedTo();
        BaseReplayCount = 2;
    }
}









