
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD056_1_TOTH_SLATE_II
中文名称: 透特石板-II
英文名称: Toth Slate-II
卡牌类型: CardType.Skill (TothSlateBase)
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 3
卡牌效果: 翠玉传书，真理不虚。
获得5点格挡。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-III”。
卡牌描述(ZHS): 翠玉传书，真理不虚。
获得5点格挡。战斗结束后，将你卡组中的这张牌变化为{IfUpgraded:show:[gold]升级过[/gold]的|}“透特石板-III”。
卡牌描述(ENG): Gain 5 Block. After combat, transform this card in your deck into "Toth Slate-III".
升级效果: 费用-1；战斗结束后变化出的下一阶段透特石板为升级状态
*/
[Pool(typeof(TokenCardPool))]
public class Card056_1_TothSlateIi() : TothSlateBase<Card056_2_TothSlateIii>(CardRarity.Token, TargetType.Self)
{
    protected override int Stage => 2;
}









