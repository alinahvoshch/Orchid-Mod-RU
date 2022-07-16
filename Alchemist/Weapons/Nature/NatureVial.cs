using OrchidMod.Alchemist.Projectiles;
using Terraria;
using Terraria.ID;

namespace OrchidMod.Alchemist.Weapons.Nature
{
	public class NatureVial : OrchidModAlchemistItem
	{
		public override void SafeSetDefaults()
		{
			Item.damage = 5;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Blue;
			this.potencyCost = 1;
			this.element = AlchemistElement.NATURE;
			this.rightClickDust = 3;
			this.colorR = 75;
			this.colorG = 117;
			this.colorB = 0;
		}

		public override void AltSetStaticDefaults()
		{
			DisplayName.SetDefault("Nature Vial");
			Tooltip.SetDefault("\n[c/FF0000:Test Item]");
		}
	}
}
