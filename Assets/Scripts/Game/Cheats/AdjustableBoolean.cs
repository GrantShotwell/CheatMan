using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Cheats {
	[Serializable]
	public class AdjustableBoolean {

		[SerializeField] private bool _value = false;
		private bool? _override = null;
		IDisposable _overrideObj = null;

		public bool Value {
			get {
				if (_override != null) {
					return _override.Value;
				}
				return _value;
			}
			set {
				_value = value;
			}
		}

		public AdjustableBoolean(bool value) {
			_value = value;
		}

		public IDisposable Override(bool value) {
			return new OverrideReference(this, value);
		}

		private class OverrideReference : IDisposable {
			private readonly AdjustableBoolean _value;
			private bool _disposed = false;

			public OverrideReference(AdjustableBoolean value, bool amount) {
				_value = value;
				value._overrideObj = this;
				value._override = amount;
			}

			public void Dispose() {
				if (_disposed || _value._overrideObj != this) return;
				_value._override = null;
				_value._overrideObj = null;
				_disposed = true;
			}
		}

		public static implicit operator bool(AdjustableBoolean adjustable) => adjustable.Value;

		public override string ToString() => Value.ToString();

	}
}
