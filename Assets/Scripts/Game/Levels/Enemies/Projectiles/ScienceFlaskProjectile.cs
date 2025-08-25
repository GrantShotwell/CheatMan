using Assets.Scripts.Game.Levels.Enemies;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Enemies.Projectiles {
	public sealed class ScienceFlaskProjectile : EnemyProjectile {
		[SerializeField, Range(0f, 360f * 10)] public float rotationSpeed = 360f;

		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();
			SetState(ThrownState);
			float direction = Mathf.Sign(startVelocity.x);
			_rb.angularVelocity = rotationSpeed * -direction;
		}

		private async UniTask ThrownState(CancellationToken cancellationToken) {
		}

	}
}
