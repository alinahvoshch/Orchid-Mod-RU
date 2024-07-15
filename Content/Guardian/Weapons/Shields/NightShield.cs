﻿using Microsoft.Xna.Framework;
using OrchidMod.Content.Guardian.Projectiles.Shields;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Guardian.Weapons.Shields
{
	public class NightShield : OrchidModGuardianShield
	{

		public override void SafeSetDefaults()
		{
			Item.value = Item.sellPrice(0, 2, 40, 50);
			Item.width = 34;
			Item.height = 42;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item73;
			Item.knockBack = 8f;
			Item.damage = 72;
			Item.rare = ItemRarityID.Orange;
			Item.useTime = 55;
			distance = 45f;
			slamDistance = 80f;
			blockDuration = 180;
		}

		public override void Slam(Player player, Projectile shield)
		{
			Projectile anchor = GetAnchor(player).Projectile;
			int type = ModContent.ProjectileType<NightShieldProjAlt>();
			Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center) * 10f;
			Projectile.NewProjectile(Item.GetSource_FromThis(), anchor.Center, dir, type, (int)(shield.damage * 0.8f), Item.knockBack, player.whoAmI);
		}
	}
}
