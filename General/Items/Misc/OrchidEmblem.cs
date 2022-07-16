using Terraria;

namespace OrchidMod.General.Items.Misc
{
	public class OrchidEmblem : OrchidModItem
	{
		public override void AltSetStaticDefaults()
		{
			DisplayName.SetDefault("Orchid Emblem");
			Tooltip.SetDefault("While the Shamans can continue their playthrough, the journey for both Alchemists and Gamblers stops here"
							+ "\n... For now !"
							+ "\nContent up to Moon Lord for these classes will be added with the upcoming updates"
							+ "\nPlease, come and say hi on Discord if you want to support me ;)");
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 32;
			Item.maxStack = 1;
			Item.useStyle = 0;
			Item.rare = -11;
			Item.value = Item.sellPrice(0, 1, 0, 0);
		}
	}
}