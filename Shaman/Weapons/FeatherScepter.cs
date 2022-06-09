using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Shaman.Weapons
{
	public class FeatherScepter : OrchidModShamanItem
	{
		public override void SafeSetDefaults()
		{
			Item.damage = 13;
			Item.melee = false;
			Item.ranged = false;
			Item.magic = false;
			Item.thrown = false;
			Item.summon = false;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 33;
			Item.useAnimation = 33;
			Item.knockBack = 0f;
			Item.rare = 1;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.UseSound = SoundID.Item45;
			Item.autoReuse = true;
			Item.shootSpeed = 15f;
			Item.shoot = Mod.Find<ModProjectile>("FeatherScepterProj").Type;
			this.empowermentType = 3;
			this.energy = 6;
		}

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Feather Scepter");
			Tooltip.SetDefault("Shoots dangerous spinning feathers"
							  + "\nThe projectiles gain in damage after a while"
							  + "\nHaving 3 or more active shamanic bonds will result in more projectiles shot");
		}

		public override bool SafeShoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			int nbBonds = OrchidModShamanHelper.getNbShamanicBonds(player, modPlayer, Mod);

			if (nbBonds > 2)
			{
				Vector2 perturbedSpeed = new Vector2(speedX / 2, speedY / 2).RotatedByRandom(MathHelper.ToRadians(15));
				this.NewShamanProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(Mod);
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(null, "HarpyTalon", 2);
			recipe.AddIngredient(ItemID.Feather, 5);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
