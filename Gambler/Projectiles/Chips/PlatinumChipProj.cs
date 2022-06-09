namespace OrchidMod.Gambler.Projectiles.Chips
{
	public class PlatinumChipProj : OrchidModGamblerProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Platinum Chip");
		}

		public override void SafeSetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.aiStyle = 2;
			Projectile.timeLeft = 250;
			Projectile.penetrate = 3;
		}
	}
}