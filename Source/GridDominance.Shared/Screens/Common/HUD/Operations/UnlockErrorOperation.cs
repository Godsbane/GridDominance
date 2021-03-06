﻿using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using MonoSAMFramework.Portable.ColorHelper;

namespace GridDominance.Shared.Screens.Common.HUD.HUDOperations
{
	class UnlockErrorOperation : HUDTimedElementOperation<UnlockPanel>
	{
		public override string Name => "UnlockErrorOperation";

		private FPoint realPos;

		public UnlockErrorOperation() : base(0.85f)
		{

		}

		protected override void OnStart(UnlockPanel element)
		{
			realPos = element.RelativePosition;

			for (int i = 0; i < 8; i++) element.CharDisp[i].Background = element.CharDisp[i].Background.WithColor(FlatColors.Alizarin);
		}

 		protected override void OnProgress(UnlockPanel element, float progress, InputState istate)
		{
			var off = Vector2.UnitX * (FloatMath.Sin(progress * FloatMath.TAU * 6) * 32) * (1 - FloatMath.FunctionEaseInCubic(progress));

			element.RelativePosition = realPos + off;
			
			element.HUD.Validate();
		}

		protected override void OnEnd(UnlockPanel element)
		{
			element.RelativePosition = realPos;

			element.CharIndex = 0;
			for (int i = 0; i < 8; i++) element.CharDisp[i].Character = ' ';

			for (int i = 0; i < 8; i++) element.CharDisp[i].Background = element.CharDisp[i].Background.WithColor(FlatColors.Clouds);
		}
	}
}