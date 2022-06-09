using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.General.Items.Sets.StaticQuartz.Projectiles
{
	public class StaticQuartzHealerPro : ModProjectile
	{
		Mod thoriumMod = OrchidMod.ThoriumMod;

		/// <summary>
		/// Flag checked when the projectile has scythe charges and a suitable NPC is hit, then set to false
		/// </summary>
		//true by default
		public bool CanGiveScytheCharge
		{
			//Clientside only, hence localAI
			get => Projectile.localAI[0] == 0f;
			set => Projectile.localAI[0] = value ? 0f : 1f;
		}

		/// <summary>
		/// Flag checked when the projectile hits an NPC for the first time, then set to false
		/// </summary>
		//true by default
		public bool FirstHit
		{
			//Clientside only, hence localAI
			get => Projectile.localAI[1] == 0f;
			set => Projectile.localAI[1] = value ? 0f : 1f;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Static Quartz Scythe");
		}

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.aiStyle = 0;
			Projectile.penetrate = -1;
			Projectile.light = 0.2f;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 26;

			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 10;
		}

		public override void OnHitNPC(NPC npc, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];

			//TODO thorium
			if (thoriumMod != null)
			{
				ModPlayer thoriumPlayer = player.GetModPlayer(thoriumMod, "ThoriumPlayer");
				if (CanGiveScytheCharge && !npc.friendly && npc.lifeMax > 5 && npc.chaseable && !npc.dontTakeDamage && !npc.immortal)
				{
					CanGiveScytheCharge = false;

					player.AddBuff(thoriumMod.Find<ModBuff>("SoulEssence").Type, 30 * 60, true);
					CombatText.NewText(npc.Hitbox, new Color(100, 255, 200), 1, false, true);

					FieldInfo fieldSoul = thoriumPlayer.GetType().GetField("soulEssence", BindingFlags.Public | BindingFlags.Instance);
					if (fieldSoul != null)
					{
						int healCharge = (int)(fieldSoul.GetValue(thoriumPlayer)) + 1;
						fieldSoul.SetValue(thoriumPlayer, healCharge);
					}
				}

				if (FirstHit)
				{
					FirstHit = false;

					//Things on first hit some scythes have
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			Player player = Main.player[Projectile.owner];
			hitDirection = target.Center.X < player.Center.X ? -1 : 1;

			//TODO thorium
			if (thoriumMod != null)
			{
				ModPlayer thoriumPlayer = player.GetModPlayer(thoriumMod, "ThoriumPlayer");
				Type playerType = thoriumPlayer.GetType();
				FieldInfo critField = playerType.GetField("radiantCrit", BindingFlags.Public | BindingFlags.Instance);
				if (critField != null)
				{
					int healCrit = (int)critField.GetValue(thoriumPlayer);
					crit = Main.rand.Next(101) <= healCrit;
				}
				else
				{
					crit = false;
				}

				FieldInfo fieldWarlock = playerType.GetField("warlockSet", BindingFlags.Public | BindingFlags.Instance);
				if (fieldWarlock != null)
				{
					bool healWarlock = (bool)fieldWarlock.GetValue(thoriumPlayer);

					if (healWarlock && Main.rand.NextFloat() < 0.5f)
					{
						int shadowWispType = thoriumMod.Find<ModProjectile>("ShadowWisp").Type;
						if (player.ownedProjectileCounts[shadowWispType] < 15)
						{
							Projectile.NewProjectile((int)target.Center.X, (int)target.Center.Y, 0f, -2f, shadowWispType, (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);
						}
					}
				}

				FieldInfo fieldIridescent = playerType.GetField("iridescentSet", BindingFlags.Public | BindingFlags.Instance);
				if (fieldIridescent != null)
				{
					bool healIridescent = (bool)fieldIridescent.GetValue(thoriumPlayer);

					if (healIridescent && Main.rand.NextFloat() < 0.15f)
					{
						SoundEngine.PlaySound(SoundID.Item, (int)Projectile.Center.X, (int)Projectile.Center.Y, 100, 1f, 0f);
						for (int k = 0; k < 20; k++)
						{
							int dust = Dust.NewDust(target.position, target.width, target.height, 87, Main.rand.Next((int)-6f, (int)6f), Main.rand.Next((int)-6f, (int)6f), 0, default(Color), 1.25f);
							Main.dust[dust].noGravity = true;
						}
						for (int k = 0; k < 10; k++)
						{
							int dust = Dust.NewDust(target.position, target.width, target.height, 91, Main.rand.Next((int)-2f, (int)2f), Main.rand.Next((int)-2f, (int)2f), 0, default(Color), 1.15f);
							Main.dust[dust].noGravity = true;
						}

						int healNoEffects = thoriumMod.Find<ModProjectile>("HealNoEffects").Type;
						int heal = 0;
						if (target.type != NPCID.TargetDummy)
						{
							for (int k = 0; k < Main.maxPlayers; k++)
							{
								Player ally = Main.player[k];
								if (ally.active && ally != player && ally.statLife < ally.statLifeMax2 && ally.DistanceSQ(Projectile.Center) < 500 * 500)
								{
									Projectile.NewProjectile(ally.Center, Vector2.Zero, healNoEffects, 0, 0, Projectile.owner, heal, ally.whoAmI);
								}
							}
						}

						for (int u = 0; u < Main.maxNPCs; u++)
						{
							NPC enemyTarget = Main.npc[u];
							if (enemyTarget.CanBeChasedBy() && enemyTarget.DistanceSQ(player.Center) < 250 * 250)
							{
								enemyTarget.AddBuff(BuffID.Confused, 120, false);
							}
						}
					}
				}
			}
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (Projectile.timeLeft < 10)
			{
				Projectile.alpha += 30;
			}

			if (player.dead)
			{
				Projectile.Kill();
			}

			int dir = (player.direction > 0).ToDirectionInt();
			Projectile.rotation += dir * 0.25f;
			Projectile.spriteDirection = dir;

			player.heldProj = Projectile.whoAmI;
			Projectile.Center = player.Center;
			Projectile.gfxOffY = player.gfxOffY;
		}
	}
}
