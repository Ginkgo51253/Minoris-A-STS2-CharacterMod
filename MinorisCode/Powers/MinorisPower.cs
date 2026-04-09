﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿namespace Minoris.MinorisCode.Powers;


public abstract class MinorisPower : CustomPowerModel
{
    private const string MissingPowerIconPath = "res://images/powers/missing_power.png";

    public override string CustomPackedIconPath
    {
        get
        {
            var atlas = $"res://atlases/power_atlas.sprites/{Id.Entry.ToLowerInvariant()}.tres";
            if (ResourceLoader.Exists(atlas)) return atlas;
            var png = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(png) ? png : MissingPowerIconPath;
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : MissingPowerIconPath;
        }
    }

    protected static async Task CheckWinConditionIfCombatEnding()
    {
        if (!CombatManager.Instance.IsInProgress) return;
        if (!CombatManager.Instance.IsEnding) return;
        if (RunManager.Instance?.ActionExecutor?.CurrentlyRunningAction != null) return;
        await CombatManager.Instance.CheckWinCondition();
    }
}
