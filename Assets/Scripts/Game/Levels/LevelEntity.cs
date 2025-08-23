using Assets.Scripts.Game.Levels.Enemies;
using Cysharp.Threading.Tasks;
using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Levels {
	public class LevelEntity : MonoBehaviour, ICheatable {
		[Inject] protected readonly DiContainer _container;
		[Inject] protected readonly CheatManager _cheatManager;
		[Inject] protected readonly PlayerController _player;
		private CancellationTokenSource _cts;
		private StateFunction _currentState;

		private bool _seenByPlayerDirty = true;
		private bool _seenByPlayer = false;

		protected delegate UniTask StateFunction(CancellationToken cancellationToken);

		public bool SeenByPlayer {
			get {
				if (!_seenByPlayerDirty) return _seenByPlayer;
				_seenByPlayerDirty = false;
				return _seenByPlayer = GetSeenByPlayer();
			}
		}

		protected virtual void Awake() {
		}

		protected virtual void Start() {
			_cheatManager.Register(this);
		}

		protected virtual void OnDestroy() {
			_cheatManager.Unregister(this);
		}

		protected virtual void FixedUpdate() {
			_seenByPlayerDirty = true;
		}

		protected void SetState(StateFunction fn, CancellationToken cancellationToken = default) {
			if (fn == _currentState) return;
			ResetState(fn, cancellationToken);
		}

		protected void ResetState(StateFunction fn, CancellationToken cancellationToken = default) {
			RefreshCts(cancellationToken);
			_currentState = fn;
			RunState(fn, _cts.Token).Forget();
		}

		private void RefreshCts(CancellationToken cancellationToken = default) {
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, cancellationToken);
		}

		private async UniTask RunState(StateFunction fn, CancellationToken cancellationToken = default) {
			while (!cancellationToken.IsCancellationRequested) {
				try {
					await fn(cancellationToken);
				} catch (Exception exception) {
					if (exception is OperationCanceledException) continue;
					Debug.LogException(exception);
				}
				await UniTask.NextFrame(PlayerLoopTiming.Update);
			}
		}

		protected virtual bool GetSeenByPlayer() {
			return Vector2.Distance(_player.transform.position, transform.position) < 100f;
		}

	}
}
