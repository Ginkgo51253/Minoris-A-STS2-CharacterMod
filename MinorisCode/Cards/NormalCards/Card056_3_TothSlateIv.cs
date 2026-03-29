
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_3_TOTH_SLATE_IV
中文名称: 透特石板-IV
英文名称: Toth Slate-IV
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 太一化生，万物由之。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-V”。
卡牌描述(ZHS): 太一化生，万物由之。
获得5点格挡。对一名敌人造成5点伤害。获得1点能量。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-V”。
卡牌描述(ENG): Gain 5 Block. Deal 5 damage. Gain 1 Energy. After combat, transform this card in your deck into "Toth Slate-V".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
[Pool(typeof(TokenCardPool))]
public class Card056_3_TothSlateIv() : TothSlateBase<Card056_4_TothSlateV>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 4;
}









