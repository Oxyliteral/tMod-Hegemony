using Terraria;
using Terraria.ModLoader;

namespace Hegemony.Content.DamageClasses
{
	public class DamageClassTrueMelee : DamageClass
	{
		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
			if (damageClass == DamageClass.Generic || damageClass == DamageClass.Melee)
				return StatInheritanceData.Full;
			return StatInheritanceData.None;
		}

		public override bool GetEffectInheritance(DamageClass damageClass) {
			return damageClass == DamageClass.Generic || damageClass == DamageClass.Melee;
		}

		public override bool UseStandardCritCalcs => true;
	}
}