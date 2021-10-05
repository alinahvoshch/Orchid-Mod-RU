﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.Alchemist.Recipes
{
	public class RecipePotionFlipper : AlchemistHiddenReactionRecipe
	{
		public override void SetDefaults()
		{
			this.level = 1;
			this.name = "Flipper Potion";
			this.description = "Gives 30 seconds of flipper Potion effect";
			this.debuffDuration = 30;
			this.soundType = 2;
			this.soundID = 25;
			this.dust = 15;
			this.buff = 109;
			this.buffDuration = 30;
			
			this.ingredients.Add(ItemType<Alchemist.Weapons.Air.ShiverthornFlask>());
			this.ingredients.Add(ItemType<Alchemist.Weapons.Water.WaterleafFlask>());
		}
		
		
		public override void Reaction(Player player, OrchidModPlayer modPlayer)
		{
		}
	}
}
