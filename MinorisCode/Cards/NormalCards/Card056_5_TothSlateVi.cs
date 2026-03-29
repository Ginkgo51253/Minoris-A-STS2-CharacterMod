
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_5_TOTH_SLATE_VI
中文名称: 透特石板-VI
英文名称: Toth Slate-VI
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 分土离精，精纯乃成。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。你可以选择一张手牌消耗。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-VII”。
卡牌描述(ZHS): 分土离精，精纯乃成。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。你可以选择一张手牌消耗。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-VII”。
卡牌描述(ENG): Gain 5 Block. Deal 5 damage. Gain 1 Energy. Heal 2 HP. You may Exhaust 1 card in your hand. After combat, transform this card in your deck into "Toth Slate-VII".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
[Pool(typeof(TokenCardPool))]
public class Card056_5_TothSlateVi() : TothSlateBase<Card056_6_TothSlateVii>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 6;
}









