using Terraria;

namespace OrchidMod.Shaman.Accessories
{
	public class WaxyVial : OrchidModShamanEquipable
	{
		public override void SafeSetDefaults()
		{
			Item.width = 28;
			Item.height = 26;
			Item.value = Item.sellPrice(0, 0, 35, 0);
			Item.rare = 2;
			Item.accessory = true;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Waxy Incense");
			Tooltip.SetDefault("Your shamanic earth bonds will cover you in honey"
							 + "\nYou have a chance to release harmful bees when under the effect of shamanic earth bonds");
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			OrchidModPlayer modPlayer = player.GetModPlayer<OrchidModPlayer>();
			modPlayer.shamanHoney = true;
		}
	}
}
