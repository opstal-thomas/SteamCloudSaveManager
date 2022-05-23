using System;
using System.Collections.Generic;
using UnityEngine;

namespace Decoy {
    /// <summary>
    /// SteamController.cs
    ///
    /// Author: Thomas van Opstal
    /// </summary>
    public class SteamController : MonoBehaviour {
        private bool initialized = false;

        public delegate void SteamHook(bool success, string error);
        public SteamHook onSteamHook;

        public delegate void SteamUnhooked();
        public SteamUnhooked onSteamUnhooked;

        public delegate void SaveFileDeleted();
        public SaveFileDeleted onSaveFileDeleted;

        private void Update() {
            if (!initialized)
                return;

            Steamworks.SteamClient.RunCallbacks();
        }

        public void HookToSteam(uint appID) {
            try {
                Steamworks.SteamClient.Init(appID);
                initialized = true;
                onSteamHook?.Invoke(true, string.Empty);
            } catch (Exception error) {
                onSteamHook?.Invoke(false, error.Message);
            }
        }

        public void UnhookFromSteam() {
            Steamworks.SteamClient.Shutdown();
            initialized = false;
            onSteamUnhooked?.Invoke();
        }

        public void DeleteSaveFile(string fileName) {
            Steamworks.SteamRemoteStorage.FileDelete(fileName);
            onSaveFileDeleted?.Invoke();
        }

        public string[] GetCloudSaveFiles() {
            List<string> saveFiles = new List<string>();
            saveFiles.AddRange(Steamworks.SteamRemoteStorage.Files);
            return saveFiles.ToArray();
        }
    }
}