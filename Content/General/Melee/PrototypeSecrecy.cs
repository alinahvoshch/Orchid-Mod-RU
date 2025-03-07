﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrchidMod.Assets;
using OrchidMod.Common;
using OrchidMod.Common.ModObjects;
using OrchidMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Content.General.Melee
{
	public class PrototypeSecrecy : ModItem
	{
		public static readonly SoundStyle MagicSound = new(OrchidAssets.SoundsPath + "Magic_1");

		// ...

		public override string Texture => OrchidAssets.ItemsPath + Name;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Prototype Secrecy");
			// Tooltip.SetDefault("Damaging to the back guarantees a critical hit");
		}

		public override void SetDefaults()
		{
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shootSpeed = 8f;
			Item.shoot = ModContent.ProjectileType<PrototypeSecrecyProjectile>();
			Item.damage = 20;
			Item.knockBack = 9f;
			Item.width = 20;
			Item.height = 40;
			Item.UseSound = MagicSound;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.noUseGraphic = true;
			Item.rare = ItemRarityID.Cyan;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.DamageType = DamageClass.Melee;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			Lighting.AddLight(Item.Center, PrototypeSecrecyProjectile.EffectColor.ToVector3() * 0.2f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			spriteBatch.DrawSimpleItemGlowmaskInWorld(Item, Color.White * 0.7f, rotation, scale);
		}

		/* Made it a dev item for S-PLAD
		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ItemID.EnchantedBoomerang, 1);
			recipe.AddIngredient(ItemID.LifeCrystal, 1);
			recipe.AddIngredient(ItemID.Silk, 2);
			recipe.Register();
		}
		*/

		public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[ModContent.ProjectileType<PrototypeSecrecyProjectile>()] <= 1; // We need exactly 2, not 1
	}

	public class PrototypeSecrecyProjectile : OrchidModProjectile
	{
		public static readonly SoundStyle MagicSound = new(OrchidAssets.SoundsPath + "Magic_0");
		public static readonly Color EffectColor = new(224, 39, 83);

		// ...

		public override string Texture => OrchidAssets.ItemsPath + nameof(PrototypeSecrecy);

		public override void AltSetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.aiStyle = 3;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
		}

		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, EffectColor.ToVector3() * 0.2f);

			if (Main.rand.NextBool(3))
			{
				var dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, 0f, 0f)];
				dust.noLight = true;
				dust.noGravity = true;
			}
		}

		public override bool OrchidPreDraw(SpriteBatch spriteBatch, ref Color lightColor)
		{
			var texture = TextureAssets.Projectile[Projectile.type].Value;
			var position = Projectile.position + Projectile.Size * 0.5f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
			Main.EntitySpriteDraw(texture, position, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

			texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			Main.EntitySpriteDraw(texture, position, null, Color.White * 0.7f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			bool flag = false;
			flag |= target.direction > 0 && Projectile.Center.X < target.Center.X && Projectile.velocity.X > 0;
			flag |= target.direction < 0 && Projectile.Center.X > target.Center.X && Projectile.velocity.X < 0;

			if (Projectile.Center.Distance(Owner.Center) > (Projectile.Center + Projectile.velocity).Distance(Owner.Center))
			{
				modifiers.SetCrit();
				SoundEngine.PlaySound(MagicSound, Projectile.Center);
			}

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PrototypeSecrecyHitProjectile>(), 0, 0f, Projectile.owner, flag.ToInt());
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = -oldVelocity;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PrototypeSecrecyHitProjectile>(), 0, 0f, Projectile.owner, 0);

			return base.OnTileCollide(oldVelocity);
		}
	}

	public class PrototypeSecrecyHitProjectile : OrchidModProjectile
	{
		public override string Texture => OrchidAssets.InvisiblePath;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("");
		}

		public override void AltSetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.damage = 0;

			Projectile.friendly = true;
			Projectile.timeLeft = 20;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Projectile.rotation += Main.rand.NextFloat(MathHelper.TwoPi);

			for (int i = 0; i < 8; i++)
			{
				var dust = Dust.NewDustPerfect(Projectile.Center, 60, Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(0.5f, 2.5f), 0, default, Main.rand.NextFloat(0.5f, 1.2f));
				dust.noLight = true;
			}
		}

		public override void AI()
		{
			Projectile.friendly = Projectile.timeLeft == 10;
			Projectile.rotation += 0.05f;
			Projectile.scale = OrchidUtils.MultiLerp<float>(MathHelper.Lerp, 1 - Projectile.timeLeft / 20f, new float[] { 1f, 1.2f, 0.6f, 0f }) * (Projectile.ai[0] > 0f ? 2.5f : 1f);

			Lighting.AddLight(Projectile.Center, PrototypeSecrecyProjectile.EffectColor.ToVector3() * 0.25f * Projectile.scale);
		}

		public override bool? CanCutTiles()
			=> false;
		public override bool? CanDamage()
			=> false;
	}
}