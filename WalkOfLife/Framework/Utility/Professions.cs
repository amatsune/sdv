﻿using StardewValley;
using StardewValley.Buildings;
using System;
using TheLion.Common.Classes;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		public static int SpelunkerBuffID { get; private set; }
		public static int DemolitionistBuffID { get; private set; }
		public static int BruteBuffID { get; private set; }
		public static int GambitBuffID { get; private set; }

		public enum Professions
		{
			Rancher = Farmer.rancher,
			Breeder = Farmer.butcher,
			Producer = Farmer.shepherd,

			Harvester = Farmer.tiller,
			Oenologist = Farmer.artisan,
			Agriculturist = Farmer.agriculturist,

			Fisher = Farmer.fisher,
			Angler = Farmer.angler,
			Aquarist = Farmer.pirate,

			Trapper = Farmer.trapper,
			Luremaster = Farmer.baitmaster,
			Conservationist = Farmer.mariner,

			Lumberjack = Farmer.forester,
			Arborist = Farmer.lumberjack,
			Tapper = Farmer.tapper,

			Forager = Farmer.gatherer,
			Ecologist = Farmer.botanist,
			Scavenger = Farmer.tracker,

			Miner = Farmer.miner,
			Spelunker = Farmer.blacksmith,
			Prospector = Farmer.burrower,

			Blaster = Farmer.geologist,
			Demolitionist = Farmer.excavator,
			Gemologist = Farmer.gemologist,

			Fighter = Farmer.fighter,
			Brute = Farmer.brute,
			Gambit = Farmer.defender,

			Rascal = Farmer.scout,
			Slimemaster = Farmer.acrobat,
			Desperado = Farmer.desperado
		};

		/// <summary>Bi-directional dictionary for looking-up profession id's by name or name's by id.</summary>
		public static BiMap<string, int> ProfessionMap { get; } = new BiMap<string, int>
		{
			// farming
			{ "rancher", Farmer.rancher },				// 0
			{ "breeder", Farmer.butcher },				// 2 (coopmaster)
			{ "producer", Farmer.shepherd },			// 3

			{ "harvester", Farmer.tiller },				// 1
			{ "oenologist", Farmer.artisan },			// 4
			{ "agriculturist", Farmer.agriculturist },	// 5

			// fishing
			{ "fisher", Farmer.fisher },				// 6
			{ "angler", Farmer.angler },				// 8
			{ "aquarist", Farmer.pirate },				// 9

			{ "trapper", Farmer.trapper },				// 7
			{ "luremaster", Farmer.baitmaster },		// 10
			{ "conservationist", Farmer.mariner },		// 11
			// Note: the game code has mariner and baitmaster ids mixed up

			// foraging
			{ "lumberjack", Farmer.forester },			// 12
			{ "arborist", Farmer.lumberjack },			// 14
			{ "tapper", Farmer.tapper },				// 15

			{ "forager", Farmer.gatherer },				// 13
			{ "ecologist", Farmer.botanist },			// 16
			{ "scavenger", Farmer.tracker },			// 17

			// mining
			{ "miner", Farmer.miner },					// 18
			{ "spelunker", Farmer.blacksmith },			// 20
			{ "prospector", Farmer.burrower },			// 21 (prospector)

			{ "blaster", Farmer.geologist },			// 19
			{ "demolitionist", Farmer.excavator },		// 22
			{ "gemologist", Farmer.gemologist },		// 23

			// combat
			{ "fighter", Farmer.fighter },				// 24
			{ "brute", Farmer.brute },					// 26
			{ "gambit", Farmer.defender },				// 27

			{ "rascal", Farmer.scout },					// 25
			{ "slimetamer", Farmer.acrobat },			// 28
			{ "desperado", Farmer.desperado }			// 29
		};

		/// <summary>Generate unique buff ids from a hash seed.</summary>
		/// <param name="hash">Unique instance hash.</param>
		public static void SetProfessionBuffIDs(int hash)
		{
			SpelunkerBuffID = hash + ProfessionMap.Forward["spelunker"];
			DemolitionistBuffID = hash + ProfessionMap.Forward["demolitionist"];
			BruteBuffID = hash - ProfessionMap.Forward["brute"];
			GambitBuffID = hash - ProfessionMap.Forward["gambit"];
		}

		/// <summary>Whether the local farmer has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool LocalPlayerHasProfession(string professionName)
		{
			return Game1.player.professions.Contains(ProfessionMap.Forward[professionName]);
		}

		/// <summary>Whether a farmer has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="who">The player.</param>
		public static bool SpecificPlayerHasProfession(string professionName, Farmer who)
		{
			return who.professions.Contains(ProfessionMap.Forward[professionName]);
		}

		/// <summary>Whether any farmer in the current multiplayer session has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="numberOfPlayersWithThisProfession">How many players have this profession.</param>
		public static bool AnyPlayerHasProfession(string professionName, out int numberOfPlayersWithThisProfession)
		{
			if (!Game1.IsMultiplayer)
			{
				if (LocalPlayerHasProfession(professionName))
				{
					numberOfPlayersWithThisProfession = 1;
					return true;
				}
			}

			numberOfPlayersWithThisProfession = 0;
			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (player.isActive() && SpecificPlayerHasProfession(professionName, player))
					++numberOfPlayersWithThisProfession;
			}

			return numberOfPlayersWithThisProfession > 0;
		}

		/// <summary>Get the price multiplier for wine sold by Oenologist.</summary>
		/// <param name="who">The player.</param>
		public static float GetOenologistPriceMultiplier(Farmer who)
		{
			if (!who.IsLocalPlayer) return 1f;

			float multiplier = 1f;
			if (AwesomeProfessions.Data.WineFameAccrued >= AwesomeProfessions.Config.WineFameNeededForMaxValue) multiplier += 1f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= (uint)(0.625 * AwesomeProfessions.Config.WineFameNeededForMaxValue)) multiplier += 0.5f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= (uint)(0.25 * AwesomeProfessions.Config.WineFameNeededForMaxValue)) multiplier += 0.25f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= (uint)(0.1 * AwesomeProfessions.Config.WineFameNeededForMaxValue)) multiplier += 0.1f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= (uint)(0.04 * AwesomeProfessions.Config.WineFameNeededForMaxValue)) multiplier += 0.05f;

			return multiplier;
		}

		/// <summary>Get the price multiplier for produce sold by Producer.</summary>
		/// <param name="who">The player.</param>
		public static float GetProducerPriceMultiplier(Farmer who)
		{
			float multiplier = 1f;
			foreach (Building b in Game1.getFarm().buildings)
			{
				if ((b.owner.Equals(who.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b.buildingType.Contains("Deluxe") && (b.indoors.Value as AnimalHouse).isFull())
					multiplier += 0.05f;
			}

			return multiplier;
		}

		/// <summary>Get the price multiplier for fish sold by Angler.</summary>
		/// <param name="who">The player.</param>
		public static float GetAnglerPriceMultiplier(Farmer who)
		{
			float multiplier = 1f;
			foreach (int id in _legendaryFishIds)
			{
				if (who.fishCaught.ContainsKey(id))
					multiplier += 0.05f;
			}

			return multiplier;
		}

		/// <summary>Get the price multiplier for items sold by Conservationist.</summary>
		/// <param name="who">The player.</param>
		public static float GetConservationistPriceMultiplier(Farmer who)
		{
			if (!who.IsLocalPlayer) return 1f;

			return 1f + AwesomeProfessions.Data.OceanTrashCollectedThisSeason % AwesomeProfessions.Config.TrashNeededForNextTaxLevel / 100f;
		}

		/// <summary>Get adjusted friendship for calculating the value of Breeder-owned farm animal.</summary>
		/// <param name="a">Farm animal instance.</param>
		public static double GetProducerAdjustedFriendship(FarmAnimal a)
		{
			return Math.Pow(Math.Sqrt(2) * a.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
		}

		/// <summary>Get the quality of forage for Ecologist.</summary>
		public static int GetEcologistForageQuality()
		{
			return AwesomeProfessions.Data.ItemsForaged < AwesomeProfessions.Config.ForagesNeededForBestQuality ? (AwesomeProfessions.Data.ItemsForaged < AwesomeProfessions.Config.ForagesNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}

		/// <summary>Get the quality of mineral for Gemologist.</summary>
		public static int GetGemologistMineralQuality()
		{
			return AwesomeProfessions.Data.MineralsCollected < AwesomeProfessions.Config.MineralsNeededForBestQuality ? (AwesomeProfessions.Data.MineralsCollected < AwesomeProfessions.Config.MineralsNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}

		/// <summary>Get the bonus ladder spawn chance for Spelunker.</summary>
		public static double GetSpelunkerBonusLadderDownChance()
		{
			return 1.0 / (1.0 + Math.Exp(Math.Log(2.0 / 3.0) / 120.0 * AwesomeProfessions.Data.LowestMineLevelReached)) - 0.5;
		}

		/// <summary>Get the quality for the chosen catch.</summary>
		/// <param name="who">The owner of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		public static int GetTrapperFishQuality(Farmer who, Random r)
		{
			if (!SpecificPlayerHasProfession("trapper", who)) return 0;

			if (r.NextDouble() < who.FishingLevel / 30.0) return 2;

			if (r.NextDouble() < who.FishingLevel / 15.0) return 1;

			return 0;
		}

		/// <summary>Get the bonus bobber bar height for Aquarist.</summary>
		public static int GetAquaristBonusBobberBarHeight()
		{
			if (!LocalPlayerHasProfession("aquarist")) return 0;

			int bonusBobberHeight = 0;
			foreach (Building b in Game1.getFarm().buildings)
			{
				if ((b.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond && (b as FishPond).FishCount >= 10)
					bonusBobberHeight += 7;
			}

			return bonusBobberHeight;
		}

		/// <summary>Get the bonus critical strike chance that should be applied to Gambit.</summary>
		public static float GetBruteBonusDamageMultiplier()
		{
			return (float)(1.0 + AwesomeProfessions.BruteKillStreak * 0.005);
		}

		/// <summary>Get the bonus critical strike chance that should be applied to Gambit.</summary>
		/// <param name="who">The player.</param>
		public static float GetGambitBonusCritChance(Farmer who)
		{
			double healthPercent = (double)who.health / who.maxHealth;
			return (float)(0.2 / (healthPercent + 0.2) - 0.2 / 1.2);
		}

		/// <summary>Get bonus slingshot damage as function of projectile travel distance.</summary>
		/// <param name="travelDistance">Distance travelled by the projectile.</param>
		public static float GetRascalBonusDamageForTravelTime(int travelDistance)
		{
			int maxDistance = 800;
			if (travelDistance > maxDistance) return 1.5f;
			return 0.5f / maxDistance * travelDistance + 1f;
		}

		/// <summary>Whether the player should track a given object.</summary>
		/// <param name="obj">The given object.</param>
		public static bool ShouldPlayerTrackObject(SObject obj)
		{
			return (LocalPlayerHasProfession("scavenger") && ((obj.IsSpawnedObject && !IsForagedMineral(obj)) || obj.ParentSheetIndex == 590))
				|| (LocalPlayerHasProfession("prospector") && (IsResourceNode(obj) || IsForagedMineral(obj)));
		}
	}
}
