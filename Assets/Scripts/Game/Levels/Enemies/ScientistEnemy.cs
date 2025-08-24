using Cysharp.Threading.Tasks;
using Game.Levels.Enemies.Projectiles;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Enemies {
	public sealed class ScientistEnemy : Enemy {
		[SerializeField, Range(0f, 2f)] public float attackAnimationSeconds;
		[SerializeField] public Vector2 throwRightVelocity;
		[SerializeField] private Sprite[] _idleAnimation;
		[SerializeField] private Sprite[] _attackAnimation;
		[SerializeField] private GameObject _flaskPrefab;
		[SerializeField] private Transform _throwRightPosition;
		private SpriteRenderer _spriteRenderer;

		protected override void Awake() {
			base.Awake();
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		protected override void Start() {
			base.Start();
			SetState(IdleState);
		}

		private async UniTask IdleState(CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				if (SeenByPlayer) {
					SetState(AttackState);
					continue;
				}
				await UniTask.NextFrame(cancellationToken);
			}
		}

		private async UniTask AttackState(CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				await UniTask.WaitForSeconds(weaponSpeed, cancellationToken: cancellationToken);
				if (!SeenByPlayer) {
					SetState(IdleState);
					continue;
				}
				await AttackAsync(Mathf.Sign(DirectionToPlayer.x), cancellationToken);
			}
		}

		private async UniTask AttackAsync(float direction, CancellationToken cancellationToken) {
			try {
			for (int i = 0; i < _attackAnimation.Length; i++) {
				_spriteRenderer.sprite = _attackAnimation[i];
					_spriteRenderer.flipX = direction < 0;
				if (i == _attackAnimation.Length - 1) {
					SpawnProjectile(direction);
					await UniTask.WaitForSeconds(2 * attackAnimationSeconds / _attackAnimation.Length, cancellationToken: cancellationToken);
				} else {
					await UniTask.WaitForSeconds(attackAnimationSeconds / _attackAnimation.Length, cancellationToken: cancellationToken);
				}
			}
			} finally {
			_spriteRenderer.sprite = _idleAnimation[0];
				_spriteRenderer.flipX = false;
			}
		}

		private void SpawnProjectile(float direction) {
			GameObject instance = _container.InstantiatePrefab(_flaskPrefab);
			Vector2 position = _throwRightPosition.localPosition;
			position.x = position.x * direction;
			instance.transform.position = transform.TransformPoint(position);
			ScienceFlaskProjectile projectile = instance.GetComponent<ScienceFlaskProjectile>();
			Vector2 velocity = new(throwRightVelocity.x * direction, throwRightVelocity.y);
			projectile.startVelocity = velocity;
			projectile.damage = weaponDamage;
			instance.SetActive(true);
		}

	}
}
