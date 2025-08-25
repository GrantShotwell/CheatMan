using Cysharp.Threading.Tasks;
using Game.Cheats;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public sealed class BouncingTireObstacle : Obstacle {
		private Rigidbody2D _rb;
		[SerializeField] public Vector2 initialVelocity = Vector2.zero;

		protected override void Awake() {
			_rb = GetComponent<Rigidbody2D>();
			base.Awake();
		}

		protected override void Start() {
			base.Start();
			SetState(BounceState);
			_rb.linearVelocity = initialVelocity;
		}

		private async UniTask BounceState(CancellationToken cancellationToken) {
		}
	}
}
