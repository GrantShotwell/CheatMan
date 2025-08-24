using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game {
	public class CameraController : MonoBehaviour {
		private Camera _camera;
		private readonly List<CameraControllerLimiter> _limiters = new(4);

		private Vector2 _positionVelocity;
		private float _sizeVelocity;
		public float baseSize = 25f;
		public float smoothTime = 0.3f;
		public float smoothSizeTime = 0.3f;

		[field: SerializeField] public Transform Target { get; set; }

		private void Awake() {
			_camera = GetComponent<Camera>();
		}

		private void Update() {
			CameraControllerLimiter sizeLimiter =
				(from limiter in _limiters
				where limiter.setSize
				select limiter).FirstOrDefault();
			float sizee = sizeLimiter ? sizeLimiter.size : baseSize;
			sizee = Mathf.SmoothDamp(_camera.orthographicSize, sizee, ref _sizeVelocity, smoothSizeTime);
			_camera.orthographicSize = sizee;

			Vector2 position = Target.position;
			if (_limiters.Count > 0) {
				Vector2 min = _limiters.Aggregate(Vector2.positiveInfinity, (a, b) => Vector2.Min(a, b.min));
				Vector2 max = _limiters.Aggregate(Vector2.negativeInfinity, (a, b) => Vector2.Max(a, b.max));
				Vector2 size = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
				position.x = Mathf.Clamp(position.x, min.x + size.x, max.x - size.y);
				position.y = Mathf.Clamp(position.y, min.y + size.y, max.y - size.y);
			}
			position = Vector2.SmoothDamp(_camera.transform.position, position, ref _positionVelocity, smoothTime);
			Vector3 position3D = position;
			position3D.z = _camera.transform.position.z;
			_camera.transform.position = position3D;
		}

		public void EnterLimiter(CameraControllerLimiter limiter) {
			_limiters.Add(limiter);
		}

		public void ExitLimiter(CameraControllerLimiter limiter) {
			_limiters.Remove(limiter);
		}

	}
}
