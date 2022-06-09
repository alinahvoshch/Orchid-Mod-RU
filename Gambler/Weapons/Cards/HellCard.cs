using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Gambler.Weapons.Cards
{
	public class HellCard : OrchidModGamblerItem
	{
		public override void SafeSetDefaults()
		{
			Item.value = Item.sellPrice(0, 0, 10, 0);
			Item.rare = 1;
			Item.damage = 48;
			Item.crit = 4;
			Item.knockBack = 3f;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.shootSpeed = 13f;
			this.cardRequirement = 3;
			this.gamblerCardSets.Add("Biome");
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Playing Card : Hell");
			Tooltip.SetDefault("Launches fiery mortar"
							+ "\nPeriodically summons a pepper, replicating the attack");
		}

		public override void GamblerShoot(Player player, Vector2 position, float speedX, float speedY, int type, int damage, float knockBack, bool dummy = false)
		{
			SoundEngine.PlaySound(2, (int)player.Center.X, (int)player.Center.Y - 200, 1);
			int projType = ProjectileType<Gambler.Projectiles.HellCardProjAlt>();
			
			for (int l = 0; l < Main.projectile.Length; l++)
			{
				Projectile proj = Main.projectile[l];
				if (proj.active && proj.type == projType && proj.owner == player.whoAmI && proj.ai[1] == 0f)
				{
					float distance = (position - proj.Center).Length();
					if (distance < 500f) {
						return;
					}
				}
			}
			
			Vector2 vel = (new Vector2(0f, -1f).RotatedBy(MathHelper.ToRadians(10)));
			int newProjectile = OrchidModGamblerHelper.DummyProjectile(Projectile.NewProjectile(position.X, position.Y, vel.X, vel.Y, projType, damage, knockBack, player.whoAmI), dummy); 
			Main.projectile[newProjectile].ai[1] = 0f;
			Main.projectile[newProjectile].netUpdate = true;
			for (int i = 0; i < 5; i++)
			{
				int dustType = 31;
				Main.dust[Dust.NewDust(player.Center, 10, 10, dustType)].velocity *= 0.25f;
			}
		}
	}
}
