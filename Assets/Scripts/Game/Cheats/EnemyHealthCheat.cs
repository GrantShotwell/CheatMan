using Game.Cheats;
using System;

namespace Assets.Scripts.Game.Cheats {
	public class EnemyHealthCheat : ICheat<IEnemyHealthCheatable> {

		private readonly AdjustmentDictionary<IEnemyHealthCheatable> _adjustments;

		public EnemyHealthCheat(AdjustmentDictionary<IEnemyHealthCheatable> adjustments) {
			this._adjustments = new(Apply);
		}

		public void EnableCheat() {
			this._adjustments.Enable();
		}

		public void DisableCheat() {
			this._adjustments.Disable();
		}

		public void OnRegistered(IEnemyHealthCheatable cheatable) {
			this._adjustments.Add(cheatable);
		}

		public void OnUnregistered(IEnemyHealthCheatable cheatable) {
			this._adjustments.Remove(cheatable);
		}

		private IDisposable Apply(IEnemyHealthCheatable cheatable) {
			return cheatable.Health.Scale(2f);
		}

	}
}
