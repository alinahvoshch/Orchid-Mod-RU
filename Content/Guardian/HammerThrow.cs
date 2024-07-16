using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrchidMod.Common.ModObjects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Guardian
{
	public class HammerThrow : OrchidModProjectile
	{
		public List<Vector2> OldPosition;
		public List<float> OldRotation;

		public bool returning = false;
		public OrchidModGuardianHammer HammerItem;
		public Texture2D HammerTexture;

		public int range = 0;
		public int ChargeToAdd = 0;
		public bool penetrate;
		public bool hitTarget = false;
		public bool WeakHit = false;
		public int dir;

		public bool Ding = false;

		public bool WeakThrow() => Projectile.ai[0] == 1;

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

		public override void AltSetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 600;
			Projectile.alpha = 256;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 30;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Player player = Main.player[Projectile.owner];
			Item item = player.inventory[player.selectedItem];

			OldPosition = new List<Vector2>();
			OldRotation = new List<float>();

			if (item == null || !(item.ModItem is OrchidModGuardianHammer hammerItem))
			{
				Projectile.Kill();
				return;
			}
			else
			{
				HammerItem = hammerItem;
				HammerTexture = TextureAssets.Item[hammerItem.Item.type].Value;
				Projectile.width = (int)(HammerTexture.Width * hammerItem.Item.scale);
				Projectile.height = (int)(HammerTexture.Height * hammerItem.Item.scale);
				Projectile.scale = hammerItem.Item.scale;

				Projectile.position.X -= Projectile.width / 2;
				Projectile.position.Y -= Projectile.height / 2;

				range = HammerItem.range;
				penetrate = HammerItem.penetrate;
			}

			dir = (Projectile.velocity.X > 0 ? 1 : -1);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			OrchidGuardian guardian = player.GetModPlayer<OrchidGuardian>();

			if (player.dead) Projectile.Kill();
			if (HammerItem != null)
			{
				if (Projectile.ai[1] <= 0) // Held
				{
					if (Main.MouseWorld.X > player.Center.X && player.direction != 1) player.ChangeDir(1);
					else if (Main.MouseWorld.X < player.Center.X && player.direction != -1) player.ChangeDir(-1);

					player.itemAnimation = 1;
					Projectile.timeLeft = 600;
					Projectile.spriteDirection = -player.direction;
					player.heldProj = Projectile.whoAmI;

					if (guardian.GuardianThrowCharge >= 180f && !Ding)
					{
						Ding = true;
						SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
					}

					if (Projectile.ai[1] == 0)
					{
						if (WeakHit)
						{ // Projectiles just did a weak charge swing, kill it
							Projectile.Kill();
							return;
						}

						player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + guardian.GuardianThrowCharge * 0.006f * Projectile.spriteDirection); // set arm position (90 degree offset since arm starts lowered)
						Vector2 armPosition = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, MathHelper.Pi - guardian.GuardianThrowCharge * 0.006f * Projectile.spriteDirection);
						Projectile.Center = armPosition - new Vector2((Projectile.width + 0.3f * guardian.GuardianThrowCharge + (float)Math.Sin(MathHelper.Pi / 210f * guardian.GuardianThrowCharge) * 10f) * player.direction * 0.4f, (Projectile.height - (Projectile.height * 0.007f) * guardian.GuardianThrowCharge) * 0.4f);


						if (guardian.GuardianThrowCharge < 210f)
						{
							guardian.GuardianThrowCharge += 30f / HammerItem.Item.useTime * player.GetAttackSpeed(DamageClass.Melee);

							if (ChargeToAdd > 0)
							{
								ChargeToAdd--;
								guardian.GuardianThrowCharge += 2f * player.GetAttackSpeed(DamageClass.Melee);
							}

							if (guardian.GuardianThrowCharge > 210f) guardian.GuardianThrowCharge = 210f;
						}

						if (!player.controlUseItem && player.whoAmI == Main.myPlayer)
						{
							if (guardian.GuardianThrowCharge > 10f)
							{
								Projectile.ai[1] = 1;
								Projectile.friendly = true;

								Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center) * HammerItem.Item.shootSpeed;

								if (guardian.ThrowLevel() < 4)
								{
									dir *= (0.3f * (guardian.ThrowLevel() + 2) / 3);
									Projectile.damage = (int)(Projectile.damage * 0.75f);
									Projectile.knockBack = (int)(Projectile.knockBack / 3f);
									Projectile.ai[0] = 1f;
								}

								Projectile.velocity = dir;
								Projectile.rotation = dir.ToRotation();
								Projectile.direction = Projectile.spriteDirection;
								Projectile.netUpdate = true;

								SoundEngine.PlaySound(HammerItem.Item.UseSound, player.Center);
								guardian.GuardianThrowCharge = 0;
							}
							else
							{
								SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
								Projectile.ai[1] = -61f;
								Projectile.friendly = true;
								Projectile.netUpdate = true;
								hitTarget = false;
							}
						}
						else if (Main.mouseRight)
						{
							SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
							Projectile.ai[1] = -60f;
							Projectile.friendly = true;
							Projectile.netUpdate = true;
							hitTarget = false;
						}
					}
					else
					{
						if (Projectile.ai[1] < -60f) // Makes easier to sync the behaviour after a weak slam
						{
							Projectile.ai[1] = -60f;
							WeakHit = true;
							guardian.GuardianThrowCharge = 0;
						}

						if (ChargeToAdd > 0 && Projectile.ai[1] > -30 && ! WeakHit)
						{
							ChargeToAdd--;
							guardian.GuardianThrowCharge += 2f * player.GetAttackSpeed(DamageClass.Melee);
							if (guardian.GuardianThrowCharge > 210f) guardian.GuardianThrowCharge = 210f;
						}

						Projectile.velocity = Vector2.UnitX * 0.001f * player.direction; // So enemies are KBd in the right direction

						float SwingOffset = (float)Math.Sin(MathHelper.Pi / 60f * Projectile.ai[1]);
						Vector2 arm = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, MathHelper.Pi - (guardian.GuardianThrowCharge * 0.006f) * Projectile.spriteDirection);
						player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + (guardian.GuardianThrowCharge * 0.006f + SwingOffset * (3f + guardian.GuardianThrowCharge * 0.006f)) * Projectile.spriteDirection);
						Vector2 armPosition = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, MathHelper.Pi - (guardian.GuardianThrowCharge * 0.006f + SwingOffset * (3f + guardian.GuardianThrowCharge * 0.006f)) * Projectile.spriteDirection);
						Projectile.Center = armPosition - new Vector2((Projectile.width + 0.3f * guardian.GuardianThrowCharge + (float)Math.Sin(MathHelper.Pi / 210f * guardian.GuardianThrowCharge) * 10f) * player.direction * 0.4f + (armPosition.X - arm.X) * (2.5f + Projectile.width * 0.035f), (armPosition.Y - arm.Y) * -(1.1f + Projectile.width * 0.015f) + (210f - guardian.GuardianThrowCharge) * 0.075f);

						float toAdd = 30f / HammerItem.Item.useTime * player.GetAttackSpeed(DamageClass.Melee);
						if (Projectile.ai[1] < -40) Projectile.ai[1] += toAdd * 1.5f;
						else
						{
							Projectile.ai[1] += toAdd * 0.66f;
							Projectile.friendly = false;
						}

						if (Projectile.ai[1] >= 0f)
						{
							Projectile.ai[1] = 0f;
							Projectile.friendly = false;
							hitTarget = false;
						}
					}
				}
				else // Thrown
				{
					if (HammerItem.ThrowAI(player, guardian, Projectile, WeakThrow()))
					{
						if (Projectile.timeLeft < 598 && HammerItem.tileCollide && range > 0) // Delay helps preventing the hammer from instantly despawning if launched from inside a tile
						{ // Hammer has a smaller hitbox for tilecollide stuff
							Vector2 collideVelocity = Collision.TileCollision(Projectile.Center - Vector2.One * 10f, Projectile.velocity, 20, 20, true, true, (int)player.gravDir);
							if (collideVelocity != Projectile.velocity)
							{
								OnTileCollide(Projectile.velocity);
							}
						}

						if (WeakThrow())
							Projectile.rotation += 0.25f * dir;
						else
							Projectile.rotation += Projectile.velocity.Length() / 30f * (Projectile.velocity.X > 0 ? 1f : -1f) * 1.2f;

						OldPosition.Add(new Vector2(Projectile.Center.X, Projectile.Center.Y));
						OldRotation.Add(0f + Projectile.rotation);
						if (OldPosition.Count > 5)
							OldPosition.RemoveAt(0);
						if (OldRotation.Count > 5)
							OldRotation.RemoveAt(0);

						range--;

						if (range < 0)
						{
							float dist = Projectile.Center.Distance(player.Center);
							Vector2 vel = player.Center - Projectile.Center;
							vel.Normalize();

							if (range < -40)
							{
								float mult = 10f;
								if (Projectile.timeLeft < 500) mult += (500 - Projectile.timeLeft) / 40f;
								vel *= mult;
								Projectile.velocity = vel;
							}
							else
							{
								vel *= 0.5f;
								Projectile.velocity += vel;
							}

							if (dist < 30f)
							{
								Projectile.Kill();
							}

							if (range < -100)
							{
								Projectile.friendly = false;
							}
						}
					}
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (Projectile.ai[1] < 0) // Less  damage for melee hits
			{
				modifiers.FinalDamage *= 0.75f;
			}
		}

		public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
		{
			if (Projectile.ai[1] < 0) // Less  damage for melee hits
			{
				modifiers.FinalDamage *= 0.75f;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			range = -40;
			return false;
		}

		public override bool OrchidPreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (HammerTexture == null) return false;

			var color = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), Color.White);
			var position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			var effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			float rotationBonus = 0f;
			if (Projectile.ai[1] == 0)
			{
				Player player = Main.player[Projectile.owner];
				OrchidGuardian guardian = player.GetModPlayer<OrchidGuardian>();
				rotationBonus += guardian.GuardianThrowCharge * 0.0065f * Projectile.spriteDirection;
			}

			if (Projectile.ai[1] < 0)
			{
				Player player = Main.player[Projectile.owner];
				OrchidGuardian guardian = player.GetModPlayer<OrchidGuardian>();
				float SwingOffset = (float)Math.Sin(MathHelper.Pi / 60f * Projectile.ai[1]);
				rotationBonus += (guardian.GuardianThrowCharge * 0.0065f + SwingOffset * (3.5f + guardian.GuardianThrowCharge * 0.006f)) * Projectile.spriteDirection;
			}

			spriteBatch.Draw(HammerTexture, position, null, color, Projectile.rotation + rotationBonus, HammerTexture.Size() * 0.5f, Projectile.scale, effect, 0f);


			if (Projectile.ai[1] != 0)
			{
				for (int i = 0; i < OldPosition.Count; i++)
				{
					color = Lighting.GetColor((int)(OldPosition[i].X / 16f), (int)(OldPosition[i].Y / 16f), Color.White) * (((WeakThrow() ? 0.05f : 0.15f) * i));
					position = OldPosition[i] - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
					spriteBatch.Draw(HammerTexture, position, null, color, OldRotation[i], HammerTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
				}
			}

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			OrchidGuardian guardian = player.GetModPlayer<OrchidGuardian>();
			if (Projectile.ai[1] > 0)
			{ // Throw
				bool weak = WeakThrow();
				if (!hitTarget)
				{
					if (!weak)
					{
						guardian.AddSlam(HammerItem.slamStacks);
						guardian.AddBlock(HammerItem.blockStacks);
					}
					hitTarget = true;
					HammerItem.OnThrowHitFirst(player, guardian, target, Projectile, hit.Knockback, hit.Crit, weak);
				}
				HammerItem.OnThrowHit(player, guardian, target, Projectile, hit.Knockback, hit.Crit, weak);

				if (!penetrate)
				{
					range = -40;
				}
			}
			else
			{ // Melee Swing
				if (!hitTarget)
				{
					hitTarget = true;
					HammerItem.OnMeleeHitFirst(player, guardian, target, Projectile, hit.Knockback, hit.Crit);
					ChargeToAdd += 30;
				}
				HammerItem.OnMeleeHit(player, guardian, target, Projectile, hit.Knockback, hit.Crit);
			}
		}
	}
}