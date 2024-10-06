using Terraria;
using Terraria.ID;

namespace OrchidMod.Content.Guardian.Accessories
{
	public class BadgeBattlesPast : OrchidModGuardianItem
	{
		public override void SafeSetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(0, 1, 50, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			OrchidGuardian modPlayer = player.GetModPlayer<OrchidGuardian>();
			modPlayer.GuardianBattlesPast = true;
		}
	}
}