﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD071_SOLAR_FORM
中文名称: 太阳形态
英文名称: Solar Form
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 在你回合开始时：随机将{IfUpgraded:show:[gold]升级过[/gold]的|}攻击牌、技能牌、能力牌各一张置入你的手中。它们在本场战斗中为 0 费，但具有虚无。
卡牌描述(ZHS): 在你回合开始时：随机将{IfUpgraded:show:[gold]升级过[/gold]的|}攻击牌、技能牌、能力牌各一张置入你的手中。它们在本场战斗中为 0 费，但具有虚无。
卡牌描述(ENG): At the start of your turn, add a random{IfUpgraded:show: [gold]Upgraded[/gold]|} Attack, Skill, and Power to your hand. They cost 0 this combat, but are Ethereal.
升级效果: 置入你手牌的卡牌为升级状态
*/
public class Card071_SolarForm() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.SolarFormPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<Powers.SolarFormPowerPlus>(Owner.Creature, DynamicVars["SolarFormPower"].IntValue, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<Powers.SolarFormPower>(Owner.Creature, DynamicVars["SolarFormPower"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
