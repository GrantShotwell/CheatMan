using UnityEngine;

namespace Game {
	public class ExampleEnemy : IEnemyHealthCheatable {

		[field: SerializeField]
		public AdjustableNumber Health { get; private set; } = new(10f);

	}
}
