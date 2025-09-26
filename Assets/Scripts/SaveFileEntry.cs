using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMG {
    /// <summary>
    /// <see cref="SaveFileEntry"/>
    ///
    /// Author: Thomas van Opstal for Total Mayhem Games
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
