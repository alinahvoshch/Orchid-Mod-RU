using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Gambler.Accessories
{
	public class VultureCharm : OrchidModGamblerEquipable
	{
		public override void SafeSetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.sellPrice(0, 0, 10, 0);
			Item.rare = 1;
			Item.accessory = true;
			Item.crit = 4;
			Item.damage = 12;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vulture Charm");
			Tooltip.SetDefault("Drawing a card releases a burst of vulture feathers");
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			modPlayer.gamblerVulture = true;
		}

		public override void AddRecipes()
		{
			Mod thoriumMod = OrchidMod.ThoriumMod;

			ModRecipe recipe = new ModRecipe(Mod);
			recipe.AddRecipeGroup("IronBar", 5);
			recipe.AddIngredient((thoriumMod != null) ? thoriumMod.Find<ModItem>("BirdTalon").Type : Mod.Find<ModItem>("VultureTalon").Type, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}