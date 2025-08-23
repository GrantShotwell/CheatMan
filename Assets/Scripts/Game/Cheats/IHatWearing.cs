using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public interface IHatWearing : ICheatable {

		AdjustableBoolean wearingHat { get; }

	}
}
