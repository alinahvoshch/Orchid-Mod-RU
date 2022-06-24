using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace OrchidMod.Shaman.Projectiles.Thorium
{
	public class ThoriumScepterProjSplit : OrchidModShamanProjectile
	{
		public override void SafeSetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.aiStyle = 1;
			Projectile.timeLeft = 500;
			Projectile.scale = 1f;
			AIType = ProjectileID.Bullet;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thorium Bolt");
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			OrchidModPlayerShaman modPlayer = player.GetModPlayer<OrchidModPlayerShaman>();
			int nbBonds = modPlayer.GetNbShamanicBonds();

			if (Projectile.timeLeft > 480)
			{
				Projectile.friendly = false;
			}
			else
			{
				Projectile.friendly = true;

				if (nbBonds > 2)
				{
					Projectile.extraUpdates = 1;
					ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

					if (Projectile.localAI[0] == 0f)
					{
						AdjustMagnitude(ref Projectile.velocity);
						Projectile.localAI[0] = 1f;
					}

					Vector2 move = Vector2.Zero;
					float distance = 600f;
					bool target = false;
					for (int k = 0; k < 200; k++)
					{
						if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
						{
							Vector2 newMove = Main.npc[k].Center - Projectile.Center;
							float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
							if (distanceTo < distance)
							{
								move = newMove;
								distance = distanceTo;
								target = true;
							}
						}
					}
					if (target)
					{
						AdjustMagnitude(ref move);
						Projectile.velocity = (20 * Projectile.velocity + move);
						AdjustMagnitude(ref Projectile.velocity);
					}
					else
					{
						Projectile.timeLeft -= 5;
					}
				}
				else
				{
					Projectile.timeLeft -= 15;
				}
			}

			if (Main.rand.Next(2) == 0)
			{
				int DustID = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 15, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 125, default(Color), 1.25f);
				Main.dust[DustID].noGravity = true;
			}
		}

		private void AdjustMagnitude(ref Vector2 vector)
		{
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 3f)
			{
				vector *= 3f / magnitude;
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 4; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity = Projectile.velocity / 2;
			}
		}
	}
}