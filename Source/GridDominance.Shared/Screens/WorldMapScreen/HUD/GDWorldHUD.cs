﻿using System.Threading.Tasks;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	public class GDWorldHUD : GameHUD, ISettingsOwnerHUD
	{
		public GDWorldMapScreen GDOwner => (GDWorldMapScreen)Screen;

		public LevelNode SelectedNode = null;

		public readonly TopLevelDisplay TopLevelDisplay;
		public readonly InformationDisplay InfoDisplay;
		public readonly SettingsButton Settings;
		public readonly ScoreDisplay ScoreDisplay;

		public GDWorldHUD(GDWorldMapScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(Settings = new SettingsButton());
			AddElement(ScoreDisplay = new ScoreDisplay(false));
			AddElement(new MultiplayerScoreDisplay(ScoreDisplay, false));
			AddElement(TopLevelDisplay = new TopLevelDisplay());
			AddElement(InfoDisplay = new InformationDisplay());
		}

		public void SelectNode(LevelNode n)
		{
			SelectedNode = n;
			InfoDisplay.ResetCycle();
			
			foreach (var node in GDOwner.GetEntities<LevelNode>())
			{
				var s = node.StateSum;
				if (node != n && (s == BistateProgress.Open || s == BistateProgress.Opening || s == BistateProgress.Undefined)) node.CloseNode();
			}
		}

		public void ShowAccountPanel()
		{
			var profile = MainGame.Inst.Profile;

			SelectNode(null);
			Settings.Close();

			if (profile.AccountType == AccountType.Local)
			{
				CreateUserAndShowAnonPanel().EnsureNoError();
			}
			else if (profile.AccountType == AccountType.Anonymous)
			{
				AddModal(new AnonymousAccountPanel(), true);
			}
			else if (profile.AccountType == AccountType.Full)
			{
				AddModal(new FullAccountPanel(), true);
			}
		}

		public void ShowHighscorePanel()
		{
			SelectNode(null);
			Settings.Close();

			AddModal(new HighscorePanel(GDOwner.GraphBlueprint, false), true);
		}

		public void ShowAboutPanel()
		{
			SelectNode(null);
			Settings.Close();

			AddModal(new InfoPanel(), true);
		}

		private async Task CreateUserAndShowAnonPanel()
		{
			var waitDialog = new HUDIconMessageBox
			{
				L10NText = L10NImpl.STR_GLOB_WAITFORSERVER,
				TextColor = FlatColors.TextHUD,
				Background = HUDBackgroundDefinition.CreateRounded(FlatColors.BelizeHole, 16),

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCogBig,
				RotationSpeed = 1f,

				CloseOnClick = false,
			};

			MainGame.Inst.DispatchBeginInvoke(() => { AddModal(waitDialog, false, 0.7f); });

			await MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile);

			waitDialog.Remove();

			MainGame.Inst.DispatchBeginInvoke(() =>
			{
				if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous)
				{
					AddModal(new AnonymousAccountPanel(), true);
				}
			});
		}

#if DEBUG
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			root.IsVisible = !DebugSettings.Get("HideHUD");
		}
#endif
	}
}
