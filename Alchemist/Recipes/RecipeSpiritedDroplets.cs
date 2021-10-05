﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Alchemist.Recipes
{
	public class RecipeSpiritedDroplets : AlchemistHiddenReactionRecipe
	{
		public override void SetDefaults()
		{
			this.level = 3;
			this.name = "Spirited Droplets";
			this.description = "Chemical attacks will release spirited water flames";
			this.debuffDuration = 20;
			this.soundType = 2;
			this.soundID = 85;
			
			this.ingredients.Add(ItemType<Alchemist.Weapons.Water.DungeonFlask>());
			this.ingredients.Add(ItemType<Alchemist.Weapons.Fire.GunpowderFlask>());
			this.ingredients.Add(ItemType<Alchemist.Weapons.Nature.AttractiteFlask>());
		}
		
		
		public override void Reaction(Player player, OrchidModPlayer modPlayer)
		{
			player.AddBuff(BuffType<Alchemist.Buffs.SpiritedWaterBuff>(), 60 * 60);
			for (int i = 0; i < 10; i++)
			{
				int dust = Dust.NewDust(player.Center, 10, 10, 29);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 2f;
				Main.dust[dust].scale *= 1.5f;
			}
		}
	}
}
