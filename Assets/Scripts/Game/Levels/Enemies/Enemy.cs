using Assets.Scripts.Game.Levels.Enemies;
using Cysharp.Threading.Tasks;
using Game.Cheats;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Levels.Enemies {
	public abstract class Enemy : MonoBehaviour, ICheatable {
		[Inject] protected readonly DiContainer _container;
		[Inject] protected readonly CheatManager _cheatManager;
		[Inject] protected readonly PlayerController _player;
		private CancellationTokenSource _cts;
		private StateFunction _currentState;

		private bool _seenByPlayerDirty = true;
		private bool _seenByPlayer = false;

		[field: SerializeField, FormerlySerializedAs("contactDamage")] public AdjustableNumber contactDamage { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("weaponDamage")] public AdjustableNumber weaponDamage { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("weaponSpeed")] public AdjustableNumber weaponSpeed { get; private set; } = new(5);
		[field: SerializeField, FormerlySerializedAs("movementSpeed")] public AdjustableNumber movementSpeed { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("currentHealth")] public float currentHealth { get; set; } = 10;

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
			if (currentHealth <= 0) {
				SetState(DeathState);
			}
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

		protected virtual UniTask DeathState(CancellationToken cancellationToken) {
			Destroy(gameObject);
			return UniTask.CompletedTask;
		}

		protected EnemyProjectile InstantiateProjectile(GameObject prefab) {
			var instance = _container.InstantiatePrefab(prefab);
			var projectile = instance.GetComponent<EnemyProjectile>();
			projectile.damange.Value = weaponDamage;
			return projectile;
		}

		protected virtual bool GetSeenByPlayer() {
			return Vector2.Distance(_player.transform.position, transform.position) < 100f;
		}

		public virtual int DealContactDamage(int health, out bool hit) {
			float damage = Mathf.Max(contactDamage, 0f);
			hit = damage <= 0;
			return health - Mathf.CeilToInt(contactDamage);
		}

	}
}
