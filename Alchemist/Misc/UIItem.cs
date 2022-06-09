using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Alchemist.Misc
{
	public class UIItem : OrchidModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 34;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(0, 0, 2, 0);
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = 1;
			Item.noUseGraphic = true;
			Item.rare = 1;
			Item.UseSound = SoundID.Item7;
			Item.shoot = ProjectileType<Alchemist.Projectiles.AlchemistRightClick>();
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			if (player.altFunctionUse == 2)
			{
				return !modPlayer.alchemistSelectUIDisplay && Main.mouseRightRelease;
			} // else {
			  // return !modPlayer.alchemistSelectUIDisplay && Main.mouseLeftRelease;
			  // }
			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			if (!modPlayer.alchemistSelectUIDisplay)
			{
				modPlayer.alchemistSelectUIDisplay = true;
				modPlayer.alchemistSelectUIInitialize = true;
			}
			return true;
		}

		/*
		public override bool UseItem(Player player) {
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			if (!modPlayer.alchemistSelectUIDisplay) {
				modPlayer.alchemistSelectUIDisplay = true;
				modPlayer.alchemistSelectUIInitialize = true;
			}
			return true;
		}
		*/

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Mod thoriumMod = OrchidMod.ThoriumMod;
			if (thoriumMod != null)
			{
				tooltips.Insert(1, new TooltipLine(Mod, "ClassTag", "-Alchemist Class-")
				{
					OverrideColor = new Color(155, 255, 55)
				});
			}
		}

		public override void HoldItem(Player player)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			modPlayer.alchemistSelectUIItem = true;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Alchemist's Cookbook");
			Tooltip.SetDefault("Allows mixing alchemical weapons by clicking"
							+ "\nRight click on an item icon to mix it"
							+ "\nLeft click to launch the attack"
							+ "\nUp to 18 items can be displayed at once");
		}

	}
}
