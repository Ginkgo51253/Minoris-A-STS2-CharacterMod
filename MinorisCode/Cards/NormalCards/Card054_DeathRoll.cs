
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD054_DEATH_ROLL
中文名称: 死亡翻滚
英文名称: Death Roll
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 1
卡牌效果: 给予 {GripPower:diff()} 层牵制。{IfUpgraded:show:对所有敌人生效。|对一名敌人生效。}
卡牌描述(ZHS): 给予 {GripPower:diff()} 层牵制。{IfUpgraded:show:对所有敌人生效。|对一名敌人生效。}
卡牌描述(ENG): Apply {GripPower:diff()} Grip. {IfUpgraded:show:\nTargets ALL enemies.|Targets a single enemy.}
升级效果: 对所有敌人生效
*/
public class Card054_DeathRoll() : MinorisCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.GripPower>(6)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        if (IsUpgraded)
        {
            await PowerCmd.Apply<Powers.GripPower>(CombatState.HittableEnemies, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
        }
        else
        {
            if (cardPlay.Target == null) return;
            await PowerCmd.Apply<Powers.GripPower>(cardPlay.Target, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
        }
    }
}









