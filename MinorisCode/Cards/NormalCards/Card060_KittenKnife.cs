
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD060_KITTEN_KNIFE
中文名称: 小猫刀
英文名称: Kitten Knife
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 在你每次攻击造成伤害后，追加造成 {KittenKnifePower:diff()} 点伤害。
卡牌描述(ZHS): 在你每次攻击造成伤害后，追加造成 {KittenKnifePower:diff()} 点伤害。
卡牌描述(ENG): Whenever your Attacks deal damage, deal {KittenKnifePower:diff()} additional damage.
升级效果: 在你每次攻击造成伤害后，追加造成伤害+1
*/
public class Card060_KittenKnife() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.KittenKnifePower>(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.KittenKnifePower>(Owner.Creature, DynamicVars["KittenKnifePower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["KittenKnifePower"].UpgradeValueBy(1m);
    }
}









