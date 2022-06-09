using Microsoft.Xna.Framework;
using OrchidMod.WorldgenArrays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OrchidMod.NPCs.Town
{
	public class BoundChemist : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bound Chemist");
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 250;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.knockBackResist = 0.5f;
			NPC.width = 28;
			NPC.height = 36;
			NPC.aiStyle = 0;
			NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.friendly = true;
			NPC.rarity = 1;
			NPC.value = Item.buyPrice(0, 0, 0, 0);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = 0;
			NPC.rotation = 0f;
		}

		public override bool CanChat()
		{
			return true;
		}

		public override string GetChat()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				NPC.Transform(NPCType<NPCs.Town.Chemist>());
				OrchidWorld.foundChemist = true;
			}
			return "Thanks, you spared me a lot of troubles right there, or... Let's not talk about it.";
		}

		public override void AI()
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				for (int index = 0; index < Main.player.Length; index++)
				{
					if (Main.player[index].talkNPC > -1)
					{
						if (Main.player[index].active && Main.npc[Main.player[index].talkNPC].type == NPC.type)
						{
							NPC.Transform(NPCType<NPCs.Town.Chemist>());
							OrchidWorld.foundChemist = true;
						}
					}
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			bool foundNPC = (NPC.FindFirstNPC(NPCType<NPCs.Town.Chemist>()) + NPC.FindFirstNPC(NPCType<NPCs.Town.BoundChemist>())) > 0;
			bool inMineshaft = false;
			if (!foundNPC && !OrchidWorld.foundChemist)
			{
				Player player = Main.player[(int)Player.FindClosest(new Vector2(Main.maxTilesX / 2 * 16f, (Main.maxTilesY / 3 + 100) * 16f), 1, 1)];
				int MSMinPosX = (Main.maxTilesX / 2) - ((OrchidMSarrays.MSLenght * 15) / 2) + 10;
				int MSMinPosY = (Main.maxTilesY / 3 + 100) + 10;
				Rectangle rect = new Rectangle(MSMinPosX, MSMinPosY, (OrchidMSarrays.MSLenght * 15) - 20, (OrchidMSarrays.MSHeight * 14) - 20);
				if (rect.Contains(new Point((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f))))
				{
					inMineshaft = true;
				}
			}
			return !OrchidWorld.foundChemist && !foundNPC && inMineshaft ? 5f : 0f;
		}
	}
}
