using Cysharp.Threading.Tasks;
using Game.Cheats;
using Game.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Levels.Enemies {
	public class EnemyProjectile : LevelEntity {
		protected Rigidbody2D _rb;

		public float damage { get; set; } = 1;
		public Vector2 startVelocity { get; set; } = Vector2.zero;
		public float lifetimeSeconds { get; set; } = 10f;

		protected override void Awake() {
			base.Awake();
			_rb = GetComponent<Rigidbody2D>();
		}

		protected override void Start() {
			base.Start();
			_rb.linearVelocity = startVelocity;
			LifetimeTimer(lifetimeSeconds, destroyCancellationToken).Forget();
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			OnHitSomething();
			var player = collision.gameObject.GetComponent<PlayerController>();
			if (player) {
				player.DealDamage(damage);
			}
		}

		public void OnHitSomething() {
			SetState(LandedState);
		}

		protected UniTask LandedState(CancellationToken cancellationToken) {
			Destroy(gameObject);
			return UniTask.CompletedTask;
		}

		protected async UniTask LifetimeTimer(float seconds,  CancellationToken cancellationToken) {
			await UniTask.WaitForSeconds(seconds, cancellationToken: cancellationToken);
			if (gameObject) Destroy(gameObject);
		}

	}
}
