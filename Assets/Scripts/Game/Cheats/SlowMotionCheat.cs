using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Cheats;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Game.Cheats {
	public class SlowMotionCheat : ICheat {
		private CancellationTokenSource _cts;

		public const float RealTimeDuration = 10f;
		public const float SlowdownAmount = 0.33f;
		public const float ActualDuration = RealTimeDuration * SlowdownAmount;

		public string DisplayName { get; } = "Slow Motion";

		public bool Enabled { get; private set; }

		public void EnableCheat() {
			if (Enabled) return;
			Enabled = true;
			SlowDownAsync(ResetCts()).Forget();
		}

		public void DisableCheat() {
			if (!Enabled) return;
			Enabled = false;
			SpeedUpAsync(ResetCts()).Forget();
		}

		public bool TryRegister(ICheatable cheatable) {
			return false;
		}

		public bool TryUnregister(ICheatable cheatable) {
			return false;
		}

		private UniTask SlowDownAsync(CancellationToken cancellationToken) {
			return DOTween.To(() => Time.timeScale, (x) => Time.timeScale = x, 0.25f, 1f)
				.SetEase(Ease.InOutSine)
				.ToUniTask(TweenCancelBehaviour.Kill);
		}

		private UniTask SpeedUpAsync(CancellationToken cancellationToken) {
			return DOTween.To(() => Time.timeScale, (x) => Time.timeScale = x, 1f, 1f)
				.SetEase(Ease.InOutSine)
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
