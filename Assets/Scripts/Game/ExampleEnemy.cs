using Game.Cheats;
using UnityEngine;
using Zenject;

namespace Game {
	public class ExampleEnemy : MonoBehaviour, IEnemyHealthCheatable {
		[Inject] private readonly CheatManager _cheatManager;

		[field: SerializeField]
		public AdjustableNumber Health { get; private set; } = new(10f);

		private void Start() {
			_cheatManager.Register(this);
			_cheatManager.EnemyHealthCheat.EnableCheat();
		}

		private void Update() {
			Debug.Log($"My health is {Health}!");
		}

		private void OnEnable() {
		}

		private void OnDisable() {
			_cheatManager.Unregister(this);
		}

		public class Factory : PlaceholderFactory<ExampleEnemy> { }
	}
}
