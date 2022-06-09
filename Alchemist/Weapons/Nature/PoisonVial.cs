using Microsoft.Xna.Framework;
using OrchidMod.Alchemist.Projectiles;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Alchemist.Weapons.Nature
{
	public class PoisonVial : OrchidModAlchemistItem
	{
		public override void SafeSetDefaults()
		{
			Item.damage = 15;
			Item.width = 30;
			Item.height = 30;
			Item.rare = 2;
			Item.value = Item.sellPrice(0, 0, 30, 0);
			this.potencyCost = 2;
			this.element = AlchemistElement.NATURE;
			this.rightClickDust = 44;
			this.colorR = 130;
			this.colorG = 151;
			this.colorB = 31;
			this.secondaryDamage = 14;
			this.secondaryScaling = 7f;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Poison Flask");
			Tooltip.SetDefault("Releases lingering poison bubbles"
							+ "\nHas a chance to release a catalytic poison bubble"
							+ "\nCan contaminate other bubbly weapons effects");
		}

		public override void KillSecond(int timeLeft, Player player, OrchidModPlayer modPlayer, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem)
		{
			int nb = 2 + Main.rand.Next(2);
			for (int i = 0; i < nb; i++)
			{
				Vector2 vel = (new Vector2(0f, -(float)(3 + Main.rand.Next(4))).RotatedByRandom(MathHelper.ToRadians(90)));
				int spawnProj = ProjectileType<Alchemist.Projectiles.Nature.PoisonVialProjAlt>();
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vel.X, vel.Y, spawnProj, 0, 0f, projectile.owner);
			}
			int dmg = getSecondaryDamage(player, modPlayer, alchProj.nbElements);
			nb = alchProj.hasCloud() ? 2 : 1;
			for (int i = 0; i < nb; i++)
			{
				Vector2 vel = (new Vector2(0f, -2.5f).RotatedByRandom(MathHelper.ToRadians(30)));
				vel *= (float)(1 - (Main.rand.Next(10) / 10));
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vel.X, vel.Y, ProjectileType<Alchemist.Projectiles.Nature.PoisonVialProj>(), dmg, 0.5f, projectile.owner);
			}
		}

		public override void OnHitNPCSecond(NPC target, int damage, float knockback, bool crit, Player player, OrchidModPlayer modPlayer,
		OrchidModAlchemistNPC modTarget, OrchidModGlobalNPC modTargetGlobal, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem)
		{
			target.AddBuff(BuffID.Poisoned, 60 * 5);
			int rand = alchProj.nbElements;
			rand += alchProj.hasCloud() ? 2 : 0;
			if (Main.rand.Next(10) < rand && !alchProj.noCatalyticSpawn)
			{
				int dmg = getSecondaryDamage(player, modPlayer, alchProj.nbElements);
				int proj = ProjectileType<Alchemist.Projectiles.Reactive.PoisonBubble>();
				Vector2 perturbedSpeed = new Vector2(0f, -5f).RotatedByRandom(MathHelper.ToRadians(20));
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, proj, dmg, 0f, projectile.owner);
			}
		}
	}
}
