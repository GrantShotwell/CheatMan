using Cysharp.Threading.Tasks;
using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.Cheats {
	public class CheatDisplayItemController : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI _text;

		public async UniTask RunActivation(CheatActivation activation, CancellationToken cancellationToken = default) {
			_text.text = activation.Cheat.ToString();
		}

	}
}
