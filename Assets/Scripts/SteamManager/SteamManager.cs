using System;
using System.Collections;
using System.Linq;
using Steamworks;
using UnityEngine;

namespace Decoy.Steam {
    /// <summary>
    /// SteamManager.cs
    ///
    /// Author: Thomas van Opstal
    /// </summary>
    public static class SteamManager {
        private const string DEBUG = "[SteamManager]: ";
        private class MonoBehaviourHelper : MonoBehaviour { }
        private static MonoBehaviourHelper monoBehaviourHelper;

        public static bool IsHookedToSteam { get; private set; } = false;

        public delegate void SteamHooked(SteamResults result, string message);
        public static SteamHooked onSteamHooked;

        public delegate void SteamUnhooked(SteamResults result, string message);
        public static SteamUnhooked onSteamUnhooked;

        public delegate void SteamCloudFileDeleted(SteamResults result, string message);
        public static SteamCloudFileDeleted onSteamCloudFileDeleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeSteamManager() {
            GameObject monohelperObject = new GameObject("MonoBehaviourHelperObject");
            monohelperObject.AddComponent<MonoBehaviourHelper>();
            monoBehaviourHelper = monohelperObject.GetComponent<MonoBehaviourHelper>();
            monoBehaviourHelper.StartCoroutine(RunSteamApiCallbacks());
        }

        /// <summary>
        /// Hook to the steam client using a specific appID.
        /// </summary>
        /// <param name="appID">game appID</param>
        /// <param name="callback">optional callback</param>
        public static void HookToSteam(uint appID, Action<SteamResults, string> callback = null) {
            try {
                SteamClient.Init(appID);
                IsHookedToSteam = true;
                onSteamHooked?.Invoke(SteamResults.Succes, "Hooked to steam client.");
                callback?.Invoke(SteamResults.Succes, "Hooked to steam client.");
            } catch (Exception exception) {
                IsHookedToSteam = false;
                onSteamHooked?.Invoke(SteamResults.Failed, "Hook failed. Error message: " + exception.Message);
                callback?.Invoke(SteamResults.Failed, "Hook failed. Error message: " + exception.Message);

                Debug.LogError("Hook failed. Error message: " + exception.Message);
            }
        }

        /// <summary>
        /// Unhook from the steam client.
        /// </summary>
        /// <param name="callback">optional callback</param>
        public static void UnhookFromSteam(Action<SteamResults, string> callback = null) {
            try {
                SteamClient.Shutdown();
                IsHookedToSteam = false;
                onSteamUnhooked?.Invoke(SteamResults.Succes, "Unhooked from steam client.");
                callback?.Invoke(SteamResults.Succes, "Unhooked from steam client.");
            } catch (Exception exception) {
                IsHookedToSteam = false;
                onSteamUnhooked?.Invoke(SteamResults.Failed, "Unhook failed. Error message: " + exception.Message);
                callback?.Invoke(SteamResults.Failed, "Unhook failed. Error message: " + exception.Message);

                Debug.LogError("Unhook failed. Error message: " + exception.Message);
            }
        }

        /// <summary>
        /// Retrieve all steam cloud file names.
        /// </summary>
        /// <returns>Array of all file names found in the cloud</returns>
        public static string[] RetrieveSteamCloudFileNames() {
            if (!IsHookedToSteam) {
                Debug.LogError(DEBUG + "Application is not hooked to the steam client.");
                return Array.Empty<string>();
            }

            return SteamRemoteStorage.Files.ToArray();
        }

        /// <summary>
        /// Delete a specific save file from the steam cloud.
        /// </summary>
        /// <param name="fileName">Name of the cloud file</param>
        /// <param name="callback">optional callback</param>
        public static void DeleteSteamCloudFile(string fileName, Action<SteamResults, string> callback = null) {
            bool succes = SteamRemoteStorage.FileDelete(fileName);
            onSteamCloudFileDeleted?.Invoke(succes ? SteamResults.Succes : SteamResults.Failed, "Operation successful: " + succes);
            callback?.Invoke(succes ? SteamResults.Succes : SteamResults.Failed, "Operation successful: " + succes);
        }

        private static IEnumerator RunSteamApiCallbacks() {
            while (true) {
                if (IsHookedToSteam)
                    SteamClient.RunCallbacks();

                yield return null;
            }
        }
    }
}