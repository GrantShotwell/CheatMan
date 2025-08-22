using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Cheats {
	public abstract class SimpleCheat<T> : ICheat<T> where T : ICheatable {
		private readonly AdjustmentDictionary<T> _adjustments;

		public SimpleCheat() {
			this._adjustments = new(Apply);
		}

		public void EnableCheat() {
			this._adjustments.Enable();
		}

		public void DisableCheat() {
			this._adjustments.Disable();
		}

		public void OnRegistered(T cheatable) {
			this._adjustments.Add(cheatable);
		}

		public void OnUnregistered(T cheatable) {
			this._adjustments.Remove(cheatable);
		}

		public bool TryRegister(ICheatable cheatable) {
			if (cheatable is not T target) {
				return false;
			}
			OnRegistered(target);
			return true;
		}

		public bool TryUnregister(ICheatable cheatable) {
			if (cheatable is not T target) {
				return false;
			}
			OnUnregistered(target);
			return true;
		}

		protected abstract IDisposable Apply(T cheatable);
	}
}
