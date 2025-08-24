using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public sealed class FancyHatCheat : SimpleCheat<IHatWearing> {

		public override string DisplayName { get; } = "Fancy Hats";

		protected override IDisposable Apply(IHatWearing cheatable) {
			return cheatable.wearingHat.Override(true);
		}

	}
}
