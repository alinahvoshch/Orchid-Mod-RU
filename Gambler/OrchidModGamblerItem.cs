﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Gambler
{
	public abstract class OrchidModGamblerItem : OrchidModItem
	{
		public int cardRequirement = -1;
		public List<string> gamblerCardSets = new List<string>();

		public virtual void GamblerShoot(Player player, Vector2 position, float speedX, float speedY, int type, int damage, float knockBack, bool dummy = false) { }

		public virtual void SafeSetDefaults() { }

		public sealed override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.melee = false;
			Item.ranged = false;
			Item.magic = false;
			Item.thrown = false;
			Item.summon = false;
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.Item64;
			Item.consumable = true;
			Item.autoReuse = false;

			this.SafeSetDefaults();

			OrchidModGlobalItem orchidItem = Item.GetGlobalItem<OrchidModGlobalItem>();
			orchidItem.gamblerCardRequirement = this.cardRequirement;
			orchidItem.gamblerCardSets = this.gamblerCardSets;
			orchidItem.gamblerShootDelegate = this.GamblerShoot;
		}

		public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
		{
			mult *= player.GetModPlayer<OrchidModPlayer>().gamblerDamage;
		}

		public override void ModifyWeaponCrit(Player player, ref float crit)
		{
			crit += player.GetModPlayer<OrchidModPlayer>().gamblerCrit;
		}

		public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
			if (Main.rand.Next(101) <= ((OrchidModPlayer)player.GetModPlayer(Mod, "OrchidModPlayer")).gamblerCrit)
				crit = true;
			else crit = false;
		}

		public override bool CloneNewInstances => true;
		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player == Main.player[Main.myPlayer])
			{
				OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
				Item[] cards = modPlayer.gamblerCardsItem;
				int count = OrchidModGamblerHelper.getNbGamblerCards(player, modPlayer);
				if (OrchidModGamblerHelper.containsGamblerCard(Item, player, modPlayer) || player.altFunctionUse == 2 || count < this.cardRequirement || count >= 20)
				{
					return false;
				}
				else
				{
					if (OrchidModGamblerHelper.getNbGamblerCards(player, modPlayer) <= 0)
					{
						bool found = false;
						for (int i = 0; i < Main.InventorySlotsTotal; i++)
						{
							Item item = Main.LocalPlayer.inventory[i];
							if (item.type != 0)
							{
								OrchidModGlobalItem orchidItem = item.GetGlobalItem<OrchidModGlobalItem>();
								if (orchidItem.gamblerDeck)
								{
									found = true;
									break;
								}
							}
						}
						if (!found)
						{
							int gamblerDeck = ItemType<Gambler.Decks.GamblerAttack>();
							player.QuickSpawnItem(gamblerDeck, 1);
						}
					}
					Item.useAnimation = 20;
					Item.useTime = 20;
					for (int i = 0; i < 20; i++)
					{
						if (cards[i].type == 0)
						{
							cards[i] = new Item();
							cards[i].SetDefaults(Item.type, true);
							OrchidModGamblerHelper.clearGamblerCardCurrent(player, modPlayer);
							OrchidModGamblerHelper.clearGamblerCardsNext(player, modPlayer);
							modPlayer.gamblerShuffleCooldown = 0;
							modPlayer.gamblerRedraws = 0;
							OrchidModGamblerHelper.drawGamblerCard(player, modPlayer);
							return true;
						}
					}
					//item.TurnToAir();
				}
			}
			return base.CanUseItem(player);
		}

		public override bool? UseItem(Player player)/* Suggestion: Return null instead of false */
		{
			return true;
		}

		// Since cards should not have prefixes, we do so to avoid errors with rarity
		public override bool? PrefixChance(int pre, UnifiedRandom rand) => false;

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
			if (tt != null)
			{
				string[] splitText = tt.Text.Split(' ');
				string damageValue = splitText.First();
				string damageWord = splitText.Last();
				tt.Text = damageValue + " gambling " + damageWord;
			}

			Player player = Main.player[Main.myPlayer];
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			Item[] cards = modPlayer.gamblerCardsItem;
			int count = OrchidModGamblerHelper.getNbGamblerCards(player, modPlayer);
			int diff = this.cardRequirement - count;

			int index = tooltips.FindIndex(ttip => ttip.Mod.Equals("Terraria") && ttip.Name.Equals("Tooltip0"));
			if (index != -1)
			{
				int tagCount = this.gamblerCardSets.Count - 1;
				if (tagCount > -1)
				{
					List<string> alreadyDone = new List<string>();
					string tags = "";
					foreach (string tag in this.gamblerCardSets)
					{
						if (!alreadyDone.Contains(tag))
						{
							tags += alreadyDone.Count > 0 ? ", " : "";
							tags += tag;
							tagCount--;
							alreadyDone.Add(tag);
						}
					}
					tags += alreadyDone.Count > 1 ? " sets" : " set";

					tooltips.Insert(index, new TooltipLine(Mod, "TagsTag", tags)
					{
						OverrideColor = new Color(175, 255, 175)
					});
				}
			}

			tooltips.Insert(1, new TooltipLine(Mod, "CardsNeeded", "Requires " + this.cardRequirement + " cards (Deck : " + count + ")")
			{
				OverrideColor = new Color(255, 200, 100)
			});

			if (OrchidModGamblerHelper.containsGamblerCard(Item, player, modPlayer))
			{
				tooltips.Insert(1, new TooltipLine(Mod, "UseTag", "Currently in your deck")
				{
					OverrideColor = new Color(255, 100, 100)
				});
			}
			else if (count == 20)
			{
				tooltips.Insert(1, new TooltipLine(Mod, "UseTag", "Your deck is full")
				{
					OverrideColor = new Color(255, 100, 100)
				});
			}
			else if (count < this.cardRequirement)
			{
				tooltips.Insert(1, new TooltipLine(Mod, "UseTag", "Requires " + diff + " more cards")
				{
					OverrideColor = new Color(255, 100, 100)
				});
			}
			else
			{
				tooltips.Insert(1, new TooltipLine(Mod, "UseTag", "Use to add to your deck")
				{
					OverrideColor = new Color(255, 200, 100)
				});
			}

			Mod thoriumMod = OrchidMod.ThoriumMod;
			if (thoriumMod != null)
			{
				tooltips.Insert(1, new TooltipLine(Mod, "ClassTag", "-Gambler Class-")
				{
					OverrideColor = new Color(255, 200, 0)
				});
			}

			tt = tooltips.FirstOrDefault(x => x.Name == "Speed" && x.Mod == "Terraria");
			if (tt != null) tooltips.Remove(tt);

			tt = tooltips.FirstOrDefault(x => x.Name == "Consumable" && x.Mod == "Terraria");
			if (tt != null) tooltips.Remove(tt);
		}
	}
}
