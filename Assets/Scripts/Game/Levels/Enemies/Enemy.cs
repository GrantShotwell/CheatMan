using Assets.Scripts.Game.Levels.Enemies;
using Cysharp.Threading.Tasks;
using Game.Cheats;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Levels.Enemies {
	public abstract class Enemy : LevelEntity, ICheatable {

		[field: SerializeField, FormerlySerializedAs("contactDamage")] public AdjustableNumber contactDamage { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("weaponDamage")] public AdjustableNumber weaponDamage { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("weaponSpeed")] public AdjustableNumber weaponSpeed { get; private set; } = new(5);
		[field: SerializeField, FormerlySerializedAs("movementSpeed")] public AdjustableNumber movementSpeed { get; private set; } = new(1);
		[field: SerializeField, FormerlySerializedAs("currentHealth")] public float currentHealth { get; set; } = 10;

		protected override void FixedUpdate() {
			base.FixedUpdate();
			if (currentHealth <= 0) {
				SetState(DeathState);
			}
		}

		protected virtual UniTask DeathState(CancellationToken cancellationToken) {
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

		public void DealDamage(float damage) {
			currentHealth -= damage;
			OnDamaged();
		}

		protected virtual void OnDamaged() { }

	}
}
