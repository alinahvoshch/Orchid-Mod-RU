using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Gambler.Weapons.Cards
{
	public class DesertCard : OrchidModGamblerItem
	{
		public override void SafeSetDefaults()
		{
			Item.value = Item.sellPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Blue;
			Item.damage = 11;
			Item.crit = 4;
			Item.knockBack = 0.5f;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.shootSpeed = 10f;
			this.cardRequirement = 1;
			this.gamblerCardSets.Add("Biome");
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Playing Card : Desert");
			Tooltip.SetDefault("Summons a desert bush above your head"
							+ "\nDrag and release a fruit to launch it");
		}

		public override void GamblerShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, bool dummy = false)
		{
			SoundEngine.PlaySound(SoundID.Item1);
			int projType = ProjectileType<Projectiles.DesertCardBase>();
			bool found = false;
			for (int l = 0; l < Main.projectile.Length; l++)
			{
				Projectile proj = Main.projectile[l];
				if (proj.active && proj.type == projType && proj.owner == player.whoAmI && proj.ai[1] != 1f)
				{
					found = true;
					break;
				}
			}
			if (!found) {
				DummyProjectile(Projectile.NewProjectile(source, position, Vector2.Zero, projType, damage, knockback, player.whoAmI), dummy);
			}
		}
	}
}
