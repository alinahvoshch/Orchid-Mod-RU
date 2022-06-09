using Terraria;

namespace OrchidMod.Alchemist.Misc
{
	public class EmptyFlask : OrchidModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.maxStack = 99;
			Item.value = Item.sellPrice(0, 0, 4, 0);
			Item.rare = 0;
		}


		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Empty Flask");
			Tooltip.SetDefault("Sold by the mineshaft chemist"
							+ "\nUsed to make various alchemist weapons");
		}

	}
}
