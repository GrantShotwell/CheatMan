using Game.Levels.Enemies;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public sealed class ElectricWallObstacle : Obstacle {

		public override int DealContactDamage(int health, out bool hit) {
			hit = true;
			return Mathf.Max(0, Mathf.CeilToInt(contactDamage.ApplyAdjustments(health / 2f)));
		}
	}
}
