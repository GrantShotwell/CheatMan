using Assets.Scripts.Game.Cheats;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Cheats {
	public class CheatDisplayController : MonoBehaviour{

		[SerializeField] private GameObject _cheatItemPrefab;
		[SerializeField] private RectTransform _cheatItemGroup;

		public async UniTask RunActivation(CheatActivation activation, CancellationToken cancellationToken = default) {
			GameObject instance = Instantiate(_cheatItemPrefab, _cheatItemGroup);
			try {
				var item = instance.GetComponent<CheatDisplayItemController>();
				await item.RunActivation(activation, cancellationToken);
			} finally {
				if (instance) Destroy(instance);
			}
		}

	}
}
