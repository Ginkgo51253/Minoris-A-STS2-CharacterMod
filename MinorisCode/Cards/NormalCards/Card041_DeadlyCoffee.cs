
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD041_DEADLY_COFFEE
中文名称: 致命咖啡
英文名称: Deadly Coffee
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 0
卡牌效果: 获得 {EnergyGain:diff()} 点能量。在你的回合结束时，受到 6 点伤害。
卡牌描述(ZHS): 获得 {EnergyGain:diff()} 点能量。在你的回合结束时，受到 6 点伤害。
卡牌描述(ENG): Gain {EnergyGain:diff()} Energy. At the end of your turn, take 6 damage.
升级效果: 获得点能量+1
*/
public class Card041_DeadlyCoffee() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string EnergyGainKey = "EnergyGain";
    private const string DamageKey = "Damage";

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new EnergyVar(EnergyGainKey, 2),
        new IntVar(DamageKey, 6)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Owner.PlayerCombatState!.GainEnergy(DynamicVars[EnergyGainKey].IntValue);
        await PowerCmd.Apply<Powers.DeadlyCoffeePower>(Owner.Creature, DynamicVars[DamageKey].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars[EnergyGainKey].UpgradeValueBy(1m);
    }
}








