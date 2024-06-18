using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrchidMod.Common.Global.Projectiles;
using OrchidMod.Common.ModObjects;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Content.Gambler.Projectiles
{
	public class SkyCardBase : OrchidModGamblerProjectile
	{
		private int count = 0;
		public Vector2 bushLeftPos;
		public Vector2 bushRightPos;
		public Texture2D bushTexture;
		public Texture2D fruitTexture;
		public Texture2D fruitTextureOutline;
		public Texture2D trajectoryTexture;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sky Card Base");
		}

		public override void SafeSetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.friendly = false;
			Projectile.aiStyle = 0;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 3600;
			Projectile.alpha = 255;
			count = 3;
		}
		
		public override void OnSpawn(IEntitySource source)
		{
			Projectile.ai[0] = 10;
			bushTexture ??= ModContent.Request<Texture2D>("OrchidMod/Content/Gambler/Projectiles/SkyCardBase", AssetRequestMode.ImmediateLoad).Value;
			fruitTextureOutline ??= ModContent.Request<Texture2D>("OrchidMod/Content/Gambler/Projectiles/SkyCardProj_Outline", AssetRequestMode.ImmediateLoad).Value;
			fruitTexture ??= ModContent.Request<Texture2D>("OrchidMod/Content/Gambler/Projectiles/SkyCardProj", AssetRequestMode.ImmediateLoad).Value;
			trajectoryTexture = ModContent.Request<Texture2D>("OrchidMod/Content/Gambler/UI/Textures/Trajectory", AssetRequestMode.ImmediateLoad).Value;
		}

		public override void SafeAI()
		{
			Player player = Main.player[Projectile.owner];
			OrchidGambler modPlayer = player.GetModPlayer<OrchidGambler>();
			bool dummy = Projectile.GetGlobalProjectile<OrchidGlobalProjectile>().gamblerDummyProj;
			
			bushLeftPos = Projectile.Center;
			bushRightPos = Projectile.Center;
			bushLeftPos.X -= (bushTexture.Width / 4);
			bushRightPos.X += (bushTexture.Width / 4);
			bushLeftPos.Y -= ((Math.Abs(modPlayer.modPlayer.timer120 - 60)) / 10) - (bushTexture.Height / 12) - 8;
			bushRightPos.Y -= ((Math.Abs(modPlayer.modPlayer.timer120 - 60) * -1f) / 10) - (bushTexture.Height / 12 * -1f) - 8;
			
			Projectile.ai[0] --;
			if (Projectile.ai[0] <= 0) {
				count++;
				Projectile.ai[0] = 90f;
				int projType = ProjectileType<Content.Gambler.Projectiles.SkyCardProj>();
				int newProjectile = (DummyProjectile(Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 10f, 0f, projType, Projectile.damage, Projectile.knockBack, Projectile.owner), dummy));
				Main.projectile[newProjectile].ai[0] = Projectile.whoAmI;
				Main.projectile[newProjectile].ai[1] = 0f;
				if (count == 4)
				{
					Main.projectile[newProjectile].frame = 1;
					count = 0;
				}
				Main.projectile[newProjectile].netUpdate = true;
			}

			if (Projectile.ai[1] == 0f)
			{
				if (Main.myPlayer == Projectile.owner)
				{
					int cardType = this.getCardType(modPlayer);
					if (cardType != ItemType<Gambler.Weapons.Cards.SkyCard>() || modPlayer.gamblerShuffleCooldown <= 0 && !dummy || dummy && !modPlayer.GamblerDummyInHand)
					{
						if (!modPlayer.gamblerLuckySprout)
						{
							Projectile.Kill();
						}
						else
						{
							Projectile.ai[1] = 1f;
							Projectile.netUpdate = true;
						}
					}
				}

				Projectile.position = player.Center;
				Projectile.position.Y -= 60;
				Projectile.position.X -= (int)(Projectile.width / 2);
			}
		}
		
		public override bool OrchidPreDraw(SpriteBatch spriteBatch, Color lightColor) {
			Vector2 position = bushLeftPos - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			spriteBatch.Draw(bushTexture, position, null, lightColor, 0f, bushTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			position = bushRightPos - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			spriteBatch.Draw(bushTexture, position, null, lightColor, 0f, bushTexture.Size() * 0.5f, 1f, SpriteEffects.FlipHorizontally, 0f);
			
			int projType = ProjectileType<Content.Gambler.Projectiles.SkyCardProj>();
			for (int l = 0; l < Main.projectile.Length; l++)
			{
				Projectile proj = Main.projectile[l];
				if (proj.active && proj.type == projType && proj.ai[1] < 2f && proj.ai[0] == Projectile.whoAmI)
				{
					position = proj.position - Main.screenPosition;
					Rectangle newBounds = fruitTexture.Bounds;
					newBounds.Height /= 2;
					newBounds.Y += proj.frame * newBounds.Height;
					position.X -= (newBounds.Width - proj.width) / 2f;
					position.Y -= (newBounds.Height - proj.height) / 2f;
					float lightMult = 0.25f + Math.Abs((1f * Main.player[Main.myPlayer].GetModPlayer<OrchidPlayer>().timer120 - 60) / 90f);
					Color yellow = Color.Yellow * 0.5f;
					Color newLightColor = new Color(yellow.R + lightColor.R, yellow.G + lightColor.G, yellow.B + lightColor.B);
					spriteBatch.Draw(fruitTextureOutline, position, newBounds, lightColor * lightMult, proj.rotation, Vector2.Zero, proj.scale, SpriteEffects.None, 0f);
					spriteBatch.Draw(fruitTexture, position, newBounds, newLightColor, proj.rotation, Vector2.Zero, proj.scale, SpriteEffects.None, 0f);

					if (proj.ai[1] == 1f) {
						Vector2 newMove = Projectile.Center - proj.Center;
						if (newMove.Length() > 1f) {
							newMove.Normalize();
							newMove *= proj.localAI[1];
							Vector2 pos = proj.Center - Main.screenPosition;
							Color drawColor = Color.White;
							for (int i = 0 ; i < (proj.frame == 1 ? 40 : 61); i ++) {
								if (i % 10 == 0) {
									drawColor *= 0.8f;
									Vector2 drawpos = pos - new Vector2(trajectoryTexture.Width / 2, trajectoryTexture.Height / 2);
									spriteBatch.Draw(trajectoryTexture, drawpos, drawColor);
								}
								pos += newMove;
							}
						}
					}
				}
			}
			return true;
		}
	}
}