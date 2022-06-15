using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Decoy.Steam {
    /// <summary>
    /// SteamInitializeScreenView.cs
    ///
    /// Author: Thomas van Opstal
    /// </summary>
    public class SteamInitializeScreenView : MonoBehaviour {
        [SerializeField] private TMP_InputField appIDInputfield;
        [SerializeField] private Button hookToSteamButton;
        [SerializeField] private CanvasGroup invalidAppIDMessage;
        [SerializeField] private float fadeSpeed;

        private void Start() {
            hookToSteamButton.onClick.AddListener(HookToSteam);
            invalidAppIDMessage.alpha = 0.0f;
        }

        private void OnDestroy() {
            hookToSteamButton.onClick.RemoveAllListeners();
        }

        private void HookToSteam() {
            StopAllCoroutines();
            invalidAppIDMessage.alpha = 0.0f;

            if (uint.TryParse(appIDInputfield.text, out uint parsedValue))
                SteamManager.HookToSteam(parsedValue);
            else
                StartCoroutine(DisplayInvalidAppIDMessage());
        }

        private IEnumerator DisplayInvalidAppIDMessage() {
            while (invalidAppIDMessage.alpha < 1.0f) {
                invalidAppIDMessage.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }

            yield return new WaitForSeconds(3.0f);

            while (invalidAppIDMessage.alpha > 0.0f) {
                invalidAppIDMessage.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
    }
}