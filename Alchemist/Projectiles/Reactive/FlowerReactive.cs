using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Alchemist.Projectiles.Reactive
{
	public class FlowerReactive : AlchemistProjReactive
	{
		public override void SafeSetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 600;
			Projectile.scale = 1f;
			this.spawnTimeLeft = 600;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flower");
		}

		public override void SafeAI()
		{
			Projectile.rotation += 0.05f;
			Projectile.velocity *= 0.95f;

			if (Main.rand.Next(60) == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<Content.Dusts.BloomingAltDust>());
				Main.dust[dust].velocity *= 0.1f;
				Main.dust[dust].scale *= 1f;
				Main.dust[dust].noGravity = true;
			}
		}

		public override void Despawn()
		{
			for (int i = 0; i < 5; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<Content.Dusts.BloomingAltDust>());
				Main.dust[dust].velocity *= 1.5f;
				Main.dust[dust].scale *= 1f;
				Main.dust[dust].noGravity = true;
			}
		}

		public override void SafeKill(int timeLeft, Player player, OrchidModPlayer modPlayer)
		{
			SoundEngine.PlaySound(2, (int)Projectile.position.X, (int)Projectile.position.Y, 17);
			int proj = ProjectileType<Alchemist.Projectiles.Reactive.ReactiveSpawn.BloomingPetal>();
			int dmg = Projectile.damage;
			int rand = Main.rand.Next(45);
			for (int i = 0; i < 4; i++)
			{
				Vector2 perturbedSpeed = new Vector2(0f, 0.5f).RotatedBy(MathHelper.ToRadians(rand + i * 90));
				int newProj = Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, proj, dmg, 1f, Projectile.owner, 0.0f, 0.0f);
			}
		}
	}
}