using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Cheats;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Game.Cheats {
	public sealed class SmallCheat : ICheat<PlayerController> {
		private CancellationTokenSource _cts;
		private readonly List<PlayerController> _players = new(1);

		public string DisplayName { get; set; } = "Small";

		public bool Enabled { get; private set; } = false;

		public void EnableCheat() {
			if (Enabled) return;
			Enabled = true;
			foreach (var player in _players) {
				EnableAsync(player, ResetCts()).Forget();
			}
		}

		public void DisableCheat() {
			if (!Enabled) return;
			Enabled = false;
			foreach (var player in _players) {
				DisableAsync(player, ResetCts()).Forget();
			}
		}

		public void OnRegistered(PlayerController cheatable) {
			_players.Add(cheatable);
		}

		public void OnUnregistered(PlayerController cheatable) {
			_players.Remove(cheatable);
		}

		public bool TryRegister(ICheatable cheatable) {
			if (cheatable is not PlayerController player) {
				return false;
			}
			OnRegistered(player);
			return true;
		}

		public bool TryUnregister(ICheatable cheatable) {
			if (cheatable is not PlayerController player) {
				return false;
			}
			OnUnregistered(player);
			return true;
		}

		private UniTask EnableAsync(PlayerController player, CancellationToken cancellationToken) {
			return player.transform.DOScale(Vector3.one * 0.2f, 1f)
				.SetEase(Ease.InOutCubic)
				.ToUniTask(TweenCancelBehaviour.Kill);
		}

		private UniTask DisableAsync(PlayerController player, CancellationToken cancellationToken) {
			return player.transform.DOScale(Vector3.one, 1f)
				.SetEase(Ease.InOutCubic)
				.ToUniTask(TweenCancelBehaviour.Kill);
		}

		private CancellationToken ResetCts() {
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = new();
			return _cts.Token;
		}
	}
}
