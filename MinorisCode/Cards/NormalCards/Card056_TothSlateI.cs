
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_TOTH_SLATE_I
中文名称: 透特石板-I
英文名称: Toth Slate-I
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 解读石板，开启箴言。
战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-II”。
卡牌描述(ZHS): 解读石板，开启箴言。
战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-II”。
卡牌描述(ENG): Interpret the slate. After combat, transform this card in your deck into "Toth Slate-II".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
public class Card056_TothSlateI() : TothSlateBase<Card056_1_TothSlateIi>(CardRarity.Rare, TargetType.None)
{
    protected override int Stage => 1;
}









