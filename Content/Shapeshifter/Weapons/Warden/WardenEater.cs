using Microsoft.Xna.Framework;
using OrchidMod.Content.Shapeshifter.Misc;
using OrchidMod.Content.Shapeshifter.Projectiles.Warden;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Shapeshifter.Weapons.Warden
{
	public class WardenEater : OrchidModShapeshifterShapeshift
	{
		private static List<int> VegetalTileTypes;
		private bool BackwardsAnimation = false;
		private bool MaxRangeSoundCue = false;

		public override void SetStaticDefaults()
		{
			VegetalTileTypes = GetVegetalTilesTypes();
		}

		public override void SafeSetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(0, 0, 74, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Grass;
			Item.useTime = 30;
			Item.shootSpeed = 10f;
			Item.knockBack = 15f;
			Item.damage = 50;
			ShapeshiftWidth = 22;
			ShapeshiftHeight = 22;
			ShapeshiftType = ShapeshifterShapeshiftType.Warden;
			Grounded = false;
			MeleeSpeedLeft = false;
		}

		public override bool ShapeshiftCanRightClick(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{ // can only create more fruits if >= 3 unripe fruits exist from that player
			int count = 0;
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.type == ModContent.ProjectileType<WardenEaterProjAlt>() && projectile.owner == proj.owner && proj.active && proj.frame == 0)
				{
					count++; 
				}
			}

			if (count >= 3) return false;
			return base.ShapeshiftCanRightClick(projectile, anchor, player, shapeshifter);
		}

		public override void ShapeshiftOnRightClick(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{ // creates a fruit
			int projectileType = ModContent.ProjectileType<WardenEaterProjAlt>();
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - projectile.Center).RotatedByRandom(MathHelper.ToRadians(7.5f)) * Item.shootSpeed * 0.25f;
			int damage = shapeshifter.GetShapeshifterDamage(Item.damage);
			Projectile newProjectile = Projectile.NewProjectileDirect(Item.GetSource_FromAI(), projectile.Center, velocity, projectileType, damage, Item.knockBack * 0.33f, player.whoAmI);
			newProjectile.CritChance = shapeshifter.GetShapeshifterCrit(Item.crit);
			newProjectile.netUpdate = true;

			anchor.ai[1] = 20f;
			projectile.ai[2] = velocity.ToRotation() - MathHelper.PiOver2;

			anchor.LeftCLickCooldown = Item.useTime;
		}

		public override bool ShapeshiftCanJump(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter) => anchor.IsInputJump && anchor.CanLeftClick;

		public override void ShapeshiftOnJump(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{ // dash
			Vector2 offSet = Vector2.Normalize(Main.MouseWorld - projectile.Center);
			projectile.ai[2] = offSet.ToRotation() - MathHelper.PiOver2;
			projectile.velocity *= 0f;
			anchor.ai[0] = 30;
			anchor.LeftCLickCooldown = Item.useTime * 2f;
			anchor.NeedNetUpdate = true;

			projectile.knockBack = 0f;
			projectile.damage = shapeshifter.GetShapeshifterDamage(Item.damage);
			projectile.CritChance = shapeshifter.GetShapeshifterCrit(Item.crit);
			projectile.friendly = true;

			SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, projectile.Center);
		}

		public override void ShapeshiftAnchorOnShapeshift(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			anchor.Frame = 0;
			anchor.Timespent = 0;
			projectile.direction = 1;
			projectile.spriteDirection = 1;
			BackwardsAnimation = false;
			MaxRangeSoundCue = false;

			if (IsLocalPlayer(player))
			{
				int projectileType = ModContent.ProjectileType<WardenEaterStem>();
				foreach (Projectile proj in Main.projectile)
				{ // Kills existing stems, redundant but fixes a case where there could be two
					if (proj.type == projectileType && proj.owner == player.whoAmI)
					{
						proj.Kill();
						break;
					}
				}

				Point playerCenterTile = new Point((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f));
				Vector2 validTileCoordinates = Vector2.Zero;
				Vector2 validTileCoordinatesVegetal = Vector2.Zero;

				// Check all the tiles in a square radius around the player center for a suitable one
				for (int i = -10; i <= 10; i++)
				{
					for (int j = -10; j <= 10; j++)
					{
						Tile tile = Framing.GetTileSafely(playerCenterTile.X + i, playerCenterTile.Y + j);
						if (tile.HasTile && (WorldGen.SolidTile(tile) || TileID.Sets.Platforms[tile.TileType]))
						{
							Vector2 tileCoordinates = new Vector2((playerCenterTile.X + i) * 16f + 8f, (playerCenterTile.Y + j) * 16f - 8f);

							if (validTileCoordinatesVegetal == Vector2.Zero)
							{ // no "vegetal" tile detected yet
								if (projectile.Center.Distance(tileCoordinates) < projectile.Center.Distance(validTileCoordinates))
								{ // replaces the target tile with the closest one
									validTileCoordinates = tileCoordinates;

									if (TileID.Sets.Platforms[tile.TileType])
									{ // slight offset when hooked to a platform
										validTileCoordinates.Y -= 4f; 
									}
								}
							}
							
							if (VegetalTileTypes.Contains(tile.TileType) && projectile.Center.Distance(tileCoordinates) < projectile.Center.Distance(validTileCoordinatesVegetal)) {
								validTileCoordinatesVegetal = tileCoordinates;
							} 
						}
					}
				}

				if (validTileCoordinates != Vector2.Zero)
				{
					Vector2 latchPosition = validTileCoordinates;
					if (validTileCoordinatesVegetal != Vector2.Zero)
					{
						latchPosition = validTileCoordinatesVegetal;
						projectile.ai[1] = 1f; // is hooked to plant
					}

					latchPosition.Y += 16f; // idk why it's offset by 1 tile upwards

					float maxRange = 240f * GetSpeedMult(player, shapeshifter, anchor); // 15 tiles * movespeed
					Projectile newProjectile = Projectile.NewProjectileDirect(Item.GetSource_FromAI(), latchPosition, Vector2.Zero, projectileType, 0, 0f, player.whoAmI, Main.rand.Next(1000), maxRange + 8f);
					projectile.ai[0] = maxRange;
					anchor.NeedNetUpdate = true;
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Item16, projectile.Center);
				}
			}

			for (int i = 0; i < 8; i++)
			{
				Main.dust[Dust.NewDust(projectile.Center, 0, 0, DustID.Smoke)].velocity *= 0.5f;
			}

			for (int i = 0; i < 10; i++)
			{
				Main.dust[Dust.NewDust(projectile.Center, 0, 0, DustID.JungleGrass)].velocity.Y -= 1.25f;
			}

			for (int i = 0; i < 5; i++)
			{
				Gore.NewGoreDirect(player.GetSource_ItemUse(Item), projectile.Center + new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), Vector2.UnitY.RotatedByRandom(MathHelper.Pi), GoreID.TreeLeaf_Jungle);
			}
		}

		public override void OnKillAnchor(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			for (int i = 0; i < 5; i++)
			{
				Main.dust[Dust.NewDust(projectile.Center, 0, 0, DustID.Smoke)].velocity *= 0.5f;
			}

			for (int i = 0; i < 10; i++)
			{
				Main.dust[Dust.NewDust(projectile.Center, 0, 0, DustID.JungleGrass)].velocity.Y -= 1.25f;
			}
		}

		public override void ShapeshiftAnchorAI(Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			// ai[0] contains the max distance to stem
			// ai[1] is == 1f if hooked to a vegetal tile, 0f else
			// ai[2] holds the angle of head during the various attacks
			// anchor.ai[0] holds the duration of the "jump" dash
			// anchor.ai[1] holds the animation duration of the right click attack

			anchor.ai[1]--;

			// MISC EFFECTS & ANIMATION

			if (anchor.ai[0] > 0f)
			{ // shoot animation
				anchor.Frame = 4;
			}
			else if (anchor.ai[1] > 0f)
			{ // "jump" dash animation
				anchor.Frame = 4;
			}
			else if (anchor.Timespent % 5 == 0)
			{ // Animation frames
				if (anchor.Frame > 2)
				{ // resets the animation after a dash or attack
					anchor.Frame = 2;
					BackwardsAnimation = true;
				}

				if (BackwardsAnimation)
				{ // normal animation loop
					anchor.Frame--;

					if (anchor.Frame <= 0)
					{
						BackwardsAnimation = false;
					}
				}
				else
				{
					anchor.Frame++;

					if (anchor.Frame >= 2)
					{
						BackwardsAnimation = true;
					}
				}
			}

			if (projectile.ai[1] == 1f)
			{ // hooked to a vegetal tile
				shapeshifter.ShapeshifterMoveSpeedBonusFinal += 0.2f;
			}

			float speedMult = GetSpeedMult(player, shapeshifter, anchor);
			Projectile stem = null;

			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active && proj.type == ModContent.ProjectileType<WardenEaterStem>() && proj.owner == player.whoAmI)
				{
					stem = proj;

					float maxRange = 240f * GetSpeedMult(player, shapeshifter, anchor); // 15 tiles * movespeed
					if (projectile.ai[0] != maxRange)
					{ // Dynamically adapts the stem range to the player movespeed
						projectile.ai[0] = maxRange;
						stem.ai[1] = maxRange + 8f;
						anchor.NeedNetUpdate = true;
						stem.netUpdate = true;
					}

					break;
				}
			}

			if (stem == null)
			{ // unshift if there is a problem with the stem
				projectile.Kill();
				return;
			}
			else
			{ // else handle the projectile rotation
				if (anchor.ai[0] > 0f || anchor.ai[1] > 0f)
				{ // dash
					projectile.rotation = projectile.ai[2] + MathHelper.Pi;
				}
				else
				{ // normal rotation, relative to stem
					projectile.rotation = (stem.Center - projectile.Center).ToRotation() - MathHelper.PiOver2;
				}
			}

			// MOVEMENT

			Vector2 intendedVelocity = projectile.velocity;

			if (anchor.ai[0] > 0f)
			{ // jump dash
				float dashmult = 1f;

				if (anchor.ai[0] < 10f)
				{
					dashmult = anchor.ai[0] / 10f;
				}

				intendedVelocity = Vector2.UnitY.RotatedBy(projectile.ai[2]) * speedMult * 8f * dashmult;
				anchor.ai[0]--;

				int projType = ModContent.ProjectileType<WardenEaterProjAlt>();
				foreach(Projectile proj in Main.projectile)
				{
					if (projectile.Hitbox.Intersects(proj.Hitbox) && proj.type == projType && proj.frame == 1 && IsLocalPlayer(player) && proj.ai[0] == 0f)
					{
						proj.ai[0] = 1f;
						proj.netUpdate = true;
						shapeshifter.modPlayer.TryHeal(15);
						break;
					}
				}

				if (anchor.ai[0] <= 0f)
				{ // dash end
					anchor.ai[0] = 0f;
					projectile.friendly = false;
				}
			}
			else
			{
				// 8 direction movement
				float velocityX = 0f;
				float velocityY = 0f;

				if (anchor.IsInputUp && !anchor.IsInputDown)
				{ // Top movement
					velocityY = -4f;
				}
				else if (anchor.IsInputDown && !anchor.IsInputUp)
				{ // Bottom movement
					velocityY = 4f;
				}
				else
				{ // Both keys pressed or no key pressed = no Y movement
					intendedVelocity.Y *= 0.85f;
				}

				if (anchor.IsInputLeft && !anchor.IsInputRight)
				{ // Left movement
					velocityX = -4f;
				}
				else if (anchor.IsInputRight && !anchor.IsInputLeft)
				{ // Right movement
					velocityX = 4f;
				}
				else
				{ // Both keys pressed or no key pressed = no X movement
					intendedVelocity.X *= 0.85f;
				}

				if (velocityX != 0f && velocityY != 0f)
				{ // diagonal movement, multiply both velocities so the speed isn't faster diagonally
					velocityX *= 0.70725f; // approx
					velocityY *= 0.70725f;
				}

				if (velocityX != 0f)
				{
					TryAccelerate(ref intendedVelocity, velocityX, speedMult, 0.6f);
				}

				if (velocityY != 0f)
				{
					TryAccelerate(ref intendedVelocity, velocityY, speedMult, 0.6f, Yaxis: true);
				}
			}

			if (stem != null)
			{
				float distance = stem.Center.Distance(projectile.Center);
				if (distance > projectile.ai[0])
				{ // Keeps the player inside the max range
					Vector2 pullBack = Vector2.Normalize(stem.Center - projectile.Center) * (stem.Center.Distance(projectile.Center) - projectile.ai[0]);
					intendedVelocity += pullBack * 0.1f;

					if (distance - projectile.ai[0] < 4f)
					{
						if (!MaxRangeSoundCue)
						{
							SoundEngine.PlaySound(SoundID.Grass, projectile.Center);
							MaxRangeSoundCue = true;
						}
					}
					else
					{
						MaxRangeSoundCue = false;
					}
				}
				else
				{
					MaxRangeSoundCue = false;
				}
			}

			FinalVelocityCalculations(ref intendedVelocity, projectile, player, forceFallThrough:true);

			// POSITION AND ROTATION VISUALS

			anchor.OldPosition.Add(projectile.Center);
			anchor.OldRotation.Add(projectile.rotation);
			anchor.OldFrame.Add(anchor.Frame);

			for (int i = 0; i < 2; i++)
			{
				if (anchor.OldPosition.Count > (anchor.ai[0] > 0f ? 7 : 4))
				{
					anchor.OldPosition.RemoveAt(0);
					anchor.OldRotation.RemoveAt(0);
					anchor.OldFrame.RemoveAt(0);
				}
			}
		}

		public override void ShapeshiftOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone, Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			if (target.knockBackResist > 0f)
			{
				target.velocity = Vector2.Normalize(projectile.velocity) * 20f * target.knockBackResist;
				target.velocity.Y -= 3f;
				target.netUpdate = true;
			}
		}

		public override bool ShapeshiftFreeDodge(Player.HurtInfo info, Projectile projectile, ShapeshifterShapeshiftAnchor anchor, Player player, OrchidShapeshifter shapeshifter)
		{
			if (anchor.ai[0] > 0) return true;
			return base.ShapeshiftFreeDodge(info, projectile, anchor, player, shapeshifter);
		}

		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddIngredient<ShapeshifterBlankEffigy>();
			recipe.AddIngredient(ItemID.Vine, 5);
			recipe.AddIngredient(ItemID.JungleSpores, 15);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public List<int> GetVegetalTilesTypes()
		{
			List<int> VegetalTypes = new List<int>()
			{
				TileID.Grass,
				TileID.AshGrass,
				TileID.CorruptGrass,
				TileID.CorruptJungleGrass,
				TileID.CrimsonGrass,
				TileID.CrimsonJungleGrass,
				TileID.HallowedGrass,
				TileID.JungleGrass,
				TileID.MushroomGrass,
				TileID.GolfGrass,
				TileID.GolfGrassHallowed,
				TileID.ArgonMoss,
				TileID.ArgonMossBrick,
				TileID.BlueMoss,
				TileID.BlueMossBrick,
				TileID.BrownMoss,
				TileID.BrownMossBrick,
				TileID.GreenMoss,
				TileID.GreenMossBrick,
				TileID.KryptonMoss,
				TileID.KryptonMossBrick,
				TileID.LavaMoss,
				TileID.LavaMossBrick,
				TileID.PurpleMoss,
				TileID.PurpleMossBrick,
				TileID.RainbowMoss,
				TileID.RainbowMossBrick,
				TileID.RedMoss,
				TileID.RedMossBrick,
				TileID.VioletMoss,
				TileID.VioletMossBrick,
				TileID.XenonMoss,
				TileID.XenonMossBrick,
				TileID.LeafBlock,
				TileID.LivingMahoganyLeaves,
				TileID.LivingWood,
				TileID.LivingMahogany
			};

			/*
			if (OrchidMod.ThoriumMod != null)
			{
				VegetalTypes.Add();
			}
			*/

			return VegetalTypes;
		}
	}
}