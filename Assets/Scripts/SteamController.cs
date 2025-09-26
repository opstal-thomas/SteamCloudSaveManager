using Steamworks;
using System;
using System.IO;
using UnityEngine;

namespace TMG {
    /// <summary>
    /// <see cref="SteamController"/>
    ///
    /// Author: Thomas van Opstal for Total Mayhem Games
    /// </summary>
    public class SteamController : MonoBehaviour {

        private bool initialized = false;

        public delegate void SteamHook(bool success, string error);
        public SteamHook onSteamHook;

        public delegate void SteamUnhooked();
        public SteamUnhooked onSteamUnhooked;

        public delegate void SaveFileDeleted(bool success);
        public SaveFileDeleted onSaveFileDeleted;

        private void Update() {
            if (!initialized)
                return;

            SteamAPI.RunCallbacks();
        }

        public void HookToSteam(uint appID) {
            UpdateSteamAppId(appID);
            try {
                bool restartAppIfNecessary = SteamAPI.RestartAppIfNecessary(new AppId_t(appID));
                bool init = SteamAPI.Init();
                initialized = !restartAppIfNecessary && init;
                onSteamHook?.Invoke(initialized, string.Empty);
            } catch (Exception error) {
                onSteamHook?.Invoke(false, error.Message);
            }
        }

        private void UpdateSteamAppId(uint appId) {
            try {
                string appIdFilePath;

#if UNITY_EDITOR
                // In the editor, the working directory is the project root
                appIdFilePath = Path.Combine(Directory.GetCurrentDirectory(), "steam_appid.txt");
#else
                // In builds, place it next to the executable
                appIdFilePath = Path.Combine(Application.dataPath, "..", "steam_appid.txt");
                appIdFilePath = Path.GetFullPath(appIdFilePath);
#endif

                File.WriteAllText(appIdFilePath, appId.ToString());
            } catch (Exception ex) {
                Debug.LogError($"[SteamAppIdUpdater] Failed to update steam_appid.txt: {ex}");
            }
        }

        public string GetStoredSteamAppID() {
            try {
                string appIdFilePath;

#if UNITY_EDITOR
                // In the editor, the working directory is the project root
                appIdFilePath = Path.Combine(Directory.GetCurrentDirectory(), "steam_appid.txt");
#else
                // In builds, place it next to the executable
                appIdFilePath = Path.Combine(Application.dataPath, "..", "steam_appid.txt");
                appIdFilePath = Path.GetFullPath(appIdFilePath);
#endif

                return File.ReadAllText(appIdFilePath);
            } catch (Exception ex) {
                Debug.LogError($"[SteamAppIdUpdater] Failed to update steam_appid.txt: {ex}");
            }

            return string.Empty;
        }

        public void UnhookFromSteam() {
            SteamAPI.Shutdown();
            initialized = false;
            onSteamUnhooked?.Invoke();
        }

        public void DeleteSaveFile(string fileName) {
            bool success = SteamRemoteStorage.FileDelete(fileName);
            onSaveFileDeleted?.Invoke(success);
        }

        public string[] GetCloudSaveFiles() {
            int fileCount = SteamRemoteStorage.GetFileCount();
            string[] fileNames = new string[fileCount];
            for (int i = 0; i < fileCount; i++)
                fileNames[i] = SteamRemoteStorage.GetFileNameAndSize(i, out _);

            return fileNames;
        }
    }
}