using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public class ScientistFlaskSmoke : Obstacle {
		private Rigidbody2D _rb;
		[SerializeField] public float lifetime = 3f;
		[SerializeField] private SpriteRenderer _spriteRenderer;

		protected override void Awake() {
			_rb = GetComponent<Rigidbody2D>();
			base.Awake();
		}

		protected override void Start() {
			base.Start();
			float speed = 3f;
			_rb.linearVelocity = new(Random.Range(-speed, +speed), Random.Range(-speed, +speed));
			SetState(FloatingState);
		}

		private async UniTask FloatingState(CancellationToken cancellationToken) {
			async UniTask DisableCollider() {
				await UniTask.WaitForSeconds(lifetime * 0.8f, cancellationToken: cancellationToken);
				// TODO
			}
			await UniTask.WhenAll(
				transform.DOScale(3f, lifetime)
					.SetEase(Ease.OutCubic)
					.ToUniTask(TweenCancelBehaviour.Kill, cancellationToken),
				_spriteRenderer.DOColor(_spriteRenderer.color * 0f, lifetime)
					.SetEase(Ease.InQuart)
					.ToUniTask(TweenCancelBehaviour.Kill, cancellationToken)
			);
			SetState(DeathState);
		}

	}
}
