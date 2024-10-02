using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Guardian.Armors.Misc
{
	[AutoloadEquip(EquipType.Head)]
	public class GuardianGitHelm: OrchidModGuardianEquipable
	{
		public override void SafeSetDefaults()
		{
			Item.width = 24;
			Item.height = 22;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.defense = 8;
		}

		public override void UpdateEquip(Player player)
		{
			OrchidGuardian modPlayer = player.GetModPlayer<OrchidGuardian>();
			modPlayer.GuardianSlamMax += 1;
			modPlayer.GuardianGuardMax += 1;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return (body.type == ItemID.CopperChainmail && legs.type == ItemID.CopperGreaves)
				|| (body.type == ItemID.TinChainmail && legs.type == ItemID.TinGreaves)
				|| (body.type == ItemID.IronChainmail && legs.type == ItemID.IronGreaves)
				|| (body.type == ItemID.LeadChainmail && legs.type == ItemID.LeadGreaves)
				|| (body.type == ItemID.SilverChainmail && legs.type == ItemID.SilverGreaves)
				|| (body.type == ItemID.TungstenChainmail && legs.type == ItemID.TungstenGreaves)
				|| (body.type == ItemID.GoldChainmail && legs.type == ItemID.GoldGreaves)
				|| (body.type == ItemID.PlatinumChainmail && legs.type == ItemID.PlatinumGreaves);
		}

		public override void UpdateArmorSet(Player player)
		{
			OrchidGuardian modPlayer = player.GetModPlayer<OrchidGuardian>();
			player.setBonus = "Blocks or parries increases movement speed by 30%";
			modPlayer.GuardianGit = true;
		}
	}
}
