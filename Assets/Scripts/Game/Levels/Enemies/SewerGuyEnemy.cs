using Cysharp.Threading.Tasks;
using System.Threading;

namespace Game.Levels.Enemies {
	public sealed class SewerGuyEnemy : Enemy {
		private bool _exposed = false;

		protected override void Start() {
			base.Start();
			SetState(IdleState);
		}

		private async UniTask IdleState(CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				if (SeenByPlayer) {
					SetState(AttackState);
					continue;
				}
				await UniTask.NextFrame(cancellationToken);
			}
		}

		private async UniTask AttackState(CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				if (!SeenByPlayer) {
					SetState(AttackState);
					continue;
				}
				await UniTask.WaitForSeconds(weaponSpeed, cancellationToken: cancellationToken);
				await AttackAsync(cancellationToken);
				await UniTask.NextFrame(cancellationToken);
			}
		}

		private async UniTask AttackAsync(CancellationToken cancellationToken) {
			try {
				_exposed = true;
				// TODO: Play animation and spawn projectile.
			} finally {
				_exposed = false;
			}
		}

	}
}
