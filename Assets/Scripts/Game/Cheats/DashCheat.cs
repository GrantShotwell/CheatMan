using Assets.Scripts.Game.Cheats;
using System;

namespace Game.Cheats {
	public sealed class DashCheat : SimpleCheat<PlayerController> {

		public override string DisplayName { get; } = "Dash";

		protected override IDisposable Apply(PlayerController cheatable) {
			return cheatable.dashEnabled.Override(true);
		}

	}
}
