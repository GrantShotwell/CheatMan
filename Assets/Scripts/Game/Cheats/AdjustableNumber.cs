using System;
using UnityEngine;

namespace Game.Cheats {
	[Serializable]
	public class AdjustableNumber {

		[SerializeField] private float _value = 0f;
		private float _scale = 1f;
		private float _offset = 0f;
		private float? _override = null;
		IDisposable _overrideObj = null;

		public float Value {
			get {
				if (_override != null) {
					return _override.Value;
				}
				return (_value * _scale) + _offset;
			}
			set {
				_value = value;
			}
		}

		public AdjustableNumber(float value) {
			_value = value;
		}

		public IDisposable Scale(float value) {
			return new ScaleReference(this, value);
		}

		public IDisposable Offset(float value) {
			return new OffsetReference(this, value);
		}

		public IDisposable Override(float value) {
			return new OverrideReference(this, value);
		}

		public int Floor() {
			return Mathf.FloorToInt(Value);
		}

		public int Ceil() {
			return Mathf.CeilToInt(Value);
		}

		private class ScaleReference : IDisposable {
			private readonly AdjustableNumber _value;
			private readonly float _amount;
			private bool _disposed = false;

			public ScaleReference(AdjustableNumber value, float amount) {
				_value = value;
				_amount = amount;
				value._scale *= amount;
			}

			public void Dispose() {
				if (_disposed) return;
				_value._scale /= _amount;
				_disposed = true;
			}
		}

		private class OffsetReference : IDisposable {
			private readonly AdjustableNumber _value;
			private readonly float _amount;
			private bool _disposed = false;
			public OffsetReference(AdjustableNumber value, float amount) {
				_value = value;
				_amount = amount;
				value._offset += amount;
			}
			public void Dispose() {
				if (_disposed) return;
				_value._offset -= _amount;
				_disposed = true;
			}
		}

		private class OverrideReference : IDisposable {
			private readonly AdjustableNumber _value;
			private readonly float _amount;
			private bool _disposed = false;

			public OverrideReference(AdjustableNumber value, float amount) {
				_value = value;
				_amount = amount;
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

		public static implicit operator float(AdjustableNumber adjustable) => adjustable.Value;

		public override string ToString() => Value.ToString();

	}
}
