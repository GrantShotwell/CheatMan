using Assets.Scripts.Game.Levels.Enemies;
using Cysharp.Threading.Tasks;
using Game.Levels.Enemies.Projectiles;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Game.Levels.Enemies {
	public sealed class Guardman : Enemy {
		[SerializeField, Range(0f, 2f)] public float attackAnimationSeconds;
        [SerializeField, Range(0f, 2f)] public float shootAnimationSeconds;
        [SerializeField, Range(0f, 2f)] public float backupAnimationSeconds;
        [SerializeField] public Vector2 shootRightVelocity;
		[SerializeField] private Sprite[] _idleAnimation;
		[SerializeField] private Sprite[] _attackAnimation;
        [SerializeField] private Sprite[] _walkAnimation;
        [SerializeField] private Sprite[] _backupAnimation;
        [SerializeField] private Sprite[] _shootAnimation;
        [SerializeField] private GameObject _shotPrefab;
		[SerializeField] private Transform _shootRightPosition;
        [SerializeField] private float attackWalkSpeed = 20;
        [SerializeField] GameObject backup;
        [SerializeField] Transform backupSpawn;
        private SpriteRenderer _spriteRenderer;
        [Inject] DiContainer _container;

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
				//await UniTask.WaitForSeconds(weaponSpeed, cancellationToken: cancellationToken);
				if (!SeenByPlayer) {
					SetState(IdleState);
					continue;
				}
				var action = Random.Range(0, 20);
				switch (action)
				{
                    case < 8:
                        await ShootAsync(Mathf.Sign(DirectionToPlayer.x), cancellationToken);
						break;
					case < 15:
                        //swipe attack
                        await UniTask.WhenAny(
                            WalkToPlayerAsync(attackWalkSpeed, 5, cancellationToken: cancellationToken),
                            RunAnimationForeverAsync(_walkAnimation, 0.3f, cancellationToken: cancellationToken));
                        await SwipeAsync(Mathf.Sign(DirectionToPlayer.x), cancellationToken);
                        break;
					case <18:
						await BackupAsync(Mathf.Sign(DirectionToPlayer.x), cancellationToken);
                        //backup
                        break;
                    default:
						//await UniTask.WaitForSeconds(0.2f);
						break;
                }
                await UniTask.WaitForSeconds(1);
            }
		}

		private async UniTask ShootAsync(float direction, CancellationToken cancellationToken) {
			try {
				for (int i = 0; i < _shootAnimation.Length; i++) {
					_spriteRenderer.sprite = _shootAnimation[i];
					_spriteRenderer.flipX = direction < 0;
					if (i == _shootAnimation.Length - 1) {
						SpawnProjectile(direction);
						await UniTask.WaitForSeconds(2 * shootAnimationSeconds / _shootAnimation.Length, cancellationToken: cancellationToken);
					} else {
						await UniTask.WaitForSeconds(shootAnimationSeconds / _shootAnimation.Length, cancellationToken: cancellationToken);
					}
				}
			} finally {
				_spriteRenderer.sprite = _idleAnimation[0];
				_spriteRenderer.flipX = false;
			}
		}

        private async UniTask SwipeAsync(float direction, CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i < _attackAnimation.Length; i++)
                {
                    _spriteRenderer.sprite = _attackAnimation[i];
                    _spriteRenderer.flipX = direction < 0;
                    if (i == _attackAnimation.Length - 1)
                    {
                        SpawnProjectile(direction);
                        await UniTask.WaitForSeconds(2 * attackAnimationSeconds / _attackAnimation.Length, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await UniTask.WaitForSeconds(attackAnimationSeconds / _attackAnimation.Length, cancellationToken: cancellationToken);
                    }
                }
            }
            finally
            {
                _spriteRenderer.sprite = _idleAnimation[0];
                _spriteRenderer.flipX = false;
            }
        }

        private async UniTask BackupAsync(float direction, CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i < _backupAnimation.Length; i++)
                {
                    _spriteRenderer.sprite = _backupAnimation[i];
                    _spriteRenderer.flipX = direction < 0;
                    if (i == _backupAnimation.Length - 1)
                    {
                        //put backup spawn
                        await UniTask.WaitForSeconds(2 * backupAnimationSeconds / _backupAnimation.Length, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await UniTask.WaitForSeconds(backupAnimationSeconds / _backupAnimation.Length, cancellationToken: cancellationToken);
                    }
                }
                InstantiateEnemy(backup,backupSpawn);
            }
            finally
            {
                _spriteRenderer.sprite = _idleAnimation[0];
                _spriteRenderer.flipX = false;
            }
        }

        private void SpawnProjectile(float direction) {
			GameObject instance = _container.InstantiatePrefab(_shotPrefab);
			Vector2 position = _shootRightPosition.localPosition;
			position.x = position.x * direction;
			instance.transform.position = transform.TransformPoint(position);
			ScienceFlaskProjectile projectile = instance.GetComponent<ScienceFlaskProjectile>();
			Vector2 velocity = new(shootRightVelocity.x * direction, shootRightVelocity.y);
			projectile.startVelocity = velocity;
			projectile.damage = weaponDamage;
            projectile.GetComponent<SpriteRenderer>().flipX = Mathf.Sign(shootRightVelocity.x) > 0;
			instance.SetActive(true);
		}

        private async UniTask WalkToPlayerAsync(float speed, float distance, CancellationToken cancellationToken)
        {
            while (DirectionToPlayer.magnitude > distance)
            {
                await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
                Vector2 direction = Vector2.right * Mathf.Sign(DirectionToPlayer.x);
                _spriteRenderer.flipX = Mathf.Sign(direction.x) < 0;
                transform.position += (Vector3)(direction * speed * Time.fixedDeltaTime);
            }
        }

        private async UniTask RunAnimationForeverAsync(Sprite[] animation, float frameSeconds, CancellationToken cancellationToken)
        {
            while (true)
            {
                for (int i = 0; i < animation.Length; i++)
                {
                    _spriteRenderer.sprite = animation[i];
                    await UniTask.WaitForSeconds(frameSeconds, cancellationToken: cancellationToken);
                }
            }
        }

        private Enemy InstantiateEnemy(GameObject prefab, Transform spawnPoint)
        {
            var instance = _container.InstantiatePrefab(prefab);
            var newenemy = instance.GetComponent<Enemy>();
            newenemy.transform.position = spawnPoint.position;
            return newenemy;
        }

    }
}
