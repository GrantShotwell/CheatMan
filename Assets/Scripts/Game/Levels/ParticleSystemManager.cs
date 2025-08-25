using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Levels {
	public sealed class ParticleSystemManager : MonoBehaviour {

		public void KeepAlive(GameObject instance, float duration) {
			instance.transform.parent = transform;
			KeepAliveAsync(instance, duration, destroyCancellationToken).Forget();
		}

		private async UniTask KeepAliveAsync(GameObject instance, float duration, CancellationToken cancellationToken = default) {
			await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);
			Destroy(instance);
		}

	}
}
