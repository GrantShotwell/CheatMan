using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Levels.Enemies {
	public class EnemyProjectile : MonoBehaviour {

		[field: SerializeField] public AdjustableNumber damange { get; private set; } = new(1);
		[field: SerializeField] public AdjustableNumber speed { get; private set; } = new(1);

	}
}
