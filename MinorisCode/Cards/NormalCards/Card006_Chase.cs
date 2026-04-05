
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD006_CHASE
中文名称: 追击
英文名称: Chase
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 造成 {Damage:diff()} 点伤害。若你上一张打出的是攻击牌，获得 1 点能量。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。若你上一张打出的是攻击牌，获得 1 点能量。
卡牌描述(ENG): Deal {Damage:diff()} damage. If your last card played was an Attack, gain 1 Energy.
升级效果: 伤害+4
*/
public class Card006_Chase() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move), new EnergyVar(1)];
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
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (WasLastCardPlayedAttack) Owner.PlayerCombatState!.GainEnergy(1);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}








