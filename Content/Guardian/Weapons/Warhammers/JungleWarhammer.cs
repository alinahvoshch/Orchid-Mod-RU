﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.Guardian.Weapons.Warhammers
{
	public class JungleWarhammer : OrchidModGuardianHammer
	{

		public override void SafeSetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
			Item.value = Item.sellPrice(0, 0, 54, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.knockBack = 10f;
			Item.shootSpeed = 10f;
			Item.damage = 82;
			range = 35;
			blockStacks = 1;
		}

		public override bool ThrowAI(Player player, OrchidGuardian guardian, Projectile projectile, bool weak)
		{
			if (Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.JunglePlants, Scale: Main.rand.NextFloat(0.8f, 1f));
				dust.velocity = dust.velocity * 0.25f + projectile.velocity * 0.2f;
				dust.noGravity = true;
			}
			return true;
		}


		public override void OnThrowHitFirst(Player player, OrchidGuardian guardian, NPC target, Projectile projectile, float knockback, bool crit, bool Weak)
		{
			for (int i = 0; i < 2 + Main.rand.Next(2); i ++)
			{
				Vector2 vel = Vector2.Normalize(projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45f)));
				Projectile newProjectile = Projectile.NewProjectileDirect(projectile.GetSource_FromAI(), projectile.Center + vel * 3f, vel * 7f, 976, (int)(projectile.damage * 0.5f), 1f, projectile.owner);
				newProjectile.DamageType = ModContent.GetInstance<GuardianDamageClass>();
			}
		}

		public override void OnThrowHit(Player player, OrchidGuardian guardian, NPC target, Projectile projectile, float knockback, bool crit, bool Weak)
		{
			SoundEngine.PlaySound(SoundID.Grass, projectile.Center);

			if (Main.rand.NextBool())
			{
				Vector2 vel = Vector2.Normalize(projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45f)));
				Projectile newProjectile = Projectile.NewProjectileDirect(projectile.GetSource_FromAI(), projectile.Center + vel * 3f, vel * 7f, 976, (int)(projectile.damage * 0.5f), 1f, projectile.owner);
				newProjectile.DamageType = ModContent.GetInstance<GuardianDamageClass>();
			}

			target.AddBuff(BuffID.Poisoned, 180);
		}

		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ItemID.Stinger, 12);
			recipe.AddIngredient(ItemID.JungleSpores, 15);
			recipe.AddIngredient(ItemID.Vine, 3);
			recipe.Register();
		}
	}
}
