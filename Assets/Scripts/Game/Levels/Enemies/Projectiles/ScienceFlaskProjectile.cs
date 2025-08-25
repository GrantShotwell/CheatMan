using Assets.Scripts.Game.Levels;
using Assets.Scripts.Game.Levels.Enemies;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Game.Levels.Enemies.Projectiles {
	public sealed class ScienceFlaskProjectile : EnemyProjectile {
		[Inject] private readonly ParticleSystemManager _particleSystemManager;
		[SerializeField, Range(0f, 360f * 10)] public float rotationSpeed = 360f;
		[SerializeField] private GameObject _particles;

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

		public override void OnHitSomething() {
			_particleSystemManager.KeepAlive(_particles, 10f);
			base.OnHitSomething();
		}
	}
}
