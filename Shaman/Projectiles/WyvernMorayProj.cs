using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrchidMod.Common;
using OrchidMod.Content.Trails;
using OrchidMod.Effects;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OrchidMod.Shaman.Projectiles
{
	public class WyvernMorayProj : OrchidModShamanProjectile
	{
		public bool Improved { get => projectile.ai[1] == 1; set => projectile.ai[1] = value.ToInt(); }

		private bool _death = false;
		private float _deathProgress = 1f;
		private PrimitiveTrailSystem.Trail _trail;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wyvern Spit");

			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
		}

		public override void SafeSetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.aiStyle = 2;
			projectile.friendly = true;
			projectile.timeLeft = 150;
			projectile.alpha = 255;
			projectile.penetrate = -1;
		}

		private readonly Color[] _effectColors = new Color[] { new Color(113, 187, 162), new Color(40, 116, 255) };

		public Color GetCurrentColor() => _effectColors[Improved.ToInt()] * _deathProgress;

		public override void OnSpawn()
		{
			_trail = new TriangularTrail(target: projectile, length: 16 * 13, width: (p) => 20 * (1 - p * 0.25f), color: (p) => GetCurrentColor() * (1 - p), effect: EffectsManager.WyvernMorayEffect);
			_trail.SetMaxPoints(35);

			PrimitiveTrailSystem.NewTrail(_trail);
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (Improved) damage += damage;
		}

		public override void AI()
		{
			if (_death) this.DeathUpdate();
			else OrchidModProjectile.resetIFrames(projectile);
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SetSpriteBatch(spriteBatch: spriteBatch, blendState: BlendState.Additive);
			{
				Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
				Texture2D texture;
				Color color = GetCurrentColor();

				// Trail
				{
					texture = OrchidHelper.GetExtraTexture(11);
					for (int k = 1; k < projectile.oldPos.Length; k++)
					{
						float progress = ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
						Vector2 drawPosTrail = projectile.oldPos[k] - Main.screenPosition + projectile.Size * 0.5f + new Vector2(0f, projectile.gfxOffY);
						spriteBatch.Draw(texture, drawPosTrail, null, color * progress, projectile.rotation, texture.Size() * 0.5f, projectile.scale * 0.4f * progress, SpriteEffects.None, 0f);
					}
					spriteBatch.Draw(texture, drawPos, null, color, projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() * 0.5f, projectile.scale * 0.6f, SpriteEffects.None, 0);
				}

				texture = OrchidHelper.GetExtraTexture(13);
				spriteBatch.Draw(texture, drawPos, null, color * 0.8f, Main.GlobalTime * 5f, texture.Size() * 0.5f, projectile.scale * 0.3f, SpriteEffects.None, 0);
				spriteBatch.Draw(OrchidHelper.GetExtraTexture(8), drawPos, null, color * _deathProgress, projectile.velocity.ToRotation() + MathHelper.PiOver2, OrchidHelper.GetExtraTexture(8).Size() * 0.5f, projectile.scale * 0.4f, SpriteEffects.None, 0);
				spriteBatch.Draw(OrchidHelper.GetExtraTexture(3), drawPos + Vector2.Normalize(projectile.velocity) * 8f, null, color * MathHelper.SmoothStep(0, 1, projectile.velocity.Length() * 0.1f), projectile.velocity.ToRotation() + MathHelper.PiOver2, OrchidHelper.GetExtraTexture(3).Size() * 0.5f, projectile.scale * 0.4f, SpriteEffects.None, 0);

				if (_death)
				{
					texture = OrchidHelper.GetExtraTexture(9);
					float progress = 1 - (float)Math.Pow(MathHelper.Lerp(0, 1, _deathProgress), 3);
					color *= progress;
					Vector2 origin = texture.Size() * 0.5f;
					float scale = projectile.scale * progress;

					spriteBatch.Draw(texture, drawPos, null, color * 0.6f, 0f, origin, scale, SpriteEffects.None, 0);
					spriteBatch.Draw(texture, drawPos, null, color, 0f, origin, scale * 1.6f, SpriteEffects.None, 0);
				}
			}
			SetSpriteBatch(spriteBatch: spriteBatch);
		}

		public void DeathUpdate()
		{
			projectile.velocity = Vector2.Zero;
			projectile.timeLeft = 2;

			if (_deathProgress == 1f)
			{
				Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 34);

				projectile.friendly = false;
				projectile.tileCollide = false;
				_trail.StartDissolving();

				var proj = Main.projectile[Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<WyvernMorayProjLingering>(), (int)(projectile.damage * 0.6f), 0.0f, projectile.owner, 0.0f, 0.0f)];
				if (proj.modProjectile is WyvernMorayProjLingering hehe)
				{
					hehe.effectColor = GetCurrentColor();
					hehe.Improved = this.Improved;
					proj.netUpdate = true;
				}
			}

			_deathProgress -= 0.085f;

			if (_deathProgress <= 0f) projectile.Kill();
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = oldVelocity;

			_death = true;
			return false;
		}

		public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit, Player player, OrchidModPlayer modPlayer)
		{
			_death = true;
		}
	}
}