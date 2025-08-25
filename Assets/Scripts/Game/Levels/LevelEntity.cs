using Assets.Scripts.Game.Cheats;
using Cysharp.Threading.Tasks;
using Game.Cheats;
using System;
using System.Threading;
using UnityEngine;
using Utilities;
using Zenject;

namespace Game.Levels {
	public abstract class LevelEntity : MonoBehaviour, ICheatable, IBowWearing, IHatWearing {
		[Inject] protected readonly DiContainer _container;
		[Inject] protected readonly CheatManager _cheatManager;
		[Inject] protected readonly PlayerController _player;
		[SerializeField] private float _visibleBoundsPadding = 1f;
		private CancellationTokenSource _cts;
		private StateFunction _currentState;

		private bool _seenByPlayerDirty = true;
		private bool _seenByPlayer = false;
		private Collider2D[] _visibleRectColliders = null;
		private bool _visibleRectDirty = true;
		private Rect _visibleRect;

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

		public virtual Rect VisibleRect {
			get {
				if (!_visibleRectDirty) return _visibleRect;
				_visibleRectDirty = false;
				return _visibleRect = GetVisibleRect();
			}
		}

		protected delegate UniTask StateFunction(CancellationToken cancellationToken);

		protected virtual void OnValidate() {
			_visibleRectDirty = true;
		}

		protected virtual void Awake() {
			_visibleRectColliders = GetComponents<Collider2D>();
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
			_visibleRectDirty = true;
		}

		protected virtual void OnDrawGizmosSelected() {
			Gizmos.color = SeenByPlayer ? Color.blue : Color.red;
			Rect rect = VisibleRect;
			Gizmos.DrawWireCube(rect.center, rect.size);
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
			return Camera.main.GetOrthographicViewport().Overlaps(VisibleRect);
		}

		protected virtual Rect GetVisibleRect() {
			Vector2 min = Vector2.positiveInfinity;
			Vector2 max = Vector2.negativeInfinity;
			int included = 0;
			if (_visibleRectColliders == null) {
				// Can be edit mode, or a spawner accessing a prefab.
				_visibleRectColliders = GetComponents<Collider2D>();
			}
			foreach (Collider2D collider in _visibleRectColliders) {
				Bounds bounds = collider.bounds;
				min = Vector2.Min(bounds.min, min);
				max = Vector2.Max(bounds.max, max);
				included++;
			}
			if (included == 0) {
				min = max = transform.position;
			}
			float p = _visibleBoundsPadding;
			return Rect.MinMaxRect(min.x - p, min.y - p, max.x + p, max.y + p);
		}

	}
}
