using System;
using System.Collections.Generic;

#nullable enable

namespace Game.Cheats {
	public class AdjustmentDictionary<T> where T : ICheatable {

		private readonly Dictionary<T, AdjustmentBox> _adjustments = new();

		private Func<T, IDisposable> Apply { get; set; }

		public bool Enabled { get; set; } = false;

		public AdjustmentDictionary(Func<T, IDisposable> apply) {
			Apply = apply;
		}

		public void Add(T item) {
			if (_adjustments.ContainsKey(item)) {
				return;
			}
			AdjustmentBox box = new();
			_adjustments[item] = box;
			if (Enabled) {
				box.Adjustment = Apply(item);
			}
		}

		public void Remove(T item) {
			_adjustments.Remove(item, out var box);
			box.Adjustment?.Dispose();
			box.Adjustment = null;
		}

		public void Enable() {
			foreach ((var cheatable, var box) in _adjustments) {
				box.Adjustment?.Dispose();
				box.Adjustment = Apply(cheatable);
			}
		}

		public void Disable() {
			foreach (var box in _adjustments.Values) {
				box.Adjustment?.Dispose();
				box.Adjustment = null;
			}
		}

		// Using a box allows us to modify the dictionary while iterating over it.
		private class AdjustmentBox {
			public IDisposable? Adjustment { get; set; }
		}

	}
}
