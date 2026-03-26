using Terraria;
using Terraria.ID;

namespace OrchidMod.Content.Guardian.Weapons.Warhammers
{
	public class IceHammer : OrchidModGuardianHammer
	{
		public override void SafeSetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.knockBack = 5f;
			Item.shootSpeed = 15f;
			Item.damage = 200;
			Item.useTime = 28;
			Range = 30;
			GuardStacks = 2;
			SlamStacks = 1;
			ReturnSpeed = 1.5f;
			SwingChargeGain = 1f;
			BlockDuration = 180;
		}
	}
}
