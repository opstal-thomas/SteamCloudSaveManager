using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Decoy {
    /// <summary>
    /// HookView.cs
    ///
    /// Author: Thomas van Opstal
    /// </summary>
    public class HookView : MonoBehaviour {
        [SerializeField] private GameObject initializePanel;
        [SerializeField] private SteamController steamController;
        [SerializeField] private TMP_InputField appIdInputfield;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Button initializeButton;

        private void Start() {
            steamController.onSteamHook += OnSteamHooked;
            steamController.onSteamUnhooked += OnSteamUnhooked;
            initializeButton.onClick.AddListener(HookToSteam);
        }

        private void OnDestroy() {
            steamController.onSteamHook -= OnSteamHooked;
            steamController.onSteamUnhooked -= OnSteamUnhooked;
            initializeButton.onClick.RemoveAllListeners();
        }
        
        private void OnSteamUnhooked() {
            initializePanel.SetActive(true);
        }

        private void OnSteamHooked(bool success, string error) {
            if (!success)
                warningText.text = "An error occured. " + error;
            else
                initializePanel.SetActive(false);
        }

        private void HookToSteam() {
            if (string.IsNullOrEmpty(appIdInputfield.text)) {
                warningText.text = "Please enter a valid appID";
                return;
            }

            if (uint.TryParse(appIdInputfield.text, out uint appID)) {
                steamController.HookToSteam(appID);
                return;
            }

            warningText.text = "Please enter a valid AppID";
        }
    }
}