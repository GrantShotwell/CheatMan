using Assets.Scripts.Game.Cheats;
using System;

namespace Game.Cheats {
	public sealed class DashCheat : SimpleCheat<PlayerController> {

		protected override IDisposable Apply(PlayerController cheatable) {
			return cheatable.dashEnabled.Override(true);
		}

	}
}
