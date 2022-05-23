using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Decoy {
    /// <summary>
    /// SaveFileEntry.cs
    ///
    /// Author: Thomas van Opstal
    /// </summary>
    public class SaveFileEntry : MonoBehaviour {
        [SerializeField] private Button deleteButton;
        [SerializeField] private TextMeshProUGUI fileNameDisplay;

        private SteamController steamController;
        private string fileName;

        public void SetupEntry(string file, SteamController sController) {
            steamController = sController;
            fileName = file;
            fileNameDisplay.text = file;
            deleteButton.onClick.AddListener(RemoveFile);
        }

        private void RemoveFile() {
            steamController.DeleteSaveFile(fileName);
        }
    }
}
