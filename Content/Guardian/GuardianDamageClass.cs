﻿using Terraria;
using Terraria.ModLoader;

namespace OrchidMod.Content.Guardian
{
	public class GuardianDamageClass : DamageClass
	{
		public override bool UseStandardCritCalcs => true;

		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
		{
			if (damageClass == Generic) return StatInheritanceData.Full;
			return StatInheritanceData.None;
		}

		public override bool GetEffectInheritance(DamageClass damageClass) => false;

		public override void SetDefaultStats(Player player)
		{
		}
	}
}