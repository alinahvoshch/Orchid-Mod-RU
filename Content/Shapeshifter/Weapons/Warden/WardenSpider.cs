using Microsoft.Xna.Framework;
using OrchidMod.Common.ModObjects;
using OrchidMod.Content.Shapeshifter.Buffs;
using OrchidMod.Content.Shapeshifter.Projectiles.Warden;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Shapeshifter.Weapons.Warden
{
	public class WardenSpider : OrchidModShapeshifterShapeshift
	{
		public bool LateralMovement = false;
		public bool JumpRelease = false;

		public override void SafeSetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.NPCHit29;
			Item.useTime = 30;
			Item.shootSpeed = 48f;
			Item.knockBack = 5f;
			Item.damage = 26;
			ShapeshiftWidth = 24;
			ShapeshiftHeight = 24;
			ShapeshiftType = ShapeshifterShapeshiftType.Warden;
			MeleeSpeedLeft = true;
		}

		public override void ShapeshiftAnchorOnShapeshift(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			anchor.Frame = 0;
			anchor.Timespent = 0;
			projectile.direction = player.direction;
			projectile.spriteDirection = player.direction;
			LateralMovement = false;

			for (int i = 0; i < 8; i++)
			{
				Main.dust[Dust.NewDust(projectile.Center, 0, 0, DustID.Smoke)].velocity *= 0.5f;
			}
		}

		public override void OnKillAnchor(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			for (int i = 0; i < 5; i++)
			{
				Main.dust[Dust.NewDust(projectile.Center, 0, 0, DustID.Smoke)].velocity *= 0.5f;
			}
		}


		public override void ShapeshiftOnLeftClick(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			Vector2 position = projectile.Center;
			Vector2 offSet = Vector2.Normalize(Main.MouseWorld - projectile.Center).RotatedByRandom(MathHelper.ToRadians(5f)) * Item.shootSpeed * Main.rand.NextFloat(0.8f, 1.2f) / 15f;

			for (int i = 0; i < 15; i++)
			{
				position += Collision.TileCollision(position, offSet, 2, 2, true, false, (int)player.gravDir);

				foreach (NPC npc in Main.npc)
				{
					if (OrchidModProjectile.IsValidTarget(npc))
					{
						if (position.Distance(npc.Center) < npc.width + 32f) // if the NPC is close to the projectile path, snaps to it.
						{
							position = npc.Center;
							break;
						}
					}
				}
			}

			int projectileType = ModContent.ProjectileType<WardenSpiderProj>();
			int damage = shapeshifter.GetShapeshifterDamage(Item.damage);
			Projectile newProjectile = Projectile.NewProjectileDirect(Item.GetSource_FromAI(), position, offSet * 0.001f, projectileType, damage, Item.knockBack, player.whoAmI);
			newProjectile.CritChance = shapeshifter.GetShapeshifterCrit(Item.crit);
			SoundEngine.PlaySound(SoundID.Zombie33, projectile.Center);

			anchor.LeftCLickCooldown = Item.useTime;
			anchor.Projectile.ai[0] = 10;
			anchor.Projectile.ai[1] = (Main.MouseWorld.X < projectile.Center.X ? -1f : 1f);
			anchor.NeedNetUpdate = true;

			anchor.Frame = 7;
		}

		public override void ShapeshiftOnRightClick(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			anchor.NeedNetUpdate = true;
			anchor.RightCLickCooldown = 360;
			//anchor.Projectile.ai[0] = -300;
			anchor.Projectile.ai[1] = projectile.spriteDirection;
			projectile.velocity.X = 0f;
			SoundEngine.PlaySound(SoundID.NPCHit24, projectile.Center);
		}

		public override void ShapeshiftAnchorAI(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			// MISC EFFECTS & ANIMATION

			float speedMult = GetSpeedMult(player, shapeshifter);
			bool walled = Framing.GetTileSafely((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f)).WallType != 0 && !anchor.IsInputJump;

			if (anchor.Projectile.ai[0] != 0)
			{ // Override animation during left and right click attack
				projectile.direction = (int)anchor.Projectile.ai[1];
				projectile.spriteDirection = projectile.direction;

				if (anchor.Projectile.ai[0] > 0)
				{ // Left Click
					anchor.Projectile.ai[0]--;
					anchor.Frame = walled ? 10 : anchor.Projectile.ai[0] < 6 ? 5 : 4;

					if (anchor.Projectile.ai[0] < 0)
					{
						anchor.Projectile.ai[0] = 0;
					}
				}

				if (anchor.Projectile.ai[0] == 0)
				{ // Puts the animation back on track
					anchor.Frame = walled ? 6 : 0;
				}
			}
			else if (walled)
			{ // Attached to a wall
				if (anchor.Frame < 6)
				{
					anchor.Frame = 6;
				}

				if (projectile.velocity.Length() > 1f)
				{ // is moving
					if (anchor.Timespent % 4 == 0 && anchor.Timespent > 0)
					{
						anchor.Frame++;
						if (anchor.Frame > 9)
						{
							anchor.Frame = 6;
						}
					}
				}
				else
				{
					anchor.Timespent = 0;
					anchor.Frame = 6;
				}
			}
			else
			{ // Grounded animations
				if (anchor.Frame > 3)
				{
					anchor.Frame = 0;
				}

				if (LateralMovement)
				{ // Player is moving left or right, cycle through frames
					if (anchor.Timespent % 4 == 0 && anchor.Timespent > 0)
					{
						anchor.Frame++;
						if (anchor.Frame == 4)
						{
							anchor.Frame = 1;
						}
					}
				}
				else
				{
					anchor.Timespent = 0;
					anchor.Frame = 0;
				}

				if (!IsGrounded(projectile, player, 8f))
				{
					anchor.Timespent = 0;
					anchor.Frame = 5;
				}
			}

			// MOVEMENT

			Vector2 intendedVelocity = projectile.velocity;
			bool horizontalMovement = false;

			if (walled)
			{ // Can walk on walls
				if ((anchor.IsInputUp || anchor.IsInputDown))
				{ // Player is inputting a movement key and didn't just start blocking
					if (anchor.IsInputUp && !anchor.IsInputDown)
					{ // Up movement
						TryAccelerate(ref intendedVelocity, LateralMovement ? -2.83f : -4f, speedMult, LateralMovement ? 0.283f : 0.4f, Yaxis: true);
						horizontalMovement = true;
					}
					else if (!anchor.IsInputUp && anchor.IsInputDown)
					{ // Down movement
						TryAccelerate(ref intendedVelocity, LateralMovement ? 2.83f : 4f, speedMult, LateralMovement ? 0.283f : 0.4f, Yaxis: true);
						horizontalMovement = true;
					}
					else
					{ // Both keys pressed = no movement
						intendedVelocity.Y *= 0.7f;
					}
				}
				else
				{ // no movement input
					intendedVelocity.Y *= 0.7f;
				}
			}
			else
			{ // Falling

				if (anchor.IsInputJump)
				{ // Jump while no charge ready
					if (IsGrounded(projectile, player, 4f) && JumpRelease)
					{
						JumpRelease = false;
						intendedVelocity.Y = -9.5f;
					}
				}
				else
				{
					JumpRelease = true;
				}

				GravityCalculations(ref intendedVelocity, player);
			}

			// Normal movement
			if ((anchor.IsInputLeft || anchor.IsInputRight))
			{ // Player is inputting a movement key and didn't just start blocking
				if (anchor.IsInputLeft && !anchor.IsInputRight)
				{ // Left movement
					TryAccelerate(ref intendedVelocity, horizontalMovement ? -2.83f : -4f, speedMult, horizontalMovement ? 0.283f : 0.4f);
					projectile.direction = -1;
					LateralMovement = true;
				}
				else if (anchor.IsInputRight && !anchor.IsInputLeft)
				{ // Right movement
					TryAccelerate(ref intendedVelocity, horizontalMovement ? 2.83f : 4f, speedMult, horizontalMovement ? 0.283f : 0.4f);
					projectile.direction = 1;
					LateralMovement = true;
				}
				else
				{ // Both keys pressed = no movement
					LateralMovement = false;
					intendedVelocity.X *= 0.7f;
				}
			}
			else
			{ // no movement input
				LateralMovement = false;
				intendedVelocity.X *= 0.7f;
			}

			if (walled)
			{
				projectile.rotation = projectile.velocity.ToRotation();
				projectile.spriteDirection = 1;
			}
			else
			{
				projectile.rotation = 0f;
				projectile.spriteDirection = projectile.direction;
			}

			FinalVelocityCalculations(ref intendedVelocity, projectile, player, true);

			// POSITION AND ROTATION VISUALS

			anchor.OldPosition.Add(projectile.Center);
			anchor.OldRotation.Add(projectile.rotation);
			anchor.OldFrame.Add(anchor.Frame);

			for (int i = 0; i < 2; i++)
			{
				if (anchor.OldPosition.Count > 4)
				{
					anchor.OldPosition.RemoveAt(0);
					anchor.OldRotation.RemoveAt(0);
					anchor.OldFrame.RemoveAt(0);
				}
			}
		}
	}
}