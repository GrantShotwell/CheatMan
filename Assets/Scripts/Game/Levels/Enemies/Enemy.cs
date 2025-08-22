using Cysharp.Threading.Tasks;
using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Levels.Enemies {
	public abstract class Enemy : MonoBehaviour, ICheatable {
		[Inject] protected readonly CheatManager _cheatManager;
		private CancellationTokenSource _cts;
		private StateFunction _currentState;

		public AdjustableNumber contactDamage = new(1);
		public AdjustableNumber weaponDamage = new(1);
		public AdjustableNumber movementSpeed = new(1);
		public AdjustableNumber currentHealth = new(10);

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
			fn(_cts.Token).Forget();
		}

		private void RefreshCts(CancellationToken cancellationToken = default) {
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, cancellationToken);
		}

		protected virtual UniTask DeathState(CancellationToken cancellationToken) {
			Destroy(gameObject);
			return UniTask.CompletedTask;
		}

	}
}
