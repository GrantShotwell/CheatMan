using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game {
	public class CameraControllerLimiter : MonoBehaviour {

		public Vector2 min = new(float.NegativeInfinity, float.NegativeInfinity);
		public Vector2 max = new(float.PositiveInfinity, float.PositiveInfinity);
		public bool setSize = false;
		public float size = 0f;

	}
}
