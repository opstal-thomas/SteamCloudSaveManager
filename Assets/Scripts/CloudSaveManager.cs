using UnityEngine;
using UnityEngine.UI;

namespace Decoy {
    /// <summary>
    /// CloudSaveManager.cs
    ///
    /// Author: Thomas van Opstal
    /// </summary>
    public class CloudSaveManager : MonoBehaviour {
        [SerializeField] private SteamController steamController;
        [SerializeField] private GameObject managementPanel;
        [SerializeField] private GameObject saveGameEntry;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private Button unhookFromSteamButton;

        private void Start() {
            steamController.onSteamHook += OnSteamHooked;
            steamController.onSteamUnhooked += OnSteamUnhooked;
            steamController.onSaveFileDeleted += DrawSaveFiles;
            unhookFromSteamButton.onClick.AddListener(steamController.UnhookFromSteam);
        }

        private void OnDestroy() {
            steamController.onSteamHook -= OnSteamHooked;
            steamController.onSteamUnhooked -= OnSteamUnhooked;
            steamController.onSaveFileDeleted -= DrawSaveFiles;
            unhookFromSteamButton.onClick.RemoveAllListeners();
        }

        private void OnSteamHooked(bool success, string error) {
            if (success) {
                managementPanel.SetActive(true);
                DrawSaveFiles();
            }
        }

        private void OnSteamUnhooked() {
            managementPanel.SetActive(false);
        }

        private void DrawSaveFiles() {
            for (int i = 0; i < contentPanel.childCount; i++)
                Destroy(contentPanel.GetChild(i).gameObject);

            string[] saveFiles = steamController.GetCloudSaveFiles();

            foreach (string file in saveFiles) {
                GameObject entry = Instantiate(saveGameEntry, contentPanel);
                entry.GetComponent<SaveFileEntry>().SetupEntry(file, steamController);
            }
        }
    }
}