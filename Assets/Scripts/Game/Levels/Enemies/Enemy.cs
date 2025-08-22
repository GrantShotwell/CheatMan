using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Levels.Enemies {
	public abstract class Enemy : MonoBehaviour, ICheatable {
		
		public AdjustableNumber contactDamage = new(1);
		public AdjustableNumber weaponDamage = new(1);
		public AdjustableNumber movementSpeed = new(1);

	}
}
