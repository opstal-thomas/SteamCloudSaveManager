// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.
#if STEAM
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Build;

// This copys various files into their required locations when Unity is launched to make installation a breeze.
[InitializeOnLoad]
public class RedistInstall {
	static RedistInstall() {
		AddDefineSymbols();
		CheckForOldDlls();
	}

	static void CheckForOldDlls() {
		string strCwdPath = Directory.GetCurrentDirectory();

		// Unfortunately we can't just delete these outright because Unity loads the dlls in the project root instantly and Windows won't let us delete them because they are in use.

		string strDllPath = Path.Combine(strCwdPath, "steam_api.dll");
		if (File.Exists(strDllPath)) {
			Debug.LogError("[Steamworks.NET] Please delete the old version of 'steam_api.dll' in your project root before continuing.");
		}

		string strDll64Path = Path.Combine(strCwdPath, "steam_api64.dll");
		if (File.Exists(strDll64Path)) {
			Debug.LogError("[Steamworks.NET] Please delete the old version of 'steam_api64.dll' in your project root before continuing.");
		}
	}

	static void AddDefineSymbols() {
		BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
		NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        string currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
		HashSet<string> defines = new HashSet<string>(currentDefines.Split(';')) {
			"STEAMWORKS_NET"
		};

        string newDefines = string.Join(";", defines); 
		if (newDefines != currentDefines)
			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);
	}
}
#endif