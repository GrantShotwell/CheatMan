using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Cheats;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Obstacles {
	public sealed class BouncingTireObstacle : Obstacle {
		[SerializeField] public Vector2 initialVelocity = Vector2.zero;
		private Rigidbody2D _rb;
		[SerializeField] private GameObject _spritesParent;
		[SerializeField] private ParticleSystem _hitParticles;

		private bool _bouncing;

		protected override void Awake() {
			_rb = GetComponent<Rigidbody2D>();
			base.Awake();
		}

		protected override void Start() {
			base.Start();
			SetState(BounceState);
			_rb.linearVelocity = initialVelocity;
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			Debug.Log("Bounce!");
			AnimateBounce(destroyCancellationToken).Forget();
			_hitParticles.Play();
			_hitParticles.transform.rotation = Quaternion.identity;
		}

		private async UniTask BounceState(CancellationToken cancellationToken) {
		}

		private async UniTask AnimateBounce(CancellationToken cancellationToken) {
			await _spritesParent.transform.DOScale(1.2f, 0.2f)
				.SetEase(Ease.OutCirc)
				.ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
			await _spritesParent.transform.DOScale(1f, 0.3f)
				.SetEase(Ease.InCirc)
				.ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
		}
	}
}
