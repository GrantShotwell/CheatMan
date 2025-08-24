using Cysharp.Threading.Tasks;
using Game.Cheats;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public sealed class BouncingTireObstacle : Obstacle {
		private Rigidbody2D _rb;
		[SerializeField] public AdjustableNumber bounceHeight { get; private set; } = new(10f);

		protected override void Awake() {
			_rb = GetComponent<Rigidbody2D>();
			base.Awake();
		}

		protected override void Start() {
			base.Start();
		}

		private async UniTask BounceState(CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				
			}
		}
	}
}
