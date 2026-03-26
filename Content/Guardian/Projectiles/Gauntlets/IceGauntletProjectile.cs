using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrchidMod.Content.Guardian.Weapons.Gauntlets;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Guardian.Projectiles.Gauntlets
{
	public class IceGauntletProjectile : OrchidModGuardianProjectile
	{
		public int TimeSpent = 0;

		public override void SafeSetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.friendly = false;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 300;
			Projectile.scale = 1f;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		public override void AI()
		{
			if (Projectile.ai[2] == 0f)
			{ // random rotation & scale (init)
				Projectile.rotation = Main.rand.NextFloat(MathHelper.Pi);
				SoundEngine.PlaySound(SoundID.Item28.WithPitchOffset(-0.3f), Projectile.Center);
			}

			Projectile.ai[2]++; // time spent alive
			Projectile.rotation += 0.05f;

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce);
				dust.velocity *= 0.5f;
				dust.noGravity = true;
			}

			Player owner = Owner;
			// Movement relative to player

			if (Projectile.ai[1] == 0f)
			{ // for netsync
				Projectile.localAI[1] = 0f;
			}

			int count = 0; // nb of more recent projectiles
			int countTotal = 0; // nb of other projectiles
			float highestTimespent = 0;
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile.active && projectile.type == Type && Projectile.owner == projectile.owner)
				{
					countTotal++;

					if (projectile.ai[2] < Projectile.ai[2])
					{
						count++;
					}

					if (projectile.ai[2] > highestTimespent)
					{
						highestTimespent = projectile.ai[2];
					}
				}
			}

			if (count < Projectile.ai[0])
			{
				count = (int)Projectile.ai[0];
			}
			else
			{
				Projectile.ai[0] = count;
			}

			if (countTotal < Projectile.ai[1])
			{
				countTotal = (int)Projectile.ai[1];
			}
			else
			{
				Projectile.ai[1] = countTotal;
			}

			if (highestTimespent < Projectile.localAI[0])
			{
				highestTimespent = (int)Projectile.localAI[0];
			}
			else
			{
				Projectile.localAI[0] = highestTimespent;
			}

			if (owner.active && !owner.dead &&  Projectile.timeLeft > 10)
			{
				if (Owner.HeldItem.type == ModContent.ItemType<IceGauntlet>())
				{ // Projectile persists while the player is holding the gauntlets
					Projectile.timeLeft = 300;
				}

				Vector2 targetPosition = owner.Center - Vector2.UnitY.RotatedBy(highestTimespent * 0.02f + (MathHelper.TwoPi / countTotal) * count) * (16f + Math.Max(owner.width, owner.height));
				Projectile.velocity = (targetPosition - Projectile.Center) * 0.1f + owner.velocity;

				foreach (Projectile projectile in Main.projectile)
				{
					if (projectile.type == ModContent.ProjectileType<GauntletPunchProjectile>() && owner.HeldItem.ModItem is IceGauntlet gauntlet && projectile.active && projectile.owner == Projectile.owner && projectile.ai[0] == 1f && projectile.Hitbox.Intersects(Projectile.Hitbox))
					{
						OrchidGuardian guardian = owner.GetModPlayer<OrchidGuardian>();
						int shardDamage = guardian.GetGuardianDamage(gauntlet.Item.damage * 0.3f);

						int projectileType = ModContent.ProjectileType<IceGauntletProjectileShard>();
						for (int i = 0; i < 7 + Main.rand.Next(4); i++)
						{
							Vector2 velocity = Vector2.Normalize(projectile.velocity).RotatedBy(MathHelper.ToRadians(-9f + i * Main.rand.NextFloat(2.5f, 3.5f))) * Main.rand.NextFloat(7.5f, 11.5f);
							Projectile newProjectile = Projectile.NewProjectileDirect(owner.GetSource_ItemUse(gauntlet.Item), projectile.Center, velocity, projectileType, shardDamage, 0.1f, owner.whoAmI);
							newProjectile.CritChance = (int)(owner.GetCritChance<GuardianDamageClass>() + owner.GetCritChance<GenericDamageClass>() + gauntlet.Item.crit);
							newProjectile.rotation = newProjectile.velocity.ToRotation();
							//newProjectile.velocity += owner.velocity * 1.5f;
							newProjectile.netUpdate = true;
						}

						for (int i = 0; i < 10; i ++)
						{
							SoundEngine.PlaySound(SoundID.Item27.WithPitchOffset(-0.3f), Projectile.Center);
							Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4, 4), 8, 8, DustID.SnowflakeIce);
							dust.velocity *= 1.5f;
							dust.noGravity = true;
						}

						Projectile.Kill();
						break;
					}
				}
			}
			else if (Projectile.timeLeft > 10)
			{
				Projectile.timeLeft = 10;
			}
		}

		public override bool OrchidPreDraw(SpriteBatch spriteBatch, ref Color lightColor)
		{
			/*
			spriteBatch.End(out SpriteBatchSnapshot spriteBatchSnapshot);
			spriteBatch.Begin(spriteBatchSnapshot with { BlendState = BlendState.Additive });
			*/

			// Draw code here

			Texture2D projTexture = TextureAssets.Projectile[Projectile.type].Value;

			float colorMult = 1f;
			if (Projectile.timeLeft < 10) colorMult *= Projectile.timeLeft / 30f;

			Color color = lightColor * 1.5f * colorMult;
			if (color.A > 128)
			{
				color.A = 128;
			}

			Vector2 drawPosition = Projectile.Center - Main.screenPosition;
			spriteBatch.Draw(projTexture, drawPosition, null, color, Projectile.rotation, projTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

			/*
			spriteBatch.End();
			spriteBatch.Begin(spriteBatchSnapshot);
			*/

			return false;
		}
	}
}