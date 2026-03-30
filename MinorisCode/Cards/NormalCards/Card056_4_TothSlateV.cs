
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_4_TOTH_SLATE_V
中文名称: 透特石板-V
英文名称: Toth Slate-V
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 日月为引，风土育成。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-VI”。
卡牌描述(ZHS): 日月为引，风土育成。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。恢复2点生命值。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-VI”。
卡牌描述(ENG): Gain 5 Block. Deal 5 damage. Gain 1 Energy. Heal 2 HP. After combat, transform this card in your deck into "Toth Slate-VI".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
[Pool(typeof(TokenCardPool))]
public class Card056_4_TothSlateV() : TothSlateBase<Card056_5_TothSlateVi>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 5;
}









