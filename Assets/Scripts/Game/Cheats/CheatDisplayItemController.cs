using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Cheats {
	public class CheatDisplayItemController : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private Image _image;
		private float _timeLeft = 0f;
		private float _totalTime = 0f;

		private float TimeLeft {
			get => _timeLeft;
			set {
				_timeLeft = value;
				if (_image) _image.fillAmount = _timeLeft / _totalTime;
			}
		}

		public async UniTask RunActivation(CheatActivation activation, CancellationToken cancellationToken = default) {
			_text.text = activation.Cheat.DisplayName;
			_totalTime = _timeLeft = activation.DurationSeconds;
			await DOTween.To(() => TimeLeft, (x) => TimeLeft = x, 0f, activation.DurationSeconds)
				.SetEase(Ease.Linear)
				.ToUniTask(TweenCancelBehaviour.Complete, cancellationToken);
		}

	}
}
