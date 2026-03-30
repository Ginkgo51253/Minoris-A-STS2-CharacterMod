
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD019_SOBEK_STRIKE
中文名称: 灾鳄打击
英文名称: Sobek Strike
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: CardTag.Strike
费用: 1
卡牌效果: 造成 {Damage:diff()} 点伤害。给予 {GripPower:diff()} 层牵制。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。给予 {GripPower:diff()} 层牵制。
卡牌描述(ENG): Deal {Damage:diff()} damage. Apply {GripPower:diff()} Grip.
升级效果: 给予层牵制+1
*/
public class Card019_SobekStrike() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<Powers.GripPower>(1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<Powers.GripPower>(cardPlay.Target, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GripPower"].UpgradeValueBy(1m);
    }
}









