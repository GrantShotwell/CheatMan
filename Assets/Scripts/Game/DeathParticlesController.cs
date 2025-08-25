using Unity.VisualScripting;
using UnityEngine;

namespace Game {

	public class DeathParticlesController : MonoBehaviour {
		[SerializeField] private ParticleSystem _particles;

		private void Awake() {
			_particles = GetComponent<ParticleSystem>();
		}

		public void StartParticles(Vector2 direction) {
			direction = direction.normalized * 15;
			var velocityModule = _particles.velocityOverLifetime;
			velocityModule.x = direction.x;
			velocityModule.y = direction.y;
			_particles.Play();
		}

	}
}
