using UnityEngine;
using UnityEngine.UI;

namespace TMG {
    /// <summary>
    /// <see cref="CloudSaveManager"/>
    ///
    /// Author: Thomas van Opstal for Total Mayhem Games
    /// </summary>
    public class CloudSaveManager : MonoBehaviour {

        [SerializeField] private SteamController steamController;
        [SerializeField] private GameObject managementPanel;
        [SerializeField] private GameObject saveGameEntry;
        [SerializeField] private GameObject noSaveFilesEntry;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private Button unhookFromSteamButton;
        [SerializeField] private Button refreshButton;

        private void Start() {
            steamController.onSteamHook += OnSteamHooked;
            steamController.onSteamUnhooked += OnSteamUnhooked;
            steamController.onSaveFileDeleted += OnSaveFileDeleted;
            unhookFromSteamButton.onClick.AddListener(steamController.UnhookFromSteam);
            refreshButton.onClick.AddListener(DrawSaveFiles);
        }

        private void OnDestroy() {
            steamController.onSteamHook -= OnSteamHooked;
            steamController.onSteamUnhooked -= OnSteamUnhooked;
            steamController.onSaveFileDeleted -= OnSaveFileDeleted;
            unhookFromSteamButton.onClick.RemoveAllListeners();
            refreshButton.onClick.RemoveAllListeners();
        }

        private void OnSaveFileDeleted(bool success) {
            if (success)
                DrawSaveFiles();
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

            if (saveFiles.Length == 0)
                Instantiate(noSaveFilesEntry, contentPanel);

        }
    }
}