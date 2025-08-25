using Assets.Scripts.Game.Levels;
using Game.Levels.Obstacles;
using UnityEngine;
using Zenject;

namespace Game.Levels.Enemies.Projectiles {
	public sealed class ScienceFlaskProjectile : EnemyProjectile {
		[Inject] private readonly ParticleSystemManager _particleSystemManager;
		[SerializeField, Range(0f, 360f * 10)] public float rotationSpeed = 360f;
		[SerializeField, Range(0, 20)] public int smokeCount = 5;
		[SerializeField] private GameObject _particles;
		[SerializeField] private GameObject _smokePrefab;

		protected override void Start() {
			base.Start();
			float direction = Mathf.Sign(startVelocity.x);
			_rb.angularVelocity = rotationSpeed * -direction;
		}

		public override void OnHitSomething() {
			_particleSystemManager.KeepAlive(_particles, 10f);
			for (int i = 0; i < smokeCount; i++) {
				GameObject instance = _container.InstantiatePrefab(_smokePrefab);
				instance.transform.position = transform.position;
			}
			base.OnHitSomething();
		}
	}
}
