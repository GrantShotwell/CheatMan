using Cysharp.Threading.Tasks;
using Game.Cheats;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Levels.Obstacles {
	public abstract class Obstacle : LevelEntity, ICheatable {

		[field: SerializeField, FormerlySerializedAs("contactDamage")] public AdjustableNumber contactDamage { get; private set; } = new(1);
		public float currentHealth = 1f;

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

		public virtual float GetContactDamage(PlayerController player, float health, out bool hit) {
			Debug.Log(health);
			float damage = Mathf.Max(contactDamage, 0f);
			hit = damage > 0f;
			return Mathf.CeilToInt(contactDamage);
		}

		public void DealDamage(float damage) {
			currentHealth -= damage;
			OnDamaged();
			if (currentHealth <= 0f) {
				SetState(DeathState);
			}
		}

		protected virtual void OnDamaged() { }

	}
}
