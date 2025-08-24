using Assets.Scripts.Game.Cheats;
using Cysharp.Threading.Tasks;
using Game.Cheats;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Game.Levels {
	public abstract class LevelEntity : MonoBehaviour, ICheatable, IBowWearing, IHatWearing {
		[Inject] protected readonly DiContainer _container;
		[Inject] protected readonly CheatManager _cheatManager;
		[Inject] protected readonly PlayerController _player;
		private CancellationTokenSource _cts;
		private StateFunction _currentState;

		private bool _seenByPlayerDirty = true;
		private bool _seenByPlayer = false;

		[field: SerializeField] public AdjustableBoolean wearingBow { get; private set; } = new(false);
		[field: SerializeField] public GameObject BowGameObject { get; private set; }
		[field: SerializeField] public AdjustableBoolean wearingHat { get; private set; } = new(false);
		[field: SerializeField] public GameObject HatGameObject { get; private set; }

		public bool SeenByPlayer {
			get {
				if (!_seenByPlayerDirty) return _seenByPlayer;
				_seenByPlayerDirty = false;
				return _seenByPlayer = GetSeenByPlayer();
			}
		}

		public virtual Vector2 DirectionToPlayer {
			get {
				return _player.transform.position - transform.position;
			}
		}

		protected delegate UniTask StateFunction(CancellationToken cancellationToken);

		protected virtual void Awake() {
		}

		protected virtual void Start() {
			_cheatManager.Register(this);
		}

		protected virtual void OnDestroy() {
			_cheatManager.Unregister(this);
		}

		protected virtual void Update() {
			if (BowGameObject != null) BowGameObject.SetActive(wearingBow);
			if (HatGameObject != null) HatGameObject.SetActive(wearingHat);
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
