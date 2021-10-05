﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Alchemist.Recipes
{
	public class RecipePotionBuilder : AlchemistHiddenReactionRecipe
	{
		public override void SetDefaults()
		{
			this.level = 1;
			this.name = "Builder Potion";
			this.description = "Gives 30 seconds of builder Potion effect";
			this.debuffDuration = 30;
			this.soundType = 2;
			this.soundID = 25;
			
			this.ingredients.Add(ItemType<Alchemist.Weapons.Fire.BlinkrootFlask>());
			this.ingredients.Add(ItemType<Alchemist.Weapons.Air.ShiverthornFlask>());
			this.ingredients.Add(ItemType<Alchemist.Weapons.Nature.MoonglowFlask>());
		}
		
		
		public override void Reaction(Player player, OrchidModPlayer modPlayer)
		{
			player.AddBuff(107, 60 * 30); // Builder
			for (int i = 0; i < 10; i++)
			{
				int dust = Dust.NewDust(player.Center, 10, 10, 15);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 2f;
				Main.dust[dust].scale *= 1.5f;
			}
		}
	}
}
