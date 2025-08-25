using System;

namespace Assets.Scripts.Game.Cheats {
	public class NoDispose : IDisposable { void IDisposable.Dispose() { } }
}
