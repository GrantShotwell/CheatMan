using Assets.Scripts.Game.Cheats;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Game.Cheats {
	public class CheatManager : IDisposable {

		private readonly CheatPopupController _popup;
		private readonly CancellationTokenSource _disposeCts = new();

		public IEnumerable<ICheat> Cheats {
			get {
				yield return Dash;
				yield return SuperSpeed;
				yield return UnlimitedJump;
				yield return WallJump;
				yield return ZeroGravity;
			}
		}

		[field: Inject] public DashCheat Dash { get; private set; }
		[field: Inject] public SuperSpeedCheat SuperSpeed { get; private set; }
		[field: Inject] public UnlimitedJumpCheat UnlimitedJump { get; private set; }
		[field: Inject] public WallJumpCheat WallJump { get; private set; }
		[field: Inject] public ZeroGravityCheat ZeroGravity { get; private set; }

		public CheatManager(CheatPopupController popup) {
			Subscribe(this._popup = popup);
		}

		public void Dispose() {
			Unsubcribe(this._popup);
			_disposeCts.Cancel();
			_disposeCts.Dispose();
		}

		public void Register(ICheatable cheatable) {
			foreach (var cheat in Cheats) {
				cheat.TryRegister(cheatable);
			}
		}

		public void Unregister(ICheatable cheatable) {
			foreach (var cheat in Cheats) {
				cheat.TryUnregister(cheatable);
			}
		}

		private bool TryGetCheat(string code, out CheatActivation activation) {
			code = code.ToLower().Replace(" ", string.Empty).Trim();
			activation = code switch {
				"dash" => new(Dash, 1f),
				"superspeed" => new(SuperSpeed, 1f),
				"unlimitedjump" => new(UnlimitedJump, 1f),
				"walljump" => new(WallJump, 1f),
				"zerogravity" => new(ZeroGravity, 1f),
				_ => default,
			};
			Debug.Log($"Cheat code \"{code}\": {activation.Cheat?.ToString() ?? "null"}");
			return activation.Cheat != null;
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
				if (TryGetCheat(text, out CheatActivation activation) && !activation.Cheat.Enabled) {
					RunCheat(activation, _disposeCts.Token).Forget();
				} else {
					// TODO: Wrong input
				}
			} catch (Exception exception) {
				Debug.LogException(exception);
			}
		}

		private async UniTask RunCheat(CheatActivation activation, CancellationToken cancellationToken = default) {
			activation.Cheat.EnableCheat();
			await UniTask.WaitForSeconds(activation.DurationSeconds, cancellationToken: cancellationToken);
			activation.Cheat.DisableCheat();
		}
	}
}
