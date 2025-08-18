using Assets.Scripts.Game.Cheats;
using System;
using UnityEngine;
using Zenject;

namespace Game.Cheats {
	public class CheatManager : IDisposable {

		private readonly CheatPopupController _popup;

		public bool IsCheating { get; private set; }

		[field: Inject]
		public EnemyHealthCheat EnemyHealthCheat { get; private set; }

		public CheatManager(CheatPopupController popup) {
			Subscribe(this._popup = popup);
		}

		public void Dispose() {
			Unsubcribe(this._popup);
		}

		public void Register(ICheatable cheatable) {
			if (cheatable is IPlayerHealthCheatable playerHealthCheatable) {
				// TODO
			}
			if (cheatable is IEnemyHealthCheatable enemyHealthCheatable) {
				EnemyHealthCheat.OnRegistered(enemyHealthCheatable);
			}
		}

		public void Unregister(ICheatable cheatable) {
			if (cheatable is IPlayerHealthCheatable playerHealthCheatable) {
				// TODO
			}
			if (cheatable is IEnemyHealthCheatable enemyHealthCheatable) {
				EnemyHealthCheat.OnUnregistered(enemyHealthCheatable);
			}
		}

		private ICheat GetCheat(string code) {
			code = code.ToLower().Replace(" ", string.Empty);
			var cheat = code switch {
				"enemyhealth" => EnemyHealthCheat,
				_ => null,
			};
			Debug.Log($"Cheat code \"{code}\": {cheat?.ToString() ?? "null"}");
			return cheat;
		}

		private void Subscribe(CheatPopupController value) {
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			value.TextSubmitted += HandleTextSubmitted;
		}

		private void Unsubcribe(CheatPopupController value) {
			if (value == null)
				return;
			value.TextSubmitted -= HandleTextSubmitted;
		}

		private void HandleTextSubmitted(string text) {
			try {
				ICheat cheat = GetCheat(text);
				if (cheat == null) {
					// TODO: Wrong input
				} else {
					cheat.EnableCheat();
				}
			} catch (Exception exception) {
				Debug.LogException(exception);
			}
		}
	}
}
