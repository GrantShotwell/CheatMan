using Assets.Scripts.Game.Cheats;
using Zenject;

namespace Game.Cheats {
	public class CheatManager {

		[field: Inject]
		public EnemyHealthCheat EnemyHealthCheat { get; private set; }

		public void Register(object cheatable) {
			if (cheatable is IPlayerHealthCheatable playerHealthCheatable) {
				// TODO
			}
			if (cheatable is IEnemyHealthCheatable enemyHealthCheatable) {
				EnemyHealthCheat.OnRegistered(enemyHealthCheatable);
			}
		}

		public void Unregister(object cheatable) {
			if (cheatable is IPlayerHealthCheatable playerHealthCheatable) {
				// TODO
			}
			if (cheatable is IEnemyHealthCheatable enemyHealthCheatable) {
				EnemyHealthCheat.OnUnregistered(enemyHealthCheatable);
			}
		}

	}
}
