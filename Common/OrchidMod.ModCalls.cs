using Microsoft.Xna.Framework.Graphics;
using OrchidMod.Common;
using OrchidMod.Content.Alchemist;
using OrchidMod.Content.Gambler;
using OrchidMod.Content.General.NPCs.Town;
using OrchidMod.Content.Guardian;
using OrchidMod.Content.Guardian.Weapons.Gauntlets;
using OrchidMod.Content.Guardian.Weapons.Warhammers;
using OrchidMod.Content.Shapeshifter;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OrchidMod
{
	public partial class OrchidMod
	{
		private static Action<Player> GuardianFocus = new (player =>
		{
			player.statDefense += 3;
			player.lifeRegen += 6;
			OrchidGuardian guardian = player.GetModPlayer<OrchidGuardian>();
			guardian.GuardianGuardMax += 3;
			guardian.GuardianSlamMax += 3;
		});

		private void ThoriumModCalls()
		{
			if (ThoriumMod == null || ThoriumMod.Version < new Version(1, 7, 2, 0)) return;
			ThoriumMod.Call("TerrariumArmorAddClassFocus", ModContent.GetInstance<GuardianDamageClass>(), GuardianFocus, OrchidColors.GuardianTag);
			ThoriumMod.Call("AddMartianItemID", ModContent.ItemType<MartianWarhammer>());
		}

		private void CensusModCalls()
		{
			if (!ModLoader.TryGetMod("Census", out Mod censusMod)) return;

			censusMod.Call
			(
				"TownNPCCondition",
				ModContent.NPCType<Croupier>(),
				ModContent.GetInstance<Croupier>().GetLocalization("Census.SpawnCondition")
			);

			censusMod.Call
			(
				"TownNPCCondition",
				ModContent.NPCType<Chemist>(),
				ModContent.GetInstance<Chemist>().GetLocalization("Census.SpawnCondition")
			);
		}

		private static void ColoredDamageTypeModCalls()
		{
			if (!Main.dedServ && ModLoader.TryGetMod("ColoredDamageTypes", out Mod coloreddamagetypes))
			{
				// Colors in order : Tooltip, Damage, Crit
				coloreddamagetypes.Call("AddDamageType", ModContent.GetInstance<GuardianDamageClass>(), (165, 130, 100), (198, 172, 146), (155, 109, 85));
				coloreddamagetypes.Call("AddDamageType", ModContent.GetInstance<ShapeshifterDamageClass>(), (100, 175, 150), (120, 195, 170), (43, 132, 101));
			}
		}
		private static void RecipeBrowserModCalls()
		{
			if (!Main.dedServ && ModLoader.TryGetMod("RecipeBrowser", out Mod recipeBrowser))
			{
				string weapons = Language.GetTextValue("Mods.RecipeBrowser.RecipeCatalogueFilters.Weapons.Name");
				string opossing = Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.OpossingCategory");

				//Damage types
				recipeBrowser.Call("AddItemCategory", opossing, weapons, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/SilverGauntlet"), new Predicate<Item>(x => x.CountsAsClass(ModContent.GetInstance<GuardianDamageClass>()) && x.damage > 0 && !x.accessory));
				recipeBrowser.Call("AddItemCategory", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.AlchemistCategory"), weapons, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/EmberVial"), new Predicate<Item>(x => x.CountsAsClass(ModContent.GetInstance<AlchemistDamageClass>()) && x.damage > 0 && !x.accessory));
				recipeBrowser.Call("AddItemCategory", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.GamblerCategory"), weapons, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/GamblingChip"), new Predicate<Item>(x => ((x.CountsAsClass(ModContent.GetInstance<GamblerChipDamageClass>()) || (x.CountsAsClass(ModContent.GetInstance<GamblerDamageClass>())) && x.damage > 0 && !x.accessory))));

				recipeBrowser.Call("AddItemCategory", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.ShapeshifterCategory"), weapons, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/SageOwl"), new Predicate<Item>(x => x.CountsAsClass(ModContent.GetInstance<ShapeshifterDamageClass>()) && x.damage > 0 && !x.accessory));

				//Guardian
				recipeBrowser.Call("AddItemFilter", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.GauntletsCategory"), opossing, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/SilverGauntlet"), new Predicate<Item>(item =>item.ModItem is OrchidModGuardianGauntlet));
				recipeBrowser.Call("AddItemFilter", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.HammersCategory"), opossing, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/Warhammer"), new Predicate<Item>(item => item.ModItem is OrchidModGuardianHammer));
				recipeBrowser.Call("AddItemFilter", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.QuarterstaffsCategory"), opossing, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/Quarterstaff"), new Predicate<Item>(item => item.ModItem is OrchidModGuardianQuarterstaff));
				recipeBrowser.Call("AddItemFilter", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.RuneCategory"), opossing, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/HellRune"), new Predicate<Item>(item => item.ModItem is OrchidModGuardianRune));
				recipeBrowser.Call("AddItemFilter", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.ShieldsCategory"), opossing, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/WoodenPavise"), new Predicate<Item>(item => item.ModItem is OrchidModGuardianShield));
				recipeBrowser.Call("AddItemFilter", Language.GetTextValue("Mods.OrchidMod.RecipeBrowserSupport.StandardCategory"), opossing, ModContent.Request<Texture2D>("OrchidMod/Assets/Textures/Icons/CopperStandard"), new Predicate<Item>(item => item.ModItem is OrchidModGuardianStandard));
			}
		}
	}
}
