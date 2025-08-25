using Cysharp.Threading.Tasks;
using Game.Cheats;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public sealed class MovingCarObstacle : Obstacle {

		[field: SerializeField] public AdjustableNumber drivingSpeed { get; private set; } = new(10f);

		protected override void Start() {
			base.Start();
			SetState(DrivingState);
		}

		private async UniTask DrivingState(CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
				transform.position += drivingSpeed * Time.fixedDeltaTime * Vector3.left;
			}
		}

	}
}
