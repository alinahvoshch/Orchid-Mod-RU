using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Shaman.Weapons
{
	public class AmberScepter : OrchidModShamanItem
	{
		public override void SafeSetDefaults()
		{
			item.damage = 26;
			item.width = 36;
			item.height = 38;
			item.useTime = 50;
			item.useAnimation = 50;
			item.knockBack = 4.75f;
			item.rare = 1;
			item.value = Item.sellPrice(0, 0, 40, 0);
			item.UseSound = SoundID.Item45;
			item.autoReuse = true;
			item.shootSpeed = 9f;
			item.shoot = mod.ProjectileType("AmberScepterProj");
			this.empowermentType = 4;
			this.energy = 6;
		}

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Amber Scepter");
			Tooltip.SetDefault("\nHitting an enemy will grant you an amber orb"
							  + "\nIf you have 3 amber orbs, your next hit will increase your maximum life for 30 seconds");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ItemID.Amber, 8);
			recipe.AddIngredient(3380, 15);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
