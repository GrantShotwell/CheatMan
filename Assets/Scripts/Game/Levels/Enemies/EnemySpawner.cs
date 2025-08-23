using Cysharp.Threading.Tasks;
using Game.Cheats;
using Game.Levels.Enemies;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Levels.Enemies {
	public sealed class EnemySpawner : MonoBehaviour {
		[Inject] private readonly DiContainer _container;

		public GameObject prefab;
		public AdjustableNumber spawnCount = new(1);
		public AdjustableNumber respawnInterval = new(1);
		public float despawnDistance;

		private readonly ICollection<Enemy> _enemies = new List<Enemy>();

		private void Start() {
			RespawnLoop(destroyCancellationToken).Forget();
		}

		private void Update() {
			var destroyed = new List<Enemy>(_enemies.Count);
			foreach (var enemy in _enemies) {
				if (enemy == null) {
					destroyed.Add(enemy);
					continue;
				}
				if (TestCanDespawn(enemy)) {
					destroyed.Add(enemy);
					Destroy(enemy.gameObject);
				}
			}
			foreach (var enemy in destroyed) {
				_enemies.Remove(enemy);
			}
		}

		private async UniTask RespawnLoop(CancellationToken cancellationToken = default) {
			while (!cancellationToken.IsCancellationRequested) {
				if (TestCanSpawn()) {
					Spawn();
					await UniTask.WaitForSeconds(respawnInterval, cancellationToken: cancellationToken);
				}
				await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken: cancellationToken);
			}
		}

		private Enemy Spawn() {
			GameObject instance = _container.InstantiatePrefab(prefab);
			instance.transform.position = transform.position;
			Enemy component = instance.GetComponent<Enemy>();
			if (component == null) {
				Debug.LogError("Prefab missing enemy component.");
				return null;
			}
			_enemies.Add(component);
			return component;
		}

		private bool TestCanSpawn() {
			return _enemies.Count < spawnCount;
		}

		private bool TestCanDespawn(Enemy enemy) {
			return !enemy.SeenByPlayer
				// Outside despawn distance.
				&& (Vector2.Distance(enemy.transform.position, transform.position) > despawnDistance);
		}

	}
}
