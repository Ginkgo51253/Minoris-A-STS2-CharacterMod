
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD040_HALFTIME
中文名称: 中场休息
英文名称: Halftime
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 0
卡牌效果: 若你上一张打出的是攻击牌，获得 {EnergyGain:diff()} 点能量。
卡牌描述(ZHS): 若你上一张打出的是攻击牌，获得 {EnergyGain:diff()} 点能量。
卡牌描述(ENG): If your last card played was an Attack, gain {EnergyGain:diff()} Energy.
升级效果: 若你上一张打出的是攻击牌，获得点能量+1
*/
public class Card040_Halftime() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string EnergyGainKey = "EnergyGain";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(EnergyGainKey, 2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedAttack;
    private bool WasLastCardPlayedAttack
    {
        get
        {
            var last = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault(e => e.CardPlay.Card.Owner == Owner && e.CardPlay.Card != this);
            if (last == null) return false;
            return last.CardPlay.Card.Type == CardType.Attack;
        }
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (WasLastCardPlayedAttack)
        {
            Owner.PlayerCombatState!.GainEnergy(DynamicVars[EnergyGainKey].IntValue);
        }
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars[EnergyGainKey].UpgradeValueBy(1m);
    }
}








