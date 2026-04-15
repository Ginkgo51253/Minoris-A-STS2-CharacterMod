
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD072_BAST_BLESSING
中文名称: 芭丝特赐福
英文名称: Bast's Blessing
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 你的攻击牌会额外打出1次
卡牌描述(ZHS): 你的攻击牌会额外打出1次
卡牌描述(ENG): Your Attacks are played twice.
升级效果: 费用-1
*/
public class Card072_BastBlessing() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const string AmountKey = "Amount";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(AmountKey, 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.BastBlessingPower>(Owner.Creature, DynamicVars[AmountKey].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}









