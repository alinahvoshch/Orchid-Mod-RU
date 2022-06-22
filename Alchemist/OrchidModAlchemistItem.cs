﻿using Microsoft.Xna.Framework;
using OrchidMod.Alchemist.Projectiles;
using OrchidMod.Common.Globals.NPCs;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Alchemist
{
	public abstract class OrchidModAlchemistItem : OrchidModItem
	{
		public int potencyCost = 0;
		public int secondaryDamage = 0;
		public float secondaryScaling = 1f;
		public AlchemistElement element = AlchemistElement.NULL;
		public int rightClickDust = -1;
		public int colorR = 255;
		public int colorG = 255;
		public int colorB = 255;

		public virtual void SafeSetDefaults() { }

		public virtual void KillFirst(int timeLeft, Player player, OrchidModPlayer modPlayer, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem) { }
		public virtual void KillSecond(int timeLeft, Player player, OrchidModPlayer modPlayer, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem) { }
		public virtual void KillThird(int timeLeft, Player player, OrchidModPlayer modPlayer, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem) { }

		public virtual void OnHitNPCFirst(NPC target, int damage, float knockback, bool crit, Player player, OrchidModPlayer modPlayer, OrchidModAlchemistNPC modTarget, OrchidGlobalNPC modTargetGlobal, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem) { }
		public virtual void OnHitNPCSecond(NPC target, int damage, float knockback, bool crit, Player player, OrchidModPlayer modPlayer, OrchidModAlchemistNPC modTarget, OrchidGlobalNPC modTargetGlobal, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem) { }
		public virtual void OnHitNPCThird(NPC target, int damage, float knockback, bool crit, Player player, OrchidModPlayer modPlayer, OrchidModAlchemistNPC modTarget, OrchidGlobalNPC modTargetGlobal, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem) { }

		public virtual void AddVariousEffects(Player player, OrchidModPlayer modPlayer, AlchemistProj alchProj, Projectile projectile, OrchidModGlobalItem globalItem) { }

		public sealed override void SetDefaults()
		{
			SafeSetDefaults();
			Item.DamageType = DamageClass.Generic;
			Item.noMelee = true;
			Item.crit = 4;
			Item.useStyle = 1;
			Item.UseSound = SoundID.Item106;
			Item.consumable = false;
			Item.noUseGraphic = true;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.autoReuse = false;
			Item.shootSpeed = 10f;
			Item.knockBack = 1f;

			OrchidModGlobalItem orchidItem = Item.GetGlobalItem<OrchidModGlobalItem>();
			orchidItem.alchemistColorR = this.colorR;
			orchidItem.alchemistColorG = this.colorG;
			orchidItem.alchemistColorB = this.colorB;
			orchidItem.alchemistRightClickDust = this.rightClickDust;
			orchidItem.alchemistPotencyCost = this.potencyCost;
			orchidItem.alchemistElement = this.element;
			orchidItem.alchemistSecondaryDamage = this.secondaryDamage;
			orchidItem.alchemistSecondaryScaling = this.secondaryScaling;
			orchidItem.alchemistWeapon = true;

			orchidItem.killFirstDelegate = KillFirst;
			orchidItem.killSecondDelegate = KillSecond;
			orchidItem.killThirdDelegate = KillThird;
			orchidItem.onHitNPCFirstDelegate = OnHitNPCFirst;
			orchidItem.onHitNPCSecondDelegate = OnHitNPCSecond;
			orchidItem.onHitNPCThirdDelegate = OnHitNPCThird;
			orchidItem.addVariousEffectsDelegate = AddVariousEffects;
		}

		public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
		{
			damage *= player.GetModPlayer<OrchidModPlayer>().alchemistDamage;
		}

		public override void ModifyWeaponCrit(Player player, ref float crit)
		{
			crit += player.GetModPlayer<OrchidModPlayer>().alchemistCrit;
		}

		public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
			if (Main.rand.Next(101) <= (player.GetModPlayer<OrchidModPlayer>().alchemistCrit))
				crit = true;
			else crit = false;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			OrchidModPlayer orchidModPlayer = player.GetModPlayer<OrchidModPlayer>();
			bool[] elements = orchidModPlayer.alchemistElements;

			if (player == Main.LocalPlayer)
			{
				orchidModPlayer.alchemistPotencyDisplayTimer = 180;

				bool alreadyContains = false;
				if ((int)this.element > 0 && (int)this.element < 7)
				{
					alreadyContains = elements[(int)this.element - 1];
				}

				if (player.altFunctionUse == 2)
				{
					Item.useAnimation = 10;
					Item.useTime = 10;
					bool noPotency = orchidModPlayer.alchemistPotency < this.potencyCost + 1;

					if (alreadyContains || noPotency || orchidModPlayer.alchemistNbElements >= orchidModPlayer.alchemistNbElementsMax)
					{
						if (noPotency && !alreadyContains)
						{
							Item.UseSound = SoundID.Item7;
							SoundEngine.PlaySound(SoundID.SplashWeak);
						}
						else
						{
							if (Main.rand.Next(2) == 0)
							{
								Item.UseSound = SoundID.Item112;
							}
							else
							{
								Item.UseSound = SoundID.Item111;
							}
						}
					}
					else
					{
						int rand = Main.rand.Next(3);
						switch (rand)
						{
							case 1:
								Item.UseSound = SoundID.Item86;
								break;
							case 2:
								Item.UseSound = SoundID.Item87;
								break;
							default:
								Item.UseSound = SoundID.Item85;
								break;
						}
						playerAddFlask(player, element, Item.type, Item.damage, potencyCost, rightClickDust, colorR, colorG, colorB);
						if (this.rightClickDust != -1)
						{
							for (int i = 0; i < 5; i++)
							{
								int dust = Dust.NewDust(player.Center, 10, 10, this.rightClickDust);
								Main.dust[dust].scale *= 1f;
							}
						}
					}
					Item.shoot = Mod.Find<ModProjectile>("AlchemistRightClick").Type;
				}
				else
				{
					if (!alreadyContains && orchidModPlayer.alchemistPotency > this.potencyCost && orchidModPlayer.alchemistNbElements < orchidModPlayer.alchemistNbElementsMax)
					{
						playerAddFlask(player, element, Item.type, Item.damage, potencyCost, rightClickDust, colorR, colorG, colorB);
					}

					if (orchidModPlayer.alchemistNbElements > 0)
					{
						Item.shootSpeed = 10f * orchidModPlayer.alchemistVelocity;
						Item.shoot = ProjectileType<Alchemist.Projectiles.AlchemistProj>();
						Item.UseSound = SoundID.Item106;
						Item.useAnimation = 30;
						Item.useTime = 30;
					}
					else
					{
						SoundEngine.PlaySound(SoundID.SplashWeak);
						return false;
					}
				}
			}
			else
			{
				return false;
			}
			return base.CanUseItem(player);
		}

		protected override bool CloneNewInstances => true;

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
			if (tt != null)
			{
				string[] splitText = tt.Text.Split(' ');
				string damageValue = splitText.First();
				string damageWord = splitText.Last();
				tt.Text = damageValue + " chemical " + damageWord;
			}

			switch (this.element)
			{
				case AlchemistElement.WATER:
					tooltips.Insert(1, new TooltipLine(Mod, "ElementTag", "Water Element")
					{
						OverrideColor = new Color(0, 119, 190)
					});
					break;
				case AlchemistElement.FIRE:
					tooltips.Insert(1, new TooltipLine(Mod, "ElementTag", "Fire Element")
					{
						OverrideColor = new Color(194, 38, 31)
					});
					break;
				case AlchemistElement.NATURE:
					tooltips.Insert(1, new TooltipLine(Mod, "ElementTag", "Nature Element")
					{
						OverrideColor = new Color(75, 139, 59)
					});
					break;
				case AlchemistElement.AIR:
					tooltips.Insert(1, new TooltipLine(Mod, "ElementTag", "Air Element")
					{
						OverrideColor = new Color(166, 231, 255)
					});
					break;
				case AlchemistElement.LIGHT:
					tooltips.Insert(1, new TooltipLine(Mod, "ElementTag", "Light Element")
					{
						OverrideColor = new Color(255, 255, 102)
					});
					break;
				case AlchemistElement.DARK:
					tooltips.Insert(1, new TooltipLine(Mod, "ElementTag", "Dark Element")
					{
						OverrideColor = new Color(138, 43, 226)
					});
					break;
				default:
					break;
			}

			tooltips.Insert(1, new TooltipLine(Mod, "Mix", "Right click to mix")
			{
				OverrideColor = new Color(155, 255, 155)
			});

			Mod thoriumMod = OrchidMod.ThoriumMod;
			if (thoriumMod != null)
			{
				tooltips.Insert(1, new TooltipLine(Mod, "ClassTag", "-Alchemist Class-")
				{
					OverrideColor = new Color(155, 255, 55)
				});
			}

			// tt = tooltips.FirstOrDefault(x => x.Name == "CritChance" && x.mod == "Terraria");
			// if (tt != null) tooltips.Remove(tt);

			tt = tooltips.FirstOrDefault(x => x.Name == "Knockback" && x.Mod == "Terraria");
			if (tt != null) tt.Text = "Uses " + this.potencyCost + " potency";

			tt = tooltips.FirstOrDefault(x => x.Name == "Speed" && x.Mod == "Terraria");
			if (tt != null) tooltips.Remove(tt);

			//tooltips.Insert(tooltips.Count - 1, new TooltipLine(mod, "PotencyUse", "Uses " + this.potencyCost + " potency"));
		}

		public static void playerAddFlask(Player player, AlchemistElement element, int flaskType, int damage, int potencyCost, int rightClickDust, int colorR, int colorG, int colorB)
		{
			OrchidModPlayer orchidModPlayer = player.GetModPlayer<OrchidModPlayer>();
			bool[] elements = orchidModPlayer.alchemistElements;
			Item[] flasks = orchidModPlayer.alchemistFlasks;

			//damage = (int)(damage * player.allDamage);
			
			int index = (int)element - 1;

			orchidModPlayer.alchemistPotency -= potencyCost;
			orchidModPlayer.alchemistPotencyWait = 300;

			elements[index] = true;
			flasks[index] = new Item();
			flasks[index].SetDefaults(flaskType);
			orchidModPlayer.alchemistFlaskDamage += damage;
			orchidModPlayer.alchemistNbElements++;

			int divider = orchidModPlayer.alchemistNbElements;
			orchidModPlayer.alchemistColorR = (int)(((orchidModPlayer.alchemistColorR * (divider - 1)) + colorR) / divider);
			orchidModPlayer.alchemistColorG = (int)(((orchidModPlayer.alchemistColorG * (divider - 1)) + colorG) / divider);
			orchidModPlayer.alchemistColorB = (int)(((orchidModPlayer.alchemistColorB * (divider - 1)) + colorB) / divider);

			orchidModPlayer.alchemistColorR = orchidModPlayer.alchemistColorR > 255 ? 255 : orchidModPlayer.alchemistColorR;
			orchidModPlayer.alchemistColorG = orchidModPlayer.alchemistColorG > 255 ? 255 : orchidModPlayer.alchemistColorG;
			orchidModPlayer.alchemistColorB = orchidModPlayer.alchemistColorB > 255 ? 255 : orchidModPlayer.alchemistColorB;
		}

		public int getSecondaryDamage(Player player, OrchidModPlayer modPlayer, int bonusDamage = 0, bool bonusDamageScaling = true)
		{
			int dmg = (int)((this.secondaryDamage + (int)(bonusDamage * (bonusDamageScaling ? this.secondaryScaling : 1f))) * (modPlayer.alchemistDamage));
			return dmg;
		}
	}
}
