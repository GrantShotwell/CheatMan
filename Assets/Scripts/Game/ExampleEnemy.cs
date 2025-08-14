using Game.Cheats;
using UnityEngine;
using Zenject;

namespace Game {
	public class ExampleEnemy : MonoBehaviour, IEnemyHealthCheatable {
		[SerializeField] private readonly CheatManager _cheatManager;

		[field: SerializeField]
		public AdjustableNumber Health { get; private set; } = new(10f);

		private void Update() {
			Debug.Log($"My health is {Health}!");
		}

		private void OnEnable() {
			_cheatManager.Register(this);
		}

		private void OnDisable() {
			_cheatManager.Unregister(this);
		}

		public class Factory : PlaceholderFactory<ExampleEnemy> { }
	}
}
