
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD049_STRESS_RESPONSE
中文名称: 应激反应
英文名称: Stress Response
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 本回合中，每当你打出一张牌时，失去 1 点生命，获得 {Amount:diff()} 点格挡。
卡牌描述(ZHS): 本回合中，每当你打出一张牌时，失去 1 点生命，获得 {Amount:diff()} 点格挡。
卡牌描述(ENG): This turn, whenever you play a card, lose 1 HP and gain {Amount:diff()} Block.
升级效果: 本回合中，每当你打出一张牌时，失去1点生命，获得格挡+4
*/
public class Card049_StressResponse() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string AmountKey = "Amount";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(AmountKey, 8)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.StressResponsePower>(Owner.Creature, DynamicVars[AmountKey].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[AmountKey].UpgradeValueBy(4m);
    }
}









