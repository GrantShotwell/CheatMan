using Assets.Scripts.Game.Cheats;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game.Cheats {
	public class CheatManager : IDisposable {

		private readonly CheatPopupController _popup;
		private readonly CheatDisplayController _display;
		private readonly CancellationTokenSource _disposeCts = new();

		public IEnumerable<ICheat> Cheats {
			get {
				yield return Dash;
				yield return SuperSpeed;
				yield return UnlimitedJump;
				yield return WallJump;
				yield return ZeroGravity;
				yield return FancyBow;
				yield return FancyHat;
				yield return AddHealth;
				yield return SlowMotion;
				yield return Small;
			}
		}

		[field: Inject] public DashCheat Dash { get; private set; }
		[field: Inject] public SuperSpeedCheat SuperSpeed { get; private set; }
		[field: Inject] public UnlimitedJumpCheat UnlimitedJump { get; private set; }
		[field: Inject] public WallJumpCheat WallJump { get; private set; }
		[field: Inject] public ZeroGravityCheat ZeroGravity { get; private set; }
		[field: Inject] public FancyBowCheat FancyBow { get; private set; }
		[field: Inject] public FancyHatCheat FancyHat { get; private set; }
		[field: Inject] public AddHealthCheat AddHealth { get; private set; }
		[field: Inject] public SlowMotionCheat SlowMotion { get; private set; }
		[field: Inject] public SmallCheat Small { get; private set; }

		/// <summary>
		/// Is true when the cheat UI is open.
		/// </summary>
		public bool IsCheating => _popup.IsOpen;

		public CheatManager(CheatPopupController popup, CheatDisplayController display) {
			Subscribe(this._popup = popup);
			this._display = display;
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
				"dash" => new(Dash, 10f),
				"superspeed" => new(SuperSpeed, 10f),
				"unlimitedjump" => new(UnlimitedJump, 10f),
				"walljump" => new(WallJump, 10f),
				"zerogravity" => new(ZeroGravity, 10f),
				"bow" => new(FancyBow, 30f),
				"hat" => new(FancyHat, 30f),
				"slowmotion" => new(SlowMotion, SlowMotionCheat.ActualDuration),
				"heal" => new(AddHealth, 30f),
				"small" => new(Small, 15f),
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
			try {
				activation.Cheat.EnableCheat();
				await _display.RunActivation(activation, cancellationToken);
			} finally {
				activation.Cheat.DisableCheat();
			}
		}
	}
}
