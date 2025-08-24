using Cysharp.Threading.Tasks;
using System.Threading;

namespace Game.Levels.Enemies {
	public sealed class ScientistEnemy : Enemy {

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
				await UniTask.WaitForSeconds(weaponSpeed, cancellationToken: cancellationToken);
				if (!SeenByPlayer) {
					SetState(IdleState);
					continue;
				}
				await AttackAsync(cancellationToken);
			}
		}

		private async UniTask AttackAsync(CancellationToken cancellationToken) {
			// TODO
		}

	}
}
