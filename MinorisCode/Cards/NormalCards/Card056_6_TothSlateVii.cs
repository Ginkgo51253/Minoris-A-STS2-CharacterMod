
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_6_TOTH_SLATE_VII
中文名称: 透特石板-VII
英文名称: Toth Slate-VII
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 地藏全功，力贯乾坤。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。你可以选择一张手牌消耗。获得1点力量。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-VIII”。
卡牌描述(ZHS): 地藏全功，力贯乾坤。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。你可以选择一张手牌消耗。获得1点力量。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-VIII”。
卡牌描述(ENG): Gain 5 Block. Deal 5 damage. Gain 1 Energy. Heal 2 HP. You may Exhaust 1 card in your hand. Gain 1 Strength. After combat, transform this card in your deck into "Toth Slate-VIII".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
[Pool(typeof(TokenCardPool))]
public class Card056_6_TothSlateVii() : TothSlateBase<Card056_7_TothSlateViii>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 7;
}









