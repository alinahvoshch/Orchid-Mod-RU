using Microsoft.Xna.Framework;
using OrchidMod.Content.Guardian.Projectiles.Gauntlets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Guardian.Weapons.Gauntlets
{
	public class IceGauntlet : OrchidModGuardianGauntlet
	{
		public override void SafeSetDefaults()
		{
			Item.width = 32;
			Item.height = 34;
			Item.knockBack = 3f;
			Item.damage = 270;
			Item.value = Item.sellPrice(0, 4, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.useTime = 25;
			StrikeVelocity = 25f;
			ParryDuration = 100;
			ChargeSpeedMultiplier = 2f;
		}

		public override void OnParryGauntlet(Player player, OrchidGuardian guardian, Entity aggressor, Projectile anchor)
		{
			int projectileType = ModContent.ProjectileType<IceGauntletProjectile>();
			int count = 0;
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile.active && projectile.type == projectileType && player.whoAmI == projectile.owner)
				{
					count++;
					projectile.ai[0] = 0;
					projectile.ai[1] = 0;
					projectile.netUpdate = true;
				}
			}

			if (count < 3)
			{
				Projectile newProjectile = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, projectileType, 0, 0f, player.whoAmI, player.whoAmI);
			}
		}

		public override Color GetColor(bool offHand)
		{
			return new Color(156, 219, 235);
		}
	}
}
