
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_2_TOTH_SLATE_III
中文名称: 透特石板-III
英文名称: Toth Slate-III
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 上下一致，万法归一。
获得5点格挡。对一名敌人造成5点伤害。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-IV”。
卡牌描述(ZHS): 上下一致，万法归一。
获得5点格挡。对一名敌人造成5点伤害。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-IV”。
卡牌描述(ENG): Gain 5 Block. Deal 5 damage. After combat, transform this card in your deck into "Toth Slate-IV".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
[Pool(typeof(TokenCardPool))]
public class Card056_2_TothSlateIii() : TothSlateBase<Card056_3_TothSlateIv>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 3;
}









