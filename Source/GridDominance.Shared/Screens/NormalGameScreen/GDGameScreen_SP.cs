﻿using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_SP : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDGameHUD(this);

		public readonly GraphBlueprint WorldBlueprint;

		public override Fraction LocalPlayerFraction => fractionPlayer;

		public GDGameScreen_SP(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff, GraphBlueprint ws) 
			: base(game, gdm, bp, diff, false, false, diff == FractionDifficulty.DIFF_0 && bp.UniqueID == Levels.LEVELID_1_1)
		{
			WorldBlueprint = ws;
		}

		public override void RestartLevel()
		{
			GDOwner.SetLevelScreen(Blueprint, Difficulty, WorldBlueprint, GameSpeedMode);
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetLevelScreen(Blueprint, diff, WorldBlueprint);
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulties, bool playerHasWon, int addPoints)
		{
			((GDGameHUD)HUD).BtnPause.IsEnabled = false;
			((GDGameHUD)HUD).BtnSpeed.IsEnabled = false;

			GameSpeedMode = GameSpeedModes.NORMAL;

			HUD.AddModal(new HUDScorePanel(lvl, profile, newDifficulties, playerHasWon, addPoints), false);
		}

		protected override void TestForGameEndingCondition()
		{
			if (HasFinished) return;

			bool hasPlayer = false;
			bool hasComputer = false;

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				if (cannon is RelayCannon) continue;
				if (cannon is ShieldProjectorCannon) continue;

				if (cannon.Fraction.IsPlayer) hasPlayer = true;
				if (cannon.Fraction.IsComputer) hasComputer = true;
			}

			if (hasPlayer && !hasComputer) EndGame(true, fractionPlayer);
			if (!hasPlayer && hasComputer)
			{
				var winner = Entities
					.Enumerate()
					.OfType<Cannon>()
					.GroupBy(p => p.Fraction)
					.Where(p => !p.Key.IsNeutral)
					.OrderBy(p => p.Count())
					.Last()
					.Key;

				EndGame(false, winner);
			}
		}

		private void EndGame(bool playerWon, Fraction winner)
		{
			HasFinished = true;
			PlayerWon = playerWon;

			if (playerWon)
			{
				var ctime = (int)(LevelTime * 1000);

				int scoreGain = 0;
				HashSet<FractionDifficulty> gains = new HashSet<FractionDifficulty>();

				for (FractionDifficulty diff = FractionDifficulty.DIFF_0; diff <= Difficulty; diff++)
				{
					if (!GDOwner.Profile.GetLevelData(Blueprint.UniqueID).HasCompletedOrBetter(diff))
					{
						scoreGain += FractionDifficultyHelper.GetScore(diff);
						gains.Add(diff);
					}
				}

				{
					if (!GDOwner.Profile.GetLevelData(Blueprint.UniqueID).HasCompletedExact(Difficulty))
					{
						GDOwner.Profile.SetCompleted(Blueprint.UniqueID, Difficulty, ctime, true);
					}
					var localdata = GDOwner.Profile.LevelData[Blueprint.UniqueID].Data[Difficulty];

					if (ctime < localdata.BestTime)
					{
						// update PB
						GDOwner.Profile.SetCompleted(Blueprint.UniqueID, Difficulty, ctime, true);
					}

					// Fake the online data until next sync
					localdata.GlobalCompletionCount++;
					if (ctime < localdata.GlobalBestTime || localdata.GlobalBestTime == -1)
					{
						localdata.GlobalBestTime = ctime;
						localdata.GlobalBestUserID = GDOwner.Profile.OnlineUserID;
					}
				}

				GDOwner.SaveProfile();
				ShowScorePanel(Blueprint, GDOwner.Profile, gains, true, scoreGain);
				MainGame.Inst.GDSound.PlayEffectGameWon();

				EndGameConvert(winner);
			}
			else
			{
				ShowScorePanel(Blueprint, GDOwner.Profile, new HashSet<FractionDifficulty>(), false, 0);

				MainGame.Inst.GDSound.PlayEffectGameOver();

				EndGameConvert(winner);
			}

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				cannon.ForceUpdateController();
			}
		}

		public override void ExitToMap()
		{
			MainGame.Inst.SetWorldMapScreenZoomedOut(WorldBlueprint, Blueprint);
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			if (HasFinished)
			{
				if (PlayerWon)
					return new EndGameAutoPlayerController(this, cannon, f);
				else
					return new EndGameAutoComputerController(this, cannon, f);
			}
			
			switch (f.Type)
			{
				case FractionType.PlayerFraction:
					return new PlayerController(this, cannon, f);

				case FractionType.ComputerFraction:
#if DEBUG
					if (DebugSettings.Get("ControlEnemies")) return new PlayerController(this, cannon, f);
#endif
					return cannon.CreateKIController(this, f);

				case FractionType.NeutralFraction:
#if DEBUG
					if (DebugSettings.Get("ControlEnemies")) return new PlayerController(this, cannon, f);
#endif
					return cannon.CreateNeutralController(this, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicLevel(Levels.WORLD_NUMBERS[WorldBlueprint.ID]);
		}
	}
}
