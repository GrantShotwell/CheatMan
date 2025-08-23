using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public class UnlimitedJumpCheat : SimpleCheat<PlayerController> {

		public override string DisplayName { get; } = "Unlimited Jump";

		protected override IDisposable Apply(PlayerController cheatable) {
			return cheatable.jumpCountMax.Offset(float.PositiveInfinity);
		}

	}
}
