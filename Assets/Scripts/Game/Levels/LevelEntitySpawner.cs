using Cysharp.Threading.Tasks;
using Game.Cheats;
using Game.Levels;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Utilities;
using Zenject;

namespace Assets.Scripts.Game.Levels.Enemies {
	public class LevelEntitySpawner : MonoBehaviour {
		[Inject] protected readonly DiContainer _container;
		private LevelEntity _prefabEntity;

		public GameObject prefab;
		public AdjustableNumber spawnCount = new(1);
		public AdjustableNumber respawnInterval = new(1);
		public float despawnDistance;

		private readonly List<LevelEntity> _enemies = new();

		private void Awake() {
			_prefabEntity = prefab.GetComponent<LevelEntity>();
		}

		private void Start() {
			RespawnLoop(destroyCancellationToken).Forget();
		}

		private void Update() {
			Span<bool> destroyed = stackalloc bool[_enemies.Count];
			for (int i = 0; i < _enemies.Count; i++) {
				var entity = _enemies[i];
				if (entity == null) {
					destroyed[i] = true;
					continue;
				}
				if (TestCanDespawn(entity)) {
					destroyed[i] = true;
					Destroy(entity.gameObject);
				}
			}
			int removedCount = 0;
			for (int i = 0; i < destroyed.Length; i++) {
				if (!destroyed[i]) continue;
				_enemies.RemoveAt(i - removedCount);
				removedCount++;
			}
		}

		private void OnDrawGizmosSelected() {
			Gizmos.color = TestCanSpawn(out var rect) ? Color.turquoise : Color.purple;
			Gizmos.DrawWireCube(rect.center, rect.size);
		}

		private async UniTask RespawnLoop(CancellationToken cancellationToken = default) {
			while (!cancellationToken.IsCancellationRequested) {
				if (TestCanSpawn(out _)) {
					Spawn();
					await UniTask.WaitForSeconds(respawnInterval, cancellationToken: cancellationToken);
				}
				await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken: cancellationToken);
			}
		}

		protected virtual LevelEntity Spawn() {
			GameObject instance = _container.InstantiatePrefab(prefab);
			instance.transform.position = transform.position;
			LevelEntity component = instance.GetComponent<LevelEntity>();
			if (component == null) {
				Debug.LogError("Prefab missing enemy component.");
				return null;
			}
			_enemies.Add(component);
			return component;
		}

		private bool TestCanSpawn(out Rect entityRect) {
			entityRect = _prefabEntity.VisibleRect;
			entityRect.position += (Vector2)transform.position - (Vector2)_prefabEntity.transform.position;
			return _enemies.Count < spawnCount
				&& !Camera.main.GetOrthographicViewport().Overlaps(entityRect);
		}

		private bool TestCanDespawn(LevelEntity enemy) {
			return !enemy.SeenByPlayer
				// Outside despawn distance.
				&& (Vector2.Distance(enemy.transform.position, transform.position) > despawnDistance);
		}

	}
}
