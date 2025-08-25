using Assets.Scripts.Game.Levels;
using Cysharp.Threading.Tasks;
using Game.Cheats;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Levels.Enemies {
	public abstract class Enemy : LevelEntity, ICheatable {
		[Inject] private readonly ParticleSystemManager _particleSystemManager;
		[SerializeField] private GameObject _particlesParent;
		private DeathParticlesController _deathParticles;

		private Vector2 _lastDamageDirection = Vector2.zero;

		[field: SerializeField, FormerlySerializedAs("contactDamage")] public AdjustableNumber contactDamage { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("weaponDamage")] public AdjustableNumber weaponDamage { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("weaponSpeed")] public AdjustableNumber weaponSpeed { get; private set; } = new(5);
		[field: SerializeField, FormerlySerializedAs("movementSpeed")] public AdjustableNumber movementSpeed { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("currentHealth")] public float currentHealth { get; set; } = 10;
		[field: SerializeField] public float particlesAfterlife { get; private set; } = 10f;

		protected override void Awake() {
			_deathParticles = GetComponentInChildren<DeathParticlesController>();
			base.Awake();
		}

		protected override void FixedUpdate() {
			base.FixedUpdate();
		}

		protected virtual UniTask DeathState(CancellationToken cancellationToken) {
			if (_deathParticles) _deathParticles.StartParticles(_lastDamageDirection);
			if (_particlesParent) _particleSystemManager.KeepAlive(_particlesParent, particlesAfterlife);
			Destroy(gameObject);
			return UniTask.CompletedTask;
		}

		protected EnemyProjectile InstantiateProjectile(GameObject prefab) {
			var instance = _container.InstantiatePrefab(prefab);
			var projectile = instance.GetComponent<EnemyProjectile>();
			projectile.damage = weaponDamage;
			return projectile;
		}

		public virtual float GetContactDamage(PlayerController player, float health, out bool hit) {
			Debug.Log(player);
			float damage = Mathf.Max(contactDamage, 0f);
			hit = damage > 0f;
			return Mathf.CeilToInt(contactDamage);
		}

		public void DealDamage(float damage, Vector2 direction) {
			currentHealth -= damage;
			_lastDamageDirection = direction;
			OnDamaged();
			if (currentHealth <= 0) {
				SetState(DeathState);
			}
		}

		protected virtual void OnDamaged() { }

	}
}
