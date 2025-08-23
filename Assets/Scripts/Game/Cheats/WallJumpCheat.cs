using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public class WallJumpCheat : SimpleCheat<PlayerController> {

		public override string DisplayName { get; } = "Wall Jump";

		protected override IDisposable Apply(PlayerController cheatable) {
			return cheatable.wallJumpEnabled.Override(true);
		}

	}
}
