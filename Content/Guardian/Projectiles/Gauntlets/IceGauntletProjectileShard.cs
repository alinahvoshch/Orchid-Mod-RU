using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace OrchidMod.Content.Guardian.Projectiles.Gauntlets
{
	public class IceGauntletProjectileShard : OrchidModGuardianProjectile
	{
		public override void SafeSetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 35;
			Projectile.scale = 1f;
			Projectile.penetrate = 3;
			Projectile.tileCollide = true;
			Projectile.alpha = 255;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			if (Projectile.ai[2] == 0f)
			{ // random rotation & scale (init)
				Projectile.rotation = Main.rand.NextFloat(MathHelper.Pi);
				Projectile.ai[0] = Main.rand.Next(3); // Frame
				Projectile.ai[1] = Main.rand.NextBool() ? 1 : -1; // Rotation direction
			}

			Projectile.rotation += Projectile.velocity.Length() * 0.01f * Projectile.ai[1];

			if (Projectile.timeLeft < 20)
			{
				Projectile.velocity *= 0.85f;
			}
		}

		public override bool OrchidPreDraw(SpriteBatch spriteBatch, ref Color lightColor)
		{
			/*
			spriteBatch.End(out SpriteBatchSnapshot spriteBatchSnapshot);
			spriteBatch.Begin(spriteBatchSnapshot with { BlendState = BlendState.Additive });
			*/

			// Draw code here

			Texture2D projTexture = TextureAssets.Projectile[Projectile.type].Value;

			float colorMult = 1f;
			if (Projectile.timeLeft < 10) colorMult *= Projectile.timeLeft / 30f;

			Color color = lightColor * 1.5f * colorMult;
			if (color.A > 128)
			{
				color.A = 128;
			}

			Rectangle drawRectangle = projTexture.Bounds;
			drawRectangle.Height /= 3;
			drawRectangle.Y += (int)Projectile.ai[0];

			Vector2 drawPosition = Projectile.Center - Main.screenPosition;
			spriteBatch.Draw(projTexture, drawPosition, drawRectangle, color, Projectile.rotation, drawRectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

			/*
			spriteBatch.End();
			spriteBatch.Begin(spriteBatchSnapshot);
			*/

			return false;
		}
	}
}