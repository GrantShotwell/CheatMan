using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Enemies {
	public sealed class RobotBird : Enemy {
		private Vector3 _startingPosition;
		private double _startingTime;

		public float flyVariance = 1f;

		protected override void Awake() {
			base.Awake();
			_startingTime = Time.timeAsDouble;
			_startingPosition = transform.localPosition;
		}

		protected override void Start() {
			base.Start();
			SetState(IdleState);
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
	}
}
