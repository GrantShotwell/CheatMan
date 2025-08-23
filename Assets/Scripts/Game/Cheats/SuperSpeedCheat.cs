using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public class SuperSpeedCheat : SimpleCheat<PlayerController> {

		public override string DisplayName { get; } = "Super Speed";

		protected override IDisposable Apply(PlayerController cheatable) {
			return cheatable.moveSpeed.Scale(3f);
		}

	}
}
