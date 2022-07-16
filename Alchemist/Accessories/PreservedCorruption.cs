using Terraria;
using Terraria.ID;

namespace OrchidMod.Alchemist.Accessories
{
	public class PreservedCorruption : OrchidModAlchemistEquipable
	{
		public override void SafeSetDefaults()
		{
			Item.width = 22;
			Item.height = 28;
			Item.value = Item.sellPrice(0, 0, 15, 0);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
		}
		public override void AltSetStaticDefaults()
		{
			DisplayName.SetDefault("Preserved Corruption");
			Tooltip.SetDefault("Maximum potency increased by 2");
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			OrchidAlchemist modPlayer = player.GetModPlayer<OrchidAlchemist>();
			modPlayer.alchemistPotencyMax += 2;
		}
	}
}