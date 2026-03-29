
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD028_HAMMER_OF_GODS
中文名称: 神的锻锤
英文名称: Hammer of the Gods
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 2
卡牌效果: 造成 {Damage:diff()} 点伤害。这张牌可以无限次升级。这张牌会保留战斗中的升级。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。这张牌可以无限次升级。这张牌会保留战斗中的升级。
卡牌描述(ENG): Deal {Damage:diff()} damage. Can be upgraded infinitely. Upgrades made in combat are permanent.
升级效果: 伤害+8；效果增强（详见升级后描述）；效果增强（详见升级后描述）；效果增强（详见升级后描述）
*/
public class Card028_HammerOfGods() : MinorisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override int MaxUpgradeLevel => int.MaxValue;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
        if (CombatState != null && DeckVersion != null && DeckVersion.IsUpgradable)
        {
            var runEntry = Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(Owner.NetId);
            runEntry?.UpgradedCards.Add(DeckVersion.Id);
            DeckVersion.UpgradeInternal();
            DeckVersion.FinalizeUpgradeInternal();
        }
    }
}









