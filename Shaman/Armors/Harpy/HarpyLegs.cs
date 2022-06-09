using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Shaman.Armors.Harpy
{
	[AutoloadEquip(EquipType.Legs)]
	public class HarpyLegs : OrchidModShamanEquipable
	{
		public override void SafeSetDefaults()
		{
			Item.width = 22;
			Item.height = 14;
			Item.value = Item.sellPrice(0, 0, 20, 50);
			Item.rare = 2;
			Item.defense = 4;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Harpy Legs");
			Tooltip.SetDefault("6% increased shamanic damage"
							 + "\n10% increased movement speed");
		}

		public override void UpdateEquip(Player player)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			player.moveSpeed += 0.1f;
			modPlayer.shamanDamage += 0.06f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(Mod);
			recipe.AddIngredient(null, "HarpyTalon", 2);
			recipe.AddIngredient(ItemID.Feather, 4);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
