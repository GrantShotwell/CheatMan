using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public sealed class FancyBowCheat : SimpleCheat<IBowWearing> {

		public override string DisplayName { get; } = "Fancy Bows";

		protected override IDisposable Apply(IBowWearing cheatable) {
			return cheatable.wearingBow.Override(true);
		}

	}
}
