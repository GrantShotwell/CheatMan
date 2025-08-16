using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Cheats {
	public class CheatPopupController : MonoBehaviour {

		[SerializeField, Range(0f, 1f)] public float openDuration = 0.5f;

		[SerializeField] private CanvasGroup _group;
		[SerializeField] private TextMeshProUGUI _text;
		private CancellationTokenSource _closeCts = null;

		public string Text { get => _text.text; set => _text.text = value; }

		public bool IsOpen { get; private set; } = false;

		public event Action<string> TextSubmitted;

		private void Start() {
			if (!IsOpen) _group.alpha = 0f;
			Keyboard.current.onTextInput += HandleTextInput;
		}

		private void OnDestroy() {
			Keyboard.current.onTextInput -= HandleTextInput;
			this._closeCts?.Cancel();
			this._closeCts?.Dispose();
			this._closeCts = null;
		}

		public void Open() {
			Debug.Log("Opening cheat popup.");
			this._closeCts?.Cancel();
			Text = string.Empty;
			IsOpen = true;
			OpenAsync(destroyCancellationToken).Forget();
		}

		private async UniTask OpenAsync(CancellationToken cancellationToken = default) {
			await this._group.DOFade(1f, openDuration).ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
		}

		public void Close() {
			Debug.Log("Closing cheat popup.");
			IsOpen = false;
			TextSubmitted?.Invoke(Text);
			this._closeCts?.Cancel();
			this._closeCts?.Dispose();
			this._closeCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
			CloseAsync(this._closeCts.Token).Forget();
		}

		private async UniTask CloseAsync(CancellationToken cancellationToken = default) {
			await this._group.DOFade(0f, openDuration).ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
		}

		private void HandleTextInput(char input) {
			try {
				if (!IsOpen || !enabled) {
					return;
				}
				if (input == '\b') {
					if (Text.Length > 0) {
						Text = Text.Substring(0, Text.Length - 1);
					}
				} else if (input == '\n' || input == '\r') {
					// Ignore
				} else {
					Text += input;
				}
			} catch (Exception exception) {
				Debug.LogException(exception);
			}
		}


		public void OnCheatInput(InputAction.CallbackContext context) {
			if (context.started) {
				if (IsOpen) Close();
				else Open();
			}
		}

	}
}
