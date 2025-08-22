using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public class SuperSpeedCheat : SimpleCheat<PlayerController> {

		protected override IDisposable Apply(PlayerController cheatable) {
			return cheatable.moveSpeed.Scale(3f);
		}

	}
}
