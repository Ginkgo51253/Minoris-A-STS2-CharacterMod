using BaseLib.Abstracts;
using Minoris.MinorisCode.Character;

namespace Minoris.MinorisCode.Potions;

[BaseLib.Utils.Pool(typeof(MinorisPotionPool))]
public abstract class MinorisPotion : CustomPotionModel
{
}
