using System;
using UnityEngine;

namespace Utilities {
	public static class CameraExtensions {

		public static Rect GetOrthographicViewport(this Camera camera) {
			if (camera == null)
				throw new ArgumentNullException(nameof(camera));
			float osize = camera.orthographicSize;
			Vector2 pos = camera.transform.position;
			Vector2 size = new(2 * osize * camera.aspect, 2 * osize);
			Vector2 min = pos - size / 2f, max = pos + size / 2f;
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

	}
}
