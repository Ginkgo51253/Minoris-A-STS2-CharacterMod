
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 抓挠增幅
能力英文名称: Scratch Amp
能力描述(ZHS): 你的“抓挠”攻击牌造成的伤害提高[blue]{Amount}[/blue]点。
能力描述(ENG): Your "Scratch" Attacks deal [blue]{Amount}[/blue] additional damage.
相关卡牌（本地键）: MINORIS-CARD017_ELEGANT_SCRATCH
*/
public class ScratchAmpPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner) return 0m;
        if (cardSource == null) return 0m;
        if (!cardSource.GetType().Name.Contains("Scratch")) return 0m;
        return Amount;
    }
}














