using Assets.Scripts.Game.Cheats;
using System.Collections.Generic;
using Zenject;

namespace Game.Cheats {
	public class CheatManager {

		private ICollection<IPlayerHealthCheatable> PlayerHealthCheatables { get; set; } = new List<IPlayerHealthCheatable>();

		[field: Inject]
		public EnemyHealthCheat EnemyHealthCheat { get; private set; }

		private ICollection<IEnemyHealthCheatable> EnemyHealthCheatables { get; set; } = new List<IEnemyHealthCheatable>();

		public void Register(object cheatable) {
			if (cheatable is IPlayerHealthCheatable playerHealthCheatable) {
				PlayerHealthCheatables.Add(playerHealthCheatable);
			}
			if (cheatable is IEnemyHealthCheatable enemyHealthCheatable) {
				EnemyHealthCheatables.Add(enemyHealthCheatable);
			}
		}

		public void Unregister(object cheatable) {
			if (cheatable is IPlayerHealthCheatable playerHealthCheatable) {
				PlayerHealthCheatables.Remove(playerHealthCheatable);
			}
			if (cheatable is IEnemyHealthCheatable enemyHealthCheatable) {
				EnemyHealthCheatables.Remove(enemyHealthCheatable);
			}
		}

	}
}
