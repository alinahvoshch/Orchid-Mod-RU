using Microsoft.Xna.Framework;
using OrchidMod.Common;
using OrchidMod.Common.Attributes;
using OrchidMod.Common.Global.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Gambler
{
	[ClassTag(ClassTags.Gambler)]
	public class GamblerDummyTest : ModItem
	{
		public override void SetDefaults()
		{
			Item.DamageType = ModContent.GetInstance<GamblerDamageClass>();
			Item.noMelee = true;
			Item.maxStack = 1;
			Item.width = 34;
			Item.height = 34;
			Item.useStyle = 1;
			Item.noUseGraphic = true;
			//item.UseSound = SoundID.Item7;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.knockBack = 1f;
			Item.damage = 1;
			Item.rare = -11;
			Item.shootSpeed = 1f;
			Item.shoot = 1;
			Item.autoReuse = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			OrchidGambler modPlayer = player.GetModPlayer<OrchidGambler>();
			Item currentCard = modPlayer.gamblerCardDummy;
			if (modPlayer.GetNbGamblerCards() > 0)
			{
				if (player.altFunctionUse == 2 || modPlayer.gamblerCardDummy.type == 0)
				{
					SoundEngine.PlaySound(SoundID.Item64, player.position);
					modPlayer.DrawDummyCard();
					currentCard = modPlayer.gamblerCardDummy;
					CheckStats(currentCard);
					Color floatingTextColor = new Color(255, 200, 0);
					CombatText.NewText(player.Hitbox, floatingTextColor, modPlayer.gamblerCardDummy.Name);
					return false;
				}
			}
			else
			{
				return false;
			}

			currentCard = modPlayer.gamblerCardDummy;
			CheckStats(currentCard);
			currentCard.GetGlobalItem<OrchidGlobalItemPerEntity>().gamblerShootDelegate(player, source, position, velocity, damage, knockback, true);
			return false;
		}

		public override void HoldItem(Player player)
		{
			OrchidGambler modPlayer = player.GetModPlayer<OrchidGambler>();
			modPlayer.GamblerDeckInHand = true;
			modPlayer.GamblerDummyInHand = true;
			if (Main.mouseLeft)
			{
				modPlayer.ShootBonusProjectiles(true);
			}
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player == Main.LocalPlayer)
			{
				if (player.altFunctionUse == 2)
				{
					Item.useAnimation = 20;
					Item.useTime = 20;
					Item.reuseDelay = 0;
				}
			}
			return base.CanUseItem(player);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.player[Main.myPlayer];
			OrchidGambler modPlayer = player.GetModPlayer<OrchidGambler>();
			Item currentCard = modPlayer.gamblerCardCurrent;

			if (currentCard.type != ItemID.None)
			{
				int index = tooltips.FindIndex(ttip => ttip.Mod.Equals("Terraria") && ttip.Name.Equals("Tooltip0"));
				if (index != -1)
				{
					Color textColor = new Color(255, 200, 0); // Rarity Color ???
					string text = Language.GetTextValue("Mods.OrchidMod.Misc.CurrentСard") + $"[c/{Terraria.ID.Colors.AlphaDarken(textColor).Hex3()}:{currentCard.HoverName.Replace(Language.GetTextValue("Mods.OrchidMod.Misc.PlayingСard"), "")}]";

					tooltips.Insert(index, new TooltipLine(Mod, "CardType", text));
				}
			}
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gambler Test Card");
			/* Tooltip.SetDefault("Allows the use of specific gambler cards"
							+ "\nRight click to cycle through your deck"
							+ "\n[c/FF0000:Test Item]"); */
		}

		public void CheckStats(Item currentCard)
		{
			if (currentCard.type != ItemID.None)
			{
				Item.damage = currentCard.damage;
				//item.rare = currentCard.rare;
				Item.crit = currentCard.crit;
				Item.useAnimation = currentCard.useAnimation;
				Item.useTime = currentCard.useTime;
				Item.reuseDelay = currentCard.reuseDelay;
				Item.knockBack = currentCard.knockBack;
				Item.shootSpeed = currentCard.shootSpeed;
				Item.channel = currentCard.channel;
			}
			else
			{
				Item.damage = 0;
				//Item.rare = ItemRarityID.White;
				Item.crit = 0;
				Item.useAnimation = 1;
				Item.useTime = 1;
				Item.reuseDelay = 1;
				Item.knockBack = 1f;
				Item.shootSpeed = 1f;
				Item.channel = false;
			}
		}
	}
}
