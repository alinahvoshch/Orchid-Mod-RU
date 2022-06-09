using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Shaman.Weapons
{
	public class Blum : OrchidModShamanItem
	{
		public override void SafeSetDefaults()
		{
			Item.damage = 17;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.knockBack = 3.25f;
			Item.rare = 2;
			Item.value = Item.sellPrice(0, 0, 47, 0);
			Item.UseSound = SoundID.Item21;
			Item.autoReuse = true;
			Item.shootSpeed = 16f;
			Item.shoot = Mod.Find<ModProjectile>("BlumProj").Type;
			this.empowermentType = 2;
			OrchidModGlobalItem orchidItem = Item.GetGlobalItem<OrchidModGlobalItem>();
			orchidItem.shamanWeaponNoUsetimeReforge = true;
			this.energy = 4;
		}

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Blum");
			Tooltip.SetDefault("Rapidly shoots dangerous magical bolts"
							  + "\nThe weapon speed depends on the number of active shamanic bonds");
		}

		public override void UpdateInventory(Player player)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			int nbBonds = OrchidModShamanHelper.getNbShamanicBonds(player, modPlayer, Mod);
			Item.useTime = 18 - (nbBonds * 2);
			Item.useAnimation = 18 - (nbBonds * 2);
		}
	}
}

