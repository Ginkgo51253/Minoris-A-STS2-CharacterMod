
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD038_PAPER_PLANE
中文名称: 回旋纸飞机
英文名称: Boomerang Paper Plane
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 0
卡牌效果: 获得 {Block:diff()} 点格挡。每当你打出 3 张攻击牌，将它置入你的手牌。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。每当你打出 3 张攻击牌，将它置入你的手牌。
卡牌描述(ENG): Gain {Block:diff()} Block. Whenever you play 3 Attacks, return this to your hand.
升级效果: 格挡+3
*/
public class Card038_PaperPlane() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    public override bool ShouldReceiveCombatHooks => true;
    private int _AttacksPerReturn = 3;
    private int _attacksThisTurn;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Creature.Side == side) _attacksThisTurn = 0;
        await Task.CompletedTask;
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner) return;
        if (playedCard.Type == CardType.Attack)
        {
            _attacksThisTurn++;
            if (_attacksThisTurn >= _AttacksPerReturn)
            {
                var inHand = PileType.Hand.GetPile(Owner).Cards.Contains(this);
                if (!inHand)
                {
                    if (PileType.Draw.GetPile(Owner).Cards.Contains(this)
                        || PileType.Discard.GetPile(Owner).Cards.Contains(this)
                        || PileType.Exhaust.GetPile(Owner).Cards.Contains(this))
                    {
                        await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Top);
                    }
                }
                _attacksThisTurn = 0;
            }
        }
    }
}









