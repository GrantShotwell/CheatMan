using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Cheats {
	public struct CheatActivation {
		public ICheat Cheat { get; }
		public float DurationSeconds { get; }

		public CheatActivation(ICheat cheat, float durationSeconds) {
			Cheat = cheat ?? throw new ArgumentNullException(nameof(cheat));
			DurationSeconds = durationSeconds;
		}
	}
}
