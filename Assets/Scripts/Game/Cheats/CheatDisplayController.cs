using Assets.Scripts.Game.Cheats;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Cheats {
	public class CheatDisplayController : MonoBehaviour {
		[SerializeField] private GameObject _cheatItemPrefab;
		[SerializeField] private RectTransform _cheatItemGroup;
		[SerializeField] private Image _healthbarForegroundImage;

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
