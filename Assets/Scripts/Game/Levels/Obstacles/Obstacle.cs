using Game.Cheats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Levels.Obstacles {
	public abstract class Obstacle : MonoBehaviour, ICheatable {

		[field: SerializeField, FormerlySerializedAs("contactDamage")] public AdjustableNumber contactDamage { get; private set; } = new(1);

		public virtual int DealContactDamage(int health, out bool hit) {
			float damage = Mathf.Max(contactDamage, 0f);
			hit = damage <= 0;
			return health - Mathf.CeilToInt(contactDamage);
		}

	}
}
