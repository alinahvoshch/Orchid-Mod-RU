﻿using Terraria;
using Terraria.ID;

namespace OrchidMod.Content.Alchemist.Projectiles.Nature
{
	public class SunflowerFlaskProj3 : OrchidModAlchemistProjectile
	{
		private int rotationDirection = 0;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sunflower");
		}

		public override void SafeSetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			if (!this.Initialized)
			{
				this.rotationDirection = Main.rand.NextBool(2)? 1 : -1;
				this.Initialized = true;
			}

			Projectile.rotation += 0.04f * this.rotationDirection;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.player[Projectile.owner].HasBuff<Buffs.StardustSamplesBuff>())
			{
				target.AddBuff(BuffID.Confused, 60 * 5);
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 250);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
			}
		}
	}
}