using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Shaman.Armors.Abyss
{
	[AutoloadEquip(EquipType.Legs)]
	public class AbyssalGreaves : OrchidModShamanEquipable
	{
		public override void SafeSetDefaults()
		{
			Item.width = 26;
			Item.height = 18;
			Item.value = 0;
			Item.rare = ItemRarityID.Red;
			Item.defense = 20;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Abyssal Greaves");
			Tooltip.SetDefault("10% increased shamanic damage"
								+ "\n10% increased movement speed");
		}

		public override void UpdateEquip(Player player)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			player.moveSpeed += 0.1f;
			modPlayer.shamanDamage += 0.1f;
			Lighting.AddLight(player.position, 0.15f, 0.15f, 0.8f);
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadowLokis = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(Mod);
			recipe.AddIngredient(ItemID.LunarBar, 12);
			recipe.AddIngredient(ModContent.ItemType<Misc.AbyssFragment>(), 15);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			OrchidHelper.DrawSimpleItemGlowmaskInWorld(Item, spriteBatch, ModContent.GetTexture("OrchidMod/Glowmasks/AbyssalGreaves_Glowmask"), Color.White, rotation, scale);
		}
	}
}
