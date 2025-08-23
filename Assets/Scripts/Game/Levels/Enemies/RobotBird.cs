using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Enemies {
	public sealed class RobotBird : Enemy {
		private Vector3 _startingPosition;
		private double _startingTime;

		public float flyVariance = 1f;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private Sprite[] _animationFrames;

		protected override void Awake() {
			base.Awake();
			_startingTime = Time.timeAsDouble;
			_startingPosition = transform.localPosition;
		}

		protected override void Start() {
			base.Start();
			SetState(IdleState);
			FlyAnimation(destroyCancellationToken).Forget();
		}

		private async UniTask IdleState(CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				float height = (float)Math.Sin(_startingTime - Time.timeAsDouble) * flyVariance;
				var pos = transform.localPosition;
				pos.y = height + _startingPosition.y;
				pos.x -= movementSpeed * Time.deltaTime;
				transform.localPosition = pos;
				await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken: cancellationToken);
			}
		}

		private async UniTask FlyAnimation(CancellationToken cancellationToken) {
			const float FrameDuration = 0.2f;
			while (!cancellationToken.IsCancellationRequested) {
				for (int i = 0; i < _animationFrames.Length; i++) {
					await UniTask.WaitForSeconds(FrameDuration);
					_spriteRenderer.sprite = _animationFrames[i];
				}
			}
		}
	}
}
