using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public sealed class FancyBowCheat : SimpleCheat<PlayerController> {

		public override string DisplayName { get; } = "Fancy Bow";

		protected override IDisposable Apply(PlayerController cheatable) {
			return cheatable.enableBow.Override(true);
		}

	}
}
