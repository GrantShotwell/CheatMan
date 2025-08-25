using Cysharp.Threading.Tasks;
using Game.Levels.Enemies;
using Game.Levels.Obstacles;
using UnityEngine;

public class Projectile : MonoBehaviour {
	private Rigidbody2D _rb;

	public bool playerProjectile = false;
	public float damage = 1f;

	private void Awake() {
		_rb = GetComponent<Rigidbody2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		TestHit(collision.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		TestHit(collision.gameObject);
	}

	private void TestHit(GameObject target) {
		//if (playerProjectile) {
		//	if (!target.transform.CompareTag("PlayerProjectile") && !target.transform.CompareTag("Player")) {
		//		Destroy(gameObject);
		//	}
		//} else {
		//	if (!target.transform.CompareTag("Enemy") && !target.transform.CompareTag("EnemyProjectile")) {
		//		Destroy(gameObject);
		//	}
		//}
		bool destroy = false;
		var enemy = target.GetComponent<Enemy>();
		if (enemy) {
			enemy.DealDamage(damage, _rb.linearVelocity);
			destroy = true;
		}
		var obstacle = target.GetComponent<Obstacle>();
		if (obstacle) {
			obstacle.DealDamage(damage);
			destroy = true;
		}
		if (destroy) {
			Destroy(gameObject);
		}
	}
}
