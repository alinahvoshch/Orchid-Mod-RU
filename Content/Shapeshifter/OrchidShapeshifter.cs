﻿using Microsoft.Xna.Framework;
using OrchidMod.Common.ModObjects;
using OrchidMod.Content.Shapeshifter;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod
{
	public class OrchidShapeshifter : ModPlayer
	{
		public OrchidPlayer modPlayer;
		public ShapeshifterShapeshiftAnchor ShapeshiftAnchor;
		public OrchidModShapeshifterShapeshift Shapeshift;
		public bool IsShapeshifted => ShapeshiftAnchor != null && ShapeshiftAnchor.Projectile.active;

		public int GetShapeshifterDamage(float damage) => (int)(Player.GetDamage<ShapeshifterDamageClass>().ApplyTo(damage) + Player.GetDamage(DamageClass.Generic).ApplyTo(damage) - damage);
		public int GetShapeshifterCrit(int additionalCritChance = 0) => (int)(Player.GetCritChance<ShapeshifterDamageClass>() + Player.GetCritChance<GenericDamageClass>() + additionalCritChance);
		public float GetShapeshifterMeleeSpeed(float additionalCritChanceMeleeSpeed = 0) => Player.GetTotalAttackSpeed(DamageClass.Melee) + ShapeshifterMeleeSpeedBonus + additionalCritChanceMeleeSpeed;

		// Can be edited by gear


		// Set effects, accessories, misc

		public int ShapeshifterSageFoxSpeed = 0;
		public float ShapeshifterMeleeSpeedBonus = 0f;
		public float ShapeshifterMoveSpeedBonus = 0f; // Additive, Scales logarithmically, can be used as shapeshifter-only alternative to player.movespeed, should be preferred
		public float ShapeshifterMoveSpeedBonusFlat = 0f; // Additive, added to the final movespeed
		public float ShapeshifterMoveSpeedBonusFinal = 0f; // Multiplicative, used before adding ShapeshifterMoveSpeedBonusFlat
		public float ShapeshifterMoveSpeedBonusGrounded = 1f; // Multiplicative, used for effects that increase grounded speed like magiluminescence
		public float ShapeshifterMoveSpeedBonusNotGrounded = 1f; // Multiplicative, used for effects that increase the movespeed of "flying" wildshapes, at all times

		// Dynamic gameplay and UI fields

		public override void HideDrawLayers(PlayerDrawSet drawInfo)
		{
			if (ShapeshiftAnchor != null)
			{
				foreach (var layer in PlayerDrawLayerLoader.DrawOrder)
				{
					layer.Hide();
				}
			}
		}

		public override void Initialize()
		{
			modPlayer = Player.GetModPlayer<OrchidPlayer>();
		}

		public override void ResetEffects()
		{
			if (ShapeshiftAnchor != null && ShapeshiftAnchor.Projectile.active)
			{
				if (Player.mount.Active || Player.grappling[0] >= 0)
				{ // Disable the shapeshift if the player is mounted or uses a hook
					ShapeshiftAnchor.Projectile.Kill();
				}
			}

			// Reset gameplay fields

			ShapeshifterMeleeSpeedBonus = 0f;
			ShapeshifterMoveSpeedBonus = 0f;
			ShapeshifterMoveSpeedBonusFlat = 0f;
			ShapeshifterMoveSpeedBonusFinal = 1f;
			ShapeshifterMoveSpeedBonusGrounded = 1f;
			ShapeshifterMoveSpeedBonusNotGrounded = 1f;
		}

		public override void PostUpdateEquips()
		{
			if (IsShapeshifted)
			{
				Shapeshift.ShapeshiftBuffs(ShapeshiftAnchor.Projectile, ShapeshiftAnchor, Player, this);

				// Cancels some equipment effects to prevent visual & audio issues

				Player.rocketBoots = 0;
				Player.vanityRocketBoots = 0;
				Player.accRunSpeed = 3f;
				Player.ExtraJumps.Clear();
				
				if (Player.wingTime > 0)
				{
					Player.wingTime = 0;
				}

				// Grants stats to make some equipment compatible with shapeshifter

				if (Player.hasMagiluminescence)
				{
					ShapeshifterMoveSpeedBonusGrounded += 0.15f;
				}

				if (Player.shadowArmor)
				{
					ShapeshifterMoveSpeedBonusGrounded += 0.15f;
				}
			}

			// Misc Effects that should be called before Shapeshifter Core mechanics (eg : stat changes that should affect the shapeshifted player)

			if (ShapeshifterSageFoxSpeed > 0)
			{
				ShapeshifterSageFoxSpeed--;
				Player.moveSpeed += ShapeshifterSageFoxSpeed * 0.003f;
			}
		}

		public override void PostUpdate()
		{
			// Shapeshifter core stuff

			if (IsShapeshifted)
			{ // Runs the shapeshift AI and adjust player position accordingly
				Player.width = Shapeshift.ShapeshiftWidth;
				Player.height = Shapeshift.ShapeshiftHeight;

				Projectile projectile = ShapeshiftAnchor.Projectile;

				if (Player.whoAmI == Main.myPlayer)
				{ // Shapeshift inputs
					ShapeshiftAnchor.CheckInputs(Player); // mostly used to sync inputs in mp

					if (Shapeshift.ShapeshiftCanLeftClick(projectile, ShapeshiftAnchor, Player, this))
					{
						Shapeshift.ShapeshiftOnLeftClick(projectile, ShapeshiftAnchor, Player, this);
					}

					if (Shapeshift.ShapeshiftCanRightClick(projectile, ShapeshiftAnchor, Player, this))
					{
						Shapeshift.ShapeshiftOnRightClick(projectile, ShapeshiftAnchor, Player, this);
					}

					if (Shapeshift.ShapeshiftCanJump(projectile, ShapeshiftAnchor, Player, this))
					{
						Shapeshift.ShapeshiftOnJump(projectile, ShapeshiftAnchor, Player, this);
					}
				}

				Shapeshift.ShapeshiftAnchorAI(projectile, ShapeshiftAnchor, Player, this);
				ShapeshiftAnchor.ExtraAI(Player, this);

				Player.velocity = projectile.velocity;
				Player.Center = projectile.Center + projectile.velocity;
			}
			else if (ShapeshiftAnchor != null || Shapeshift != null)
			{ // Failsafe in case the anchor isn't properly killed
				Player.width = Player.defaultWidth;
				Player.height = Player.defaultHeight;
				Shapeshift = null;
				ShapeshiftAnchor = null;
			}

			// Misc Effects that should be called after shapeshifter core mechanics (eg: that depend of the player width and height to be correct)

			if (ShapeshifterSageFoxSpeed > 0)
			{
				if (Main.rand.NextBool((int)(30 - ShapeshifterSageFoxSpeed / 6f) + 1))
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.IceTorch, Scale: Main.rand.NextFloat(1f, 1.4f));
					dust.noGravity = true;
					dust.noLight = true;
				}
			}
		}

		public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
		{
			if (IsShapeshifted && !Player.noKnockback)
			{
				Shapeshift.ShapeshiftOnHitByAnything(hurtInfo, ShapeshiftAnchor.Projectile, ShapeshiftAnchor, Player, this);
				Shapeshift.ShapeshiftOnHitByProjectile(proj, hurtInfo, ShapeshiftAnchor.Projectile, ShapeshiftAnchor, Player, this);

				if (!Player.noKnockback)
				{ // Player knockback on hit
					ShapeshiftAnchor.Projectile.velocity = new Vector2(3f * hurtInfo.HitDirection, -3f);
				}
			}
		}

		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			if (IsShapeshifted)
			{
				Shapeshift.ShapeshiftOnHitByAnything(hurtInfo, ShapeshiftAnchor.Projectile, ShapeshiftAnchor, Player, this);
				Shapeshift.ShapeshiftOnHitByNPC(npc, hurtInfo, ShapeshiftAnchor.Projectile, ShapeshiftAnchor, Player, this);

				if (!Player.noKnockback)
				{ // Player knockback on hit
					ShapeshiftAnchor.Projectile.velocity = new Vector2(3f * hurtInfo.HitDirection, -3f);
				}
			}
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (IsShapeshifted)
			{
				Shapeshift.ShapeshiftModifyHurt(ref modifiers, ShapeshiftAnchor.Projectile, ShapeshiftAnchor, Player, this);
			}
		}

		public override bool FreeDodge(Player.HurtInfo info)
		{
			if (IsShapeshifted)
			{
				return Shapeshift.ShapeshiftFreeDodge(info, ShapeshiftAnchor.Projectile, ShapeshiftAnchor, Player, this);
			}
			return false;
		}
	}
}
