using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public sealed class AddHealthCheat : SimpleCheat<PlayerController> {

		public override string DisplayName { get; } = "Heal";

		protected override IDisposable Apply(PlayerController cheatable) {
			cheatable.GiveHealing(10f);
			return new NoDispose();
		}

	}
}
