using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using OrchidMod;
using OrchidMod.Dusts;
using OrchidMod.Shaman;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Shaman
{
	public class OrchidModShamanHelper
	{
		public static void shamanPostUpdateEquips(Player player, OrchidModPlayer modPlayer, Mod mod) {
			if (modPlayer.UIDisplayTimer == 0) {
				modPlayer.shamanFireBondLoading = 0;
				modPlayer.shamanWaterBondLoading = 0;
				modPlayer.shamanAirBondLoading = 0;
				modPlayer.shamanEarthBondLoading = 0;
				modPlayer.shamanSpiritBondLoading = 0;
			}
			
			if (modPlayer.shamanFireTimer > 0) {
				modPlayer.shamanFireTimer -= player.HasBuff(BuffType<Shaman.Buffs.SpiritBuffMana>()) ? 0 : 1;
				modPlayer.shamanPollFire += modPlayer.timer120 % 15 == 0 ? modPlayer.shamanTimerCombat > 0 ? 2 : 1 : 0;
				if (modPlayer.shamanPollFire > 0) {
					modPlayer.shamanPollFire --;
					modPlayer.shamanFireBondLoading += modPlayer.shamanFireBondLoading < 100 ? 1 : 0;
				}
				
				if (modPlayer.shamanSmite && modPlayer.timer120 % 60 == 0) {
					int dmg = (int)(100 * modPlayer.shamanDamage);
					
					int randX = Main.rand.Next(50);
					int randY = Main.rand.Next(50);
						
					for (int i = 0 ; i < 3 ; i ++) {
						int dust = Dust.NewDust(new Vector2((int)(player.Center.X + 25 - randX), (int)(player.Center.Y  + 15 - randY)), 0, 0, 162, (float)(1 - Main.rand.Next(2)), (float)(1 - Main.rand.Next(2)), 0, default(Color), 2f);
						Main.dust[dust].noGravity = true;
					}
					
					Projectile.NewProjectile((int)(player.Center.X + 25 - randX), (int)(player.Center.Y  + 15 - randY), 0f, 0f, mod.ProjectileType("Smite"), dmg, 0f, player.whoAmI);
				}
				
				//shamanDamage += (0.04f * shamanFireBuff) + (0.04f * shamanFireBonus);
				modPlayer.shamanTimerCombat -= modPlayer.shamanTimerCombat > 0 ? 1 : 0;
			} else {
				modPlayer.shamanFireBondLoading -= modPlayer.shamanFireBondLoading > 0 && modPlayer.timer120 % 10 == 0 ? 1 : 0;
				modPlayer.shamanPollFire = 0;
				modPlayer.shamanTimerCombat = 0;
			}
			
			if (modPlayer.shamanWaterTimer > 0) {
				modPlayer.shamanWaterTimer -= player.HasBuff(BuffType<Shaman.Buffs.SpiritBuffMana>()) ? 0 : 1;
				if (modPlayer.shamanPollWater > 0) {
					modPlayer.shamanPollWater --;
					modPlayer.shamanWaterBondLoading += modPlayer.shamanWaterBondLoading < 100 ? 1 : 0;
				}
				//player.endurance += (0.04f * shamanWaterBuff) + (0.04f * shamanWaterBonus);
				
				if (modPlayer.shamanHeavy) {
					player.statDefense += 10;
					player.moveSpeed -= 0.2f;
				}
				
			} else {
				modPlayer.shamanWaterBondLoading -= modPlayer.shamanWaterBondLoading > 0 && modPlayer.timer120 % 10 == 0 ? 1 : 0;
				modPlayer.shamanPollWater = 0;
			}
			
			if (modPlayer.shamanAirTimer > 0) {
				modPlayer.shamanAirTimer -= player.HasBuff(BuffType<Shaman.Buffs.SpiritBuffMana>()) ? 0 : 1;
				//shamanCrit += (4 * shamanAirBuff) + (4 * shamanAirBonus);
				float vel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
				modPlayer.shamanPollAir += modPlayer.timer120 % 20 == 0 && vel > 1f ? vel > 3.5f ? vel > 6f ? 3 : 2 : 1 : 0;
				
				if (modPlayer.shamanDripping) {
					if (modPlayer.timer120 % 10 == 0)
					{
						int dmg = (int)(30 * modPlayer.shamanDamage + 5E-06f);
						Projectile.NewProjectile(player.Center.X - 10 + (Main.rand.Next(20)), player.Center.Y + 16, 0f, -0.001f, mod.ProjectileType("LavaDroplet"), dmg, 0f, player.whoAmI);
					}	
				}
				
				if (modPlayer.shamanFeather) {
					if (!player.controlDown) player.gravity /= 3;
				}

				if (modPlayer.shamanHarpyAnklet) {
					modPlayer.doubleJumpHarpy = true;
				}
				
				if (modPlayer.shamanPollAir > 0) {
					modPlayer.shamanPollAir --;
					modPlayer.shamanAirBondLoading += modPlayer.shamanAirBondLoading < 100 ? 1 : 0;
				}
			} else {
				modPlayer.shamanAirBondLoading -= modPlayer.shamanAirBondLoading > 0 && modPlayer.timer120 % 10 == 0 ? 1 : 0;
				modPlayer.shamanPollAir = 0;
			}
			
			if (modPlayer.shamanEarthTimer > 0) {
				modPlayer.shamanEarthTimer -= player.HasBuff(BuffType<Shaman.Buffs.SpiritBuffMana>()) ? 0 : 1;
				//player.lifeRegen += (2 * shamanEarthBuff) + (2 * shamanEarthBonus);
				modPlayer.shamanTimerImmobile = player.velocity == player.velocity * 0f ? modPlayer.shamanTimerImmobile < 240 ? modPlayer.shamanTimerImmobile + 1 : modPlayer.shamanTimerImmobile : 0;
				
				if (modPlayer.timer120 % 8 == 0) {
					modPlayer.shamanPollEarth += modPlayer.shamanTimerImmobile >= 80 ? modPlayer.shamanTimerImmobile >= 160 ? modPlayer.shamanTimerImmobile >= 240 ? 3 : 2 : 1 : 0;
				}
				
				if (modPlayer.shamanPollEarth > 0) {
					modPlayer.shamanPollEarth --;
					modPlayer.shamanEarthBondLoading += modPlayer.shamanEarthBondLoading < 100 ? 1 : 0;
				}
				if (modPlayer.shamanHoney) {
					player.AddBuff((48), 1); // Honey
					if (modPlayer.shamanEarthTimer % 90 == 0) {
						
						int randX = Main.rand.Next(150);
						int randY = Main.rand.Next(100);
						
						for (int i = 0 ; i < 3 ; i ++) {
							int dust = Dust.NewDust(new Vector2((int)(player.Center.X + 75 - randX), (int)(player.Center.Y  + 15 - randY)), 0, 0, 152, 0f, 0f, 0, default(Color), 1.3f);
							Main.dust[dust].noGravity = true;
						}
						
						if (Main.player[Main.myPlayer].strongBees && Main.rand.Next(2) == 0) 
							Projectile.NewProjectile((int)(player.Center.X + 75 - randX), (int)(player.Center.Y  + 15 - randY), (float)(Main.rand.Next(3) - 1.5), (float)(Main.rand.Next(3) - 1.5), 566, (int)(12), 0f, player.whoAmI, 0f, 0f);
						else
							Projectile.NewProjectile((int)(player.Center.X + 75 - randX), (int)(player.Center.Y  + 15 - randY), (float)(Main.rand.Next(3) - 1.5), (float)(Main.rand.Next(3) - 1.5), 181, (int)(10), 0f, player.whoAmI, 0f, 0f);
					}
				}
				
				if (modPlayer.shamanForest) {
					player.AddBuff(mod.BuffType("DeepForestAura"), 2);
				}
				
				if (modPlayer.shamanAmber)
					player.statLifeMax2 += 50;
				
			} else {
				modPlayer.shamanEarthBondLoading -= modPlayer.shamanEarthBondLoading > 0 && modPlayer.timer120 % 10 == 0 ? 1 : 0;
				modPlayer.shamanPollEarth = 0;
				modPlayer.shamanTimerImmobile = 0;
			}
			
			if (modPlayer.shamanSpiritTimer > 0) {
				modPlayer.shamanSpiritTimer -= player.HasBuff(BuffType<Shaman.Buffs.SpiritBuffMana>()) ? 0 : 1;
				//player.moveSpeed += (0.05f * shamanSpiritBuff) + (0.05f * shamanSpiritBonus);
				if (modPlayer.shamanPollSpirit > 0) {
					modPlayer.shamanPollSpirit --;
					modPlayer.shamanSpiritBondLoading += modPlayer.shamanSpiritBondLoading < 100 ? 1 : 0;
				}
			} else {
				modPlayer.shamanSpiritBondLoading -= modPlayer.shamanSpiritBondLoading > 0 && modPlayer.timer120 % 10 == 0 ? 1 : 0;
				modPlayer.shamanPollSpirit = 0;
			}
			
			if (modPlayer.doubleJumpHarpy)  // Vanilla double jump code is insanely weird.
			{
				if (!player.controlJump) modPlayer.harpySpaceKeyReleased = true;
				
				if (!(player.doubleJumpCloud || player.doubleJumpSail || player.doubleJumpSandstorm 
				|| player.doubleJumpBlizzard || player.doubleJumpFart || player.doubleJumpUnicorn)) 
					player.doubleJumpCloud = true;
					
				if (player.velocity.Y == 0 || player.grappling[0] >= 0 || (modPlayer.shamanHarpyAnklet && modPlayer.shamanSpiritTimer > 0 && !player.controlJump)) 
				{
					if (player.jumpAgainCloud)
					{
					  modPlayer.jumpHeightCheck = (int) ((double) Player.jumpHeight * 0.75);
					}
					if (player.jumpAgainSail)
					{
					  modPlayer.jumpHeightCheck = (int) ((double) Player.jumpHeight * 1.25);
					}
					if (player.jumpAgainFart)
					{
					  modPlayer.jumpHeightCheck = Player.jumpHeight * 2;
					}
					if (player.jumpAgainBlizzard)
					{
					  modPlayer.jumpHeightCheck = (int) ((double) Player.jumpHeight * 1.5);
					}
					if (player.jumpAgainSandstorm)
					{
					  modPlayer.jumpHeightCheck = Player.jumpHeight * 3;
					}
					if (player.jumpAgainUnicorn)
					{
					  modPlayer.jumpHeightCheck = Player.jumpHeight * 2;
					}
				}
				
				if (player.jumpAgainCloud && player.jump == (int) ((double) Player.jumpHeight * 0.75))
					player.jump --;
				
				if ((player.jump == modPlayer.jumpHeightCheck && modPlayer.harpySpaceKeyReleased == true)) {
					modPlayer.harpySpaceKeyReleased = false;
					int dmg = 10;
					if (modPlayer.shamanHarpyAnklet && modPlayer.shamanSpiritTimer > 0) {
						dmg = (int)(12 * modPlayer.shamanDamage);
						if (player.FindBuffIndex(mod.BuffType("HarpyAgility")) > -1)
							dmg += (int)(12 * modPlayer.shamanDamage); 
					}
								
					for (float dirX = -1 ; dirX < 2; dirX ++) {
						for (float dirY = -1 ; dirY < 2; dirY ++) {
							bool ankletCanShoot = !(dirX == 0 && dirY == 0 && dirX == dirY);
							float ankletSpeed = 10f;
							if (dirX != 0 && dirY != 0) ankletSpeed = 7.5f;
							if (ankletCanShoot) {
								Projectile.NewProjectile(player.Center.X, player.Center.Y, (dirX*ankletSpeed), (dirY*ankletSpeed), mod.ProjectileType("HarpyAnkletProj"), dmg, 0.0f, player.whoAmI, 0.0f, 0.0f);
							}
						}
					}
				}
			}
			
			if (modPlayer.shamanFireTimer <= 0) modPlayer.shamanFireBuff = 0;
			else modPlayer.UIDisplayTimer = modPlayer.UIDisplayDelay;
			if (modPlayer.shamanWaterTimer <= 0) modPlayer.shamanWaterBuff = 0;
			else modPlayer.UIDisplayTimer = modPlayer.UIDisplayDelay;
			if (modPlayer.shamanAirTimer <= 0) modPlayer.shamanAirBuff = 0;
			else modPlayer.UIDisplayTimer = modPlayer.UIDisplayDelay;
			if (modPlayer.shamanEarthTimer <= 0) modPlayer.shamanEarthBuff = 0;
			else modPlayer.UIDisplayTimer = modPlayer.UIDisplayDelay;
			if (modPlayer.shamanSpiritTimer <= 0) modPlayer.shamanSpiritBuff = 0;
			else modPlayer.UIDisplayTimer = modPlayer.UIDisplayDelay;
			
			if (modPlayer.shamanFireBuff + modPlayer.shamanWaterBuff + modPlayer.shamanAirBuff + modPlayer.shamanEarthBuff + modPlayer.shamanSpiritBuff == 0 && modPlayer.UIDisplayTimer > 0) {
				modPlayer.UIDisplayTimer --;
			}
			
				
			modPlayer.OchidScreenH = Main.screenHeight;
			modPlayer.OchidScreenW = Main.screenWidth;
			if (modPlayer.OchidScreenHCompare != modPlayer.OchidScreenH || modPlayer.OchidScreenWCompare != modPlayer.OchidScreenW) {
				OrchidMod.reloadShamanUI = true;
				modPlayer.OchidScreenHCompare = modPlayer.OchidScreenH;
				modPlayer.OchidScreenWCompare = modPlayer.OchidScreenW;
			}
			
			if(modPlayer.doubleTap > 0) modPlayer.doubleTap --;
			else modPlayer.doubleTapLock = false;
			if(modPlayer.doubleTapCooldown > 0) modPlayer.doubleTapCooldown--;
			
			if (!Main.ReversedUpDownArmorSetBonuses) {
				if (player.controlDown && modPlayer.doubleTap == 0 && modPlayer.doubleTapCooldown == 0) {
					modPlayer.doubleTap += 30;
					modPlayer.doubleTapCooldown = 60;
					modPlayer.doubleTapLock = true;
				}
				
				if (!player.controlDown && modPlayer.doubleTap > 0 && modPlayer.doubleTapLock) {
					modPlayer.doubleTapLock = false;
				}
				
				if (player.controlDown && modPlayer.doubleTap > 0 && !modPlayer.doubleTapLock) {
					if (modPlayer.abyssSet) {
						Projectile.NewProjectile((Main.mouseX + Main.screenPosition.X), (Main.mouseY + Main.screenPosition.Y), 0f, 0f, mod.ProjectileType("AbyssPortal"), 0, 5, player.whoAmI);
						Main.PlaySound(SoundID.Item122, player.Center);
					}
					modPlayer.doubleTap = 0;
					modPlayer.doubleTapCooldown += 1000;
				}
			}
			else {
					if (player.controlUp && modPlayer.doubleTap == 0 && modPlayer.doubleTapCooldown == 0) {
					modPlayer.doubleTap = 30;
					modPlayer.doubleTapLock = true;
				}
				
				if (!player.controlUp && modPlayer.doubleTap > 0 && modPlayer.doubleTapLock) {
					modPlayer.doubleTapLock = false;
				}
				
				if (player.controlUp && modPlayer.doubleTap > 0 && !modPlayer.doubleTapLock) {
					if (modPlayer.abyssSet) {
						Projectile.NewProjectile((Main.mouseX + Main.screenPosition.X), (Main.mouseY + Main.screenPosition.Y), 0f, 0f, mod.ProjectileType("AbyssPortal"), 0, 5, player.whoAmI);
						Main.PlaySound(SoundID.Item122, player.Center);
					}
					modPlayer.doubleTap = 0;
					modPlayer.doubleTapCooldown += 1000;
				}
			}
			
			modPlayer.timer120 ++;
			if (modPlayer.timer120 == 120) {
				modPlayer.timer120 = 0;	
			}
		}
		
		public static void postUpdateShaman(Player player, OrchidModPlayer modPlayer, Mod mod) {
			if (modPlayer.shamanFireTimer <= 0
				&& modPlayer.shamanWaterTimer <= 0
				&& modPlayer.shamanAirTimer <= 0
				&& modPlayer.shamanEarthTimer <= 0
				&& modPlayer.shamanSpiritTimer <= 0) 
			{

				modPlayer.shamanOrbBig = ShamanOrbBig.NULL;
				modPlayer.shamanOrbSmall = ShamanOrbSmall.NULL;
				modPlayer.shamanOrbLarge = ShamanOrbLarge.NULL;
				modPlayer.shamanOrbUnique = ShamanOrbUnique.NULL;
				
				modPlayer.orbCountSmall = 0;
				modPlayer.orbCountBig = 0;
				modPlayer.orbCountLarge = 0;
				modPlayer.orbCountUnique = 0;
				modPlayer.orbCountCircle = 0;
			}
			
			if (!(player.FindBuffIndex(mod.BuffType("SpiritualBurst")) > -1) && modPlayer.orbCountCircle > 39 && modPlayer.shamanOrbCircle == ShamanOrbCircle.REVIVER)
					modPlayer.orbCountCircle = 0;

			if (modPlayer.orbCountBig < 0) 
				modPlayer.orbCountBig = 0;
			
			if (modPlayer.shamanTimerCrimson < 30) {
				modPlayer.shamanTimerCrimson ++;
			}

			if (modPlayer.shamanTimerViscount < 180) {
				modPlayer.shamanTimerViscount ++;
			}
			
			if (modPlayer.shamanTimerHellDamage < 600) {
				modPlayer.shamanTimerHellDamage ++;
			}
			
			if (modPlayer.shamanTimerHellDefense < 300) {
				modPlayer.shamanTimerHellDefense ++;
			}
			
			if (modPlayer.timerVial < 30) {
				modPlayer.timerVial ++;
			}
		}
		
		public static void OnHitNPCWithProjShaman(Projectile projectile, NPC target, int damage, float knockback, bool crit, Player player, OrchidModPlayer modPlayer, Mod mod) {
			modPlayer.shamanTimerCombat = 120;
				
			if (modPlayer.shamanCrimtane && modPlayer.shamanEarthBuff != 0 && modPlayer.shamanTimerCrimson == 30) {
				modPlayer.shamanTimerCrimson = 0;
				if (Main.myPlayer == player.whoAmI)
					player.HealEffect(2, true);
				player.statLife += 2;	
			}
				
			if (modPlayer.shamanVampire && Main.rand.Next(5) == 0 && modPlayer.shamanTimerViscount == 180) {
				modPlayer.shamanTimerViscount = 0;
				Vector2 perturbedSpeed = new Vector2(0f, - 5f).RotatedByRandom(MathHelper.ToRadians(40));
				if (Main.rand.Next(2) == 0) {
					int type = ProjectileType<Shaman.Projectiles.Thorium.Equipment.Viscount.ViscountBlood>();
					Projectile.NewProjectile(target.Center.X, target.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, type, 0, 0.0f, projectile.owner, 0.0f, 0.0f);
				} else {
					int type = ProjectileType<Shaman.Projectiles.Thorium.Equipment.Viscount.ViscountSound>();
					Projectile.NewProjectile(target.Center.X, target.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, type, 0, 0.0f, projectile.owner, 0.0f, 0.0f);
				}
			}
				
			if (modPlayer.shamanHell && modPlayer.shamanTimerHellDamage == 600 && modPlayer.shamanFireTimer > 0) {
				modPlayer.shamanTimerHellDamage = 0;
				for (int i = 0 ; i < 10 ; i ++) {
					Vector2 perturbedSpeed = new Vector2(0f, - 5f).RotatedByRandom(MathHelper.ToRadians(360));
					int dmg = (int)(50 * modPlayer.shamanDamage);
					int type = ProjectileType<Shaman.Projectiles.Equipment.Hell.ShamanHellHoming>();
					Projectile.NewProjectile(target.Center.X, target.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, type, dmg, 0.0f, projectile.owner, 0.0f, 0.0f);
				}
			}
		}
		
		public static void ModifyHitNPCWithProjShaman(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection, Player player, OrchidModPlayer modPlayer, Mod mod) {
			if (crit) {
				if (player.FindBuffIndex(mod.BuffType("OpalEmpowerment")) > -1) {
					damage += 5;
				}
					
				if (player.FindBuffIndex(mod.BuffType("DestroyerFrenzy")) > -1) {
					damage = (int)(damage * 1.15f);
				}
					
				if (projectile.type == mod.ProjectileType("TitanicScepterProj")) {
					damage = (int)(damage * 1.5f);
				}
					
				if (player.FindBuffIndex(mod.BuffType("TitanicBuff")) > -1) {
					damage = (int)(damage * 1.2f);
				}
					
				if (modPlayer.shamanDestroyer && modPlayer.shamanWaterTimer > 0) {
					modPlayer.shamanTimerDestroyer = 60;
					modPlayer.shamanDestroyerCount ++;
				}
			}
			
			if (target.type != NPCID.TargetDummy) {
				if (projectile.type != mod.ProjectileType("LavaDroplet")
					&& projectile.type != mod.ProjectileType("LostSoul")
					&& projectile.type != mod.ProjectileType("HarpyAnkletProj")
					&& projectile.type != mod.ProjectileType("DeepForestCharmProj")
					&& projectile.type != mod.ProjectileType("Smite")
					){
					if (modPlayer.shamanFireBuff > 0) {
						if (modPlayer.shamanPoison) target.AddBuff((20), 5 * 60);
						if (modPlayer.shamanVenom) target.AddBuff((70), 5 * 60);
						if (modPlayer.shamanFire) target.AddBuff((24), 5 * 60);
						if (modPlayer.shamanIce) target.AddBuff((44), 5 * 60);
						if (modPlayer.shamanDemonite) target.AddBuff(153, 20); // Shadowflame
					}
						
					if (crit == true && modPlayer.shamanSkull && modPlayer.shamanWaterBuff != 0) {	
						int dmg = (int)(80 * modPlayer.shamanDamage + 5E-06f);
						Vector2 mouseTarget = Main.screenPosition + new Vector2((float)Main.mouseX - 8, (float)Main.mouseY);
						Vector2 heading = mouseTarget - Main.player[projectile.owner].position;
						heading.Normalize();
						heading *= new Vector2(5f, 5f).Length();
						Vector2 projectileVelocity = ( new Vector2(heading.X, heading.Y).RotatedByRandom(MathHelper.ToRadians(10)));
						Projectile.NewProjectile(Main.player[projectile.owner].Center.X, Main.player[projectile.owner].Center.Y, projectileVelocity.X, projectileVelocity.Y, mod.ProjectileType("LostSoul"), dmg, 0f, projectile.owner, 0f, 0f);
					}
						
					if (crit == true && modPlayer.shamanWaterHoney && modPlayer.shamanWaterBuff != 0 && modPlayer.timerVial == 30) {
						modPlayer.timerVial = 0;
						player.HealEffect(3, true);
						player.statLife += 3;
					}
						
					if (Main.rand.Next(15) == 0 && modPlayer.shamanDownpour && OrchidModShamanHelper.getNbShamanicBonds(player, modPlayer, mod) > 2) {
						int dmg = (int)(50 * modPlayer.shamanDamage + 5E-06f);
						target.StrikeNPCNoInteraction(dmg, 0f, 0);
						Main.PlaySound(2, (int)target.Center.X ,(int)target.Center.Y - 200, 93);
							
						for(int i=0; i < 15; i++) {
							int dust = Dust.NewDust(target.position, target.width, target.height, 226);
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 2f;
						}
					}
				}
			}
		}
		
		public static void DrawEffectsShaman(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright, Player player, OrchidModPlayer modPlayer, Mod mod) {
			if (modPlayer.shamanShadowEmpowerment) {
				if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 27, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 1f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					Main.playerDrawDust.Add(dust);
				}
			}
			
			if (modPlayer.abyssalWings && player.controlJump) {
				if (Main.rand.Next(6) == 0 && drawInfo.shadow == 0f && player.wingTime > 0)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(15f, 2f), player.width + 30, player.height + 4, DustType<AbyssalDust>());
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity.Y -= 0.5f;
					Main.dust[dust].scale = 1.75f;
					Main.playerDrawDust.Add(dust);
				}
				if (Main.rand.Next(6) == 0 && drawInfo.shadow == 0f && player.wingTime > 0)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(15f, 2f), player.width + 30, player.height + 4, DustType<AbyssalDustBright>());
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity.Y -= 0.5f;
					Main.dust[dust].scale = 1.75f;
					Main.playerDrawDust.Add(dust);
				}
			}	
		}
		
		public static bool PreHurtShaman(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit,
		ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, Player player, OrchidModPlayer modPlayer, Mod mod) {
			if (modPlayer.shamanMourningTorch) {
				modPlayer.shamanFireTimer -= 5 * 60;
				modPlayer.shamanWaterTimer -= 5 * 60;
				modPlayer.shamanAirTimer -= 5 * 60;
				modPlayer.shamanEarthTimer -= 5 * 60;
				modPlayer.shamanSpiritTimer -= 5 * 60;
			}
			
			if (modPlayer.shamanSunBelt) {
				player.AddBuff((mod.BuffType("BrokenPower")), 60 * 15);
			}
			
			if (modPlayer.shamanHell && modPlayer.shamanTimerHellDefense == 300 && modPlayer.shamanEarthTimer > 0) {
				modPlayer.shamanTimerHellDefense = 0;
				int dmg = (int)(50 * modPlayer.shamanDamage);
				Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ProjectileType<Shaman.Projectiles.Equipment.Hell.ShamanHellExplosion>(), dmg, 0.0f, player.whoAmI, 0.0f, 0.0f);
				Main.PlaySound(2, (int)player.Center.X, (int)player.Center.Y, 14);
				OrchidModProjectile.spawnDustCircle(player.Center, 6, 10, 15, true, 1f, 1f, 8f, true, true, false, 0, 0, true);
				OrchidModProjectile.spawnDustCircle(player.Center, 6, 10, 15, true, 1.5f, 1f, 5f, true, true, false, 0, 0, true);
				OrchidModProjectile.spawnDustCircle(player.Center, 6, 10, 15, true, 2f, 1f, 3f, true, true, false, 0, 0, true);
			}
			
			if (modPlayer.shamanDiabolist && modPlayer.shamanEarthTimer > 0) {
				modPlayer.shamanTimerDiabolist = 60;
				modPlayer.shamanDiabolistCount += damage;
			}
			
			return true;
		}
		
		public static void BondHotKeyPressed(Player player, OrchidModPlayer modPlayer, Mod mod) {
			if (modPlayer.shamanFireBondLoading >= 100) {
				int level = (modPlayer.shamanFireBuff + modPlayer.shamanFireBonus);
				int dmg = (int)(10 * level * modPlayer.shamanDamage + 5E-06f);
				float spd = 4f + (float)(level / 3);
				int proj = mod.ProjectileType("FireBondProj1");
				if (level >= 3) proj = mod.ProjectileType("FireBondProj2");
				if (level >= 5) proj = mod.ProjectileType("FireBondProj3");
				if (level >= 8) proj = mod.ProjectileType("FireBondProj4");
				if (level >= 12) proj = mod.ProjectileType("FireBondProj5");
				Vector2 mouseTarget = Main.screenPosition + new Vector2((float)Main.mouseX - 8, (float)Main.mouseY);
				Vector2 heading = mouseTarget - player.position;
				heading.Normalize();
				heading *= new Vector2(spd, spd).Length();
				//Vector2 vel = ( new Vector2(heading.X, heading.Y).RotatedByRandom(MathHelper.ToRadians(10)));
				Vector2 vel = (new Vector2(heading.X, heading.Y));
				Projectile.NewProjectile(player.Center.X, player.Center.Y, vel.X, vel.Y, proj, dmg, 0f, player.whoAmI);
				modPlayer.shamanFireBondLoading = 0;
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 45);
			}
			
			if (modPlayer.shamanWaterBondLoading >= 100) {
				int level = (modPlayer.shamanWaterBuff + modPlayer.shamanWaterBonus);
				int dmg = (int)(5 * level * modPlayer.shamanDamage + 5E-06f);
				int nb = 3;
				int proj = mod.ProjectileType("WaterBondProj");
				int rotation = -25;
				int ai = 0;
				
				if (level >= 3) {
					nb ++;
					ai ++;
				}
				
				if (level >= 5) {
					nb ++;
					ai ++;
				}
				
				if (level >= 8) {
					nb ++;
					ai ++;
				}
				
				if (level >= 12) {
					nb ++;
					ai ++;
				}
				
				Vector2 heading = new Vector2(player.position.X, player.position.Y - 10f);
				heading = heading - player.position;
				heading.Normalize();
				heading *= new Vector2(-5f, -5f).Length();
				int rotationVal = (int)(50 / nb);
				while (nb > 0) {
					Vector2 vel = (new Vector2(heading.X, heading.Y).RotatedBy(MathHelper.ToRadians(rotation)));
					int waterProj = Projectile.NewProjectile(player.Center.X, player.Center.Y, vel.X, vel.Y, proj, dmg, 0f, player.whoAmI);
					Main.projectile[waterProj].ai[0] = ai;
					Main.projectile[waterProj].netUpdate = true;
					rotation += rotationVal;
					nb --;
				}
				modPlayer.shamanWaterBondLoading = 0;
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 45);
			}
			
			if (modPlayer.shamanAirBondLoading >= 100) {
				int level = (modPlayer.shamanAirBuff + modPlayer.shamanAirBonus);
				int dmg = (int)(10 * level * modPlayer.shamanDamage + 5E-06f);
				int proj = mod.ProjectileType("WindBondProj1");
				int ai = 0;
				
				if (level >= 3) {
					ai ++;
				}
				
				if (level >= 5) {
					ai ++;
				}
				
				if (level >= 8) {
					ai ++;
				}
				
				if (level >= 12) {
					ai ++;
				}
				
				int airProj = Projectile.NewProjectile(player.Center.X, player.position.Y - 300, 0f, 1f, proj, 0, 0.0f, player.whoAmI);
				Main.projectile[airProj].ai[1] = ai;
				Main.projectile[airProj].netUpdate = true;
				modPlayer.shamanAirBondLoading = 0;
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 45);
			}
			
			if (modPlayer.shamanEarthBondLoading >= 100) {
				int level = (modPlayer.shamanEarthBuff + modPlayer.shamanEarthBonus);
				
				if (level >= 3 && level < 8) {
					float posX = player.position.X;
					float posY = player.position.Y;
					
					for (int k = 0; k < 255; k++) {
						Player targetPlayer = Main.player[k];
						if (MathHelper.Distance(posX, targetPlayer.position.X) < 100f && MathHelper.Distance(posY, targetPlayer.position.Y) < 100f) {
							targetPlayer.AddBuff((mod.BuffType("EarthDefense")), 60 * 10);
						}
					}
				}
				
				if (level >= 8) {
					float posX = player.position.X;
					float posY = player.position.Y;
					
					for (int k = 0; k < 255; k++) {
						Player targetPlayer = Main.player[k];
						if (MathHelper.Distance(posX, targetPlayer.position.X) < 100f && MathHelper.Distance(posY, targetPlayer.position.Y) < 100f) {
							targetPlayer.AddBuff((mod.BuffType("EarthRegeneration")), 60 * 10);
						}
					}
				}
				
				if (level >= 5) {
					int projTotem = mod.ProjectileType("EarthBondProj");
					
					for (int l = 0; l < Main.projectile.Length; l++)
					{  
						Projectile proj = Main.projectile[l];
						if (proj.active && proj.type == projTotem && proj.owner == player.whoAmI)
						{
							proj.active = false;
						}
					}
					
					int earthProj = Projectile.NewProjectile(player.Center.X, player.position.Y, 0f, 1f, projTotem, 0, 0.0f, player.whoAmI);
					
					 
					if (level >= 12) {
						Main.projectile[earthProj].ai[1] = 1;
					} else {
						Main.projectile[earthProj].ai[1] = 0;
					}
					Main.projectile[earthProj].netUpdate = true;
				}
				
				if (Main.myPlayer == player.whoAmI)
					player.HealEffect(15 * level, true);
				player.statLife += 10 * level;	
				
				modPlayer.shamanEarthBondLoading = 0;
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 45);
			}
			
			if (modPlayer.shamanSpiritBondLoading >= 100) {
				int level = (modPlayer.shamanSpiritBuff + modPlayer.shamanSpiritBonus);
				
				if (level >= 3 && level < 8) {
					float posX = player.position.X;
					float posY = player.position.Y;
					
					for (int k = 0; k < 255; k++) {
						Player targetPlayer = Main.player[k];
						if (MathHelper.Distance(posX, targetPlayer.position.X) < 100f && MathHelper.Distance(posY, targetPlayer.position.Y) < 100f) {
							targetPlayer.AddBuff((mod.BuffType("SpiritAttack")), 60 * 10);
						}
					}
				}
				
				if (level >= 8) {
					float posX = player.position.X;
					float posY = player.position.Y;
					
					for (int k = 0; k < 255; k++) {
						Player targetPlayer = Main.player[k];
						if (MathHelper.Distance(posX, targetPlayer.position.X) < 100f && MathHelper.Distance(posY, targetPlayer.position.Y) < 100f) {
							targetPlayer.AddBuff((mod.BuffType("SpiritRegeneration")), 60 * 10);
						}
					}
				}
				
				if (level >= 5) {
					int projTotem = mod.ProjectileType("SpiritBondProj1");
					
					for (int l = 0; l < Main.projectile.Length; l++)
					{  
						Projectile proj = Main.projectile[l];
						if (proj.active && proj.type == projTotem && proj.owner == player.whoAmI)
						{
							proj.active = false;
						}
					}
					
					int earthProj = Projectile.NewProjectile(player.Center.X, player.position.Y, 0f, -1f, projTotem, 0, 0.0f, player.whoAmI);
					
					 
					if (level >= 12) {
						Main.projectile[earthProj].ai[1] = 1;
					} else {
						Main.projectile[earthProj].ai[1] = 0;
					}
					Main.projectile[earthProj].netUpdate = true;
				}
				
				int maxBufftimer = 60 * modPlayer.shamanBuffTimer;
				int toAdd = 90 + (30 * level);
				modPlayer.shamanFireTimer = modPlayer.shamanFireBuff == 0 ? 0 : modPlayer.shamanFireTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanFireTimer + toAdd;
				modPlayer.shamanWaterTimer = modPlayer.shamanWaterBuff == 0 ? 0 : modPlayer.shamanWaterTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanWaterTimer + toAdd;
				modPlayer.shamanAirTimer = modPlayer.shamanAirBuff == 0 ? 0 : modPlayer.shamanAirTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanAirTimer + toAdd;
				modPlayer.shamanEarthTimer = modPlayer.shamanEarthBuff == 0 ? 0 : modPlayer.shamanEarthTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanEarthTimer + toAdd;
				
				modPlayer.shamanSpiritBondLoading = 0;
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 45);
			}
		}
		
		public static void ResetEffectsShaman(Player player, OrchidModPlayer modPlayer, Mod mod) {
			modPlayer.shamanCrit = 0;
			modPlayer.shamanDamage = 1.0f;
			modPlayer.shamanBuffTimer = 5;
			modPlayer.hauntedCandle = false;
			modPlayer.spawnedGhost = false;
			modPlayer.doubleJumpHarpy = false;
			modPlayer.abyssalWings = false;
			modPlayer.abyssSet = false;
			
			modPlayer.shamanFireBonus = 0;
			modPlayer.shamanWaterBonus = 0;
			modPlayer.shamanAirBonus = 0;
			modPlayer.shamanEarthBonus = 0;
			modPlayer.shamanSpiritBonus = 0;
			
			modPlayer.shamanFire = false;
			modPlayer.shamanIce = false;
			modPlayer.shamanPoison = false;
			modPlayer.shamanVenom = false;
			modPlayer.shamanHoney = false;
			modPlayer.shamanFeather = false;
			modPlayer.shamanVampire = false;
			modPlayer.shamanDestroyer = false;
			modPlayer.shamanDiabolist = false;
			modPlayer.shamanDripping = false;
			modPlayer.shamanAmber = false;
			modPlayer.shamanDryad = false;
			modPlayer.shamanForest = false;
			modPlayer.shamanHeavy = false;
			modPlayer.shamanSkull = false;
			modPlayer.shamanWaterHoney = false;
			modPlayer.shamanSmite = false;
			modPlayer.shamanCrimtane = false;
			modPlayer.shamanDemonite = false;
			modPlayer.shamanDownpour = false;
			modPlayer.shamanHell = false;
			modPlayer.shamanHarpyAnklet = false;
			modPlayer.shamanWyvern = false;
			modPlayer.shamanRage = false;
			
			modPlayer.shamanShadowEmpowerment = false;
			modPlayer.shamanMourningTorch = false;
			modPlayer.shamanSunBelt = false;
			
			modPlayer.shamanHitDelay -= modPlayer.shamanHitDelay > 0 && modPlayer.timer120 % 5 == 0 ? 1 : 0;
		}
		
		public static void onRespawnShaman(Player player, OrchidModPlayer modPlayer, Mod mod) {
			modPlayer.orbCountSmall = 0;
			modPlayer.orbCountBig = 0;
			modPlayer.orbCountLarge = 0;
			modPlayer.orbCountUnique = 0;
			modPlayer.orbCountCircle = 0;
			
			modPlayer.shamanOrbBig = ShamanOrbBig.NULL;
			modPlayer.shamanOrbSmall = ShamanOrbSmall.NULL;
			modPlayer.shamanOrbLarge = ShamanOrbLarge.NULL;
			modPlayer.shamanOrbUnique = ShamanOrbUnique.NULL;
			modPlayer.shamanOrbCircle = ShamanOrbCircle.NULL;
			
			modPlayer.shamanFireTimer = 0;
			modPlayer.shamanFireBuff = 0;
			modPlayer.shamanWaterTimer = 0;
			modPlayer.shamanWaterBuff = 0;
			modPlayer.shamanAirTimer = 0;
			modPlayer.shamanAirBuff = 0;
			modPlayer.shamanEarthTimer = 0;
			modPlayer.shamanEarthBuff = 0;
			modPlayer.shamanSpiritTimer = 0;
			modPlayer.shamanSpiritBuff = 0;
			modPlayer.UIDisplayTimer = 0;
		}
		
		public static int getNbShamanicBonds(Player player, OrchidModPlayer modPlayer, Mod mod) {
			int val = 0;
			
			if (modPlayer.shamanFireBuff != 0)
				val ++;
			
			if (modPlayer.shamanWaterBuff != 0)
				val ++;
			
			if (modPlayer.shamanAirBuff != 0)
				val ++;
			
			if (modPlayer.shamanEarthBuff != 0)
				val ++;
				
			if (modPlayer.shamanSpiritBuff != 0)
				val ++;
			
			return val;
		}
		
		public static void addShamanicEmpowerment(int type, int level, Player player, OrchidModPlayer modPlayer, Mod mod) {
			if (type == 0) {
				return;
			}
			
			if (modPlayer.shamanForest && type == 4  && modPlayer.shamanEarthTimer == 0) {
				player.AddBuff(BuffType<Shaman.Buffs.DeepForestAura>(), 1);
				int projType = ProjectileType<Shaman.Projectiles.Equipment.DeepForestCharmProj>();
				Projectile.NewProjectile(player.Center.X, player.position.Y, 0f, 0f, projType, 1, 0, player.whoAmI, 0f, 0f);
				Projectile.NewProjectile(player.Center.X, player.position.Y, 0f, 0f, projType, 2, 0, player.whoAmI, 0f, 0f);	
			}

			bool newEmpowerment = false;
			int currentTimer = 0;
			int lowestDuration = 60 * modPlayer.shamanBuffTimer + 1;
					
			lowestDuration = (modPlayer.shamanFireTimer < lowestDuration) ? modPlayer.shamanFireTimer : lowestDuration;
			lowestDuration = (modPlayer.shamanWaterTimer < lowestDuration) ? modPlayer.shamanWaterTimer : lowestDuration;
			lowestDuration = (modPlayer.shamanAirTimer < lowestDuration) ? modPlayer.shamanAirTimer : lowestDuration;
			lowestDuration = (modPlayer.shamanEarthTimer < lowestDuration) ? modPlayer.shamanEarthTimer : lowestDuration;
			lowestDuration = (modPlayer.shamanSpiritTimer < lowestDuration) ? modPlayer.shamanSpiritTimer : lowestDuration;
					
			switch (type) {
				case 1:
					currentTimer = modPlayer.shamanFireTimer;
					break;
				case 2:
					currentTimer = modPlayer.shamanWaterTimer;
					break;
				case 3:
					currentTimer = modPlayer.shamanAirTimer;
					break;
				case 4:
					currentTimer = modPlayer.shamanEarthTimer;
					break;
				case 5:
					currentTimer = modPlayer.shamanSpiritTimer;
					break;
				default:
					return;
			}
				
			newEmpowerment = currentTimer == 0;
			if (currentTimer == lowestDuration) modPlayer.shamanPollWater += 3;
			if (currentTimer < (modPlayer.shamanBuffTimer * 60) / 2) modPlayer.shamanPollWater += 2;
			if (currentTimer < ((modPlayer.shamanBuffTimer * 60) / 4) * 3) modPlayer.shamanPollWater += 2;
			if (currentTimer < (modPlayer.shamanBuffTimer * 60) / 4) modPlayer.shamanPollWater += 2;
				
			if (newEmpowerment) {
				modPlayer.shamanPollWater += 2;
				if (modPlayer.shamanDryad && OrchidModShamanHelper.getNbShamanicBonds(player, modPlayer, mod) > 0) {
					int lowestNoZeroDuation = 60 * modPlayer.shamanBuffTimer + 1;
							
					lowestNoZeroDuation = (modPlayer.shamanFireTimer != 0 && modPlayer.shamanFireTimer < lowestDuration) ? 0 + modPlayer.shamanFireTimer : lowestDuration;
					lowestNoZeroDuation = (modPlayer.shamanWaterTimer != 0 && modPlayer.shamanWaterTimer < lowestDuration) ? 0 +modPlayer.shamanWaterTimer : lowestDuration;
					lowestNoZeroDuation = (modPlayer.shamanAirTimer != 0 && modPlayer.shamanAirTimer < lowestDuration) ? 0 + modPlayer.shamanAirTimer : lowestDuration;
					lowestNoZeroDuation = (modPlayer.shamanEarthTimer != 0 && modPlayer.shamanEarthTimer < lowestDuration) ? 0 + modPlayer.shamanEarthTimer : lowestDuration;
					lowestNoZeroDuation = (modPlayer.shamanSpiritTimer != 0 && modPlayer.shamanSpiritTimer < lowestDuration) ? 0 + modPlayer.shamanSpiritTimer : lowestDuration;
						
					modPlayer.shamanFireTimer = (modPlayer.shamanFireTimer == lowestDuration) ? modPlayer.shamanFireTimer + 60 * 3 : modPlayer.shamanFireTimer;
					modPlayer.shamanWaterTimer = (modPlayer.shamanWaterTimer == lowestDuration) ? modPlayer.shamanWaterTimer + 60 * 3 : modPlayer.shamanWaterTimer;
					modPlayer.shamanAirTimer = (modPlayer.shamanAirTimer == lowestDuration) ? modPlayer.shamanAirTimer + 60 * 3 : modPlayer.shamanAirTimer;
					modPlayer.shamanEarthTimer = (modPlayer.shamanEarthTimer == lowestDuration) ? modPlayer.shamanEarthTimer + 60 * 3 : modPlayer.shamanEarthTimer;
					modPlayer.shamanSpiritTimer = (modPlayer.shamanSpiritTimer == lowestDuration) ? modPlayer.shamanSpiritTimer + 60 * 3 : modPlayer.shamanSpiritTimer;
				}
			}
			
			int maxBufftimer = 60 * modPlayer.shamanBuffTimer;
			int toAdd = (int)(maxBufftimer / (2 + modPlayer.shamanHitDelay - (player.HasBuff(BuffType<Shaman.Buffs.SpiritRegeneration>()) ? 1 : 0)));
			switch (type) {
				case 1:
					modPlayer.shamanFireBuff = level > modPlayer.shamanFireBuff ? level : modPlayer.shamanFireBuff;
					modPlayer.shamanFireTimer = modPlayer.shamanFireTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanFireTimer + toAdd;
					break;
				case 2:
					modPlayer.shamanWaterBuff = level > modPlayer.shamanWaterBuff ? level : modPlayer.shamanWaterBuff;
					modPlayer.shamanWaterTimer = modPlayer.shamanWaterTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanWaterTimer + toAdd;
					break;
				case 3:
					modPlayer.shamanAirBuff = level > modPlayer.shamanAirBuff ? level : modPlayer.shamanAirBuff;
					modPlayer.shamanAirTimer = modPlayer.shamanAirTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanAirTimer + toAdd;
					break;
				case 4:
					modPlayer.shamanEarthBuff = level > modPlayer.shamanEarthBuff ? level : modPlayer.shamanEarthBuff;
					modPlayer.shamanEarthTimer = modPlayer.shamanEarthTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanEarthTimer + toAdd;
					break;
				case 5:
					modPlayer.shamanSpiritBuff = level > modPlayer.shamanSpiritBuff ? level : modPlayer.shamanSpiritBuff;
					modPlayer.shamanSpiritTimer = modPlayer.shamanSpiritTimer + toAdd > maxBufftimer ? maxBufftimer : modPlayer.shamanSpiritTimer + toAdd;
					break;
				default:
					return;
			}
			modPlayer.shamanHitDelay = 8;
		}
	}
}