using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public sealed class FireHydrant : Obstacle {

		private Collider2D _collider;

		protected override void Awake() {
			_collider = GetComponent<Collider2D>();
		}

		protected override async UniTask DeathState(CancellationToken cancellationToken) {
			try {
				_collider.enabled = false;
				// TODO: Death animation
				await cancellationToken.WaitUntilCanceled();
			} finally {
				if (_collider) _collider.enabled = true;
			}
		}
	}
}
