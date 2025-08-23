using Game.Cheats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Levels.Obstacles {
	public abstract class Obstacle : MonoBehaviour, ICheatable {

		[field: SerializeField, FormerlySerializedAs("contactDamage")] public AdjustableNumber contactDamage { get; private set; } = new(1);

		public virtual float GetContactDamage(PlayerController player, float health, out bool hit) {
			Debug.Log(health);
			float damage = Mathf.Max(contactDamage, 0f);
			hit = damage > 0f;
			return Mathf.CeilToInt(contactDamage);
		}

	}
}
