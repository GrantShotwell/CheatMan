using Assets.Scripts.Game.Levels.Enemies;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public class TireSpawner : LevelEntitySpawner {

		[SerializeField] public Vector2 initialVelocity = Vector2.zero;
		[SerializeField] public Vector2 randomVelocityOffsetMin = Vector2.zero;
		[SerializeField] public Vector2 randomVelocityOffsetMax = Vector2.zero;

		protected override LevelEntity Spawn() {
			var tire = (BouncingTireObstacle)base.Spawn();
			Vector2 rand = new(
				Random.Range(randomVelocityOffsetMin.x, randomVelocityOffsetMax.x),
				Random.Range(randomVelocityOffsetMin.y, randomVelocityOffsetMax.y)
			);
			tire.initialVelocity = initialVelocity + rand;
			return tire;
		}
	}
}
