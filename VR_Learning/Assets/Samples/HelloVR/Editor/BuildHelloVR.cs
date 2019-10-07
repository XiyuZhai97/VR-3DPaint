// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class BuildHelloVR
{
	private static string _destinationPath;
	private static void CustomizedCommandLine()
	{
		Dictionary<string, Action<string>> cmdActions = new Dictionary<string, Action<string>>
		{
			{
				"-destinationPath", delegate(string argument)
				{
					_destinationPath = argument;
				}
			}
		};

		Action<string> actionCache;
		string[] cmdArguments = Environment.GetCommandLineArgs();

		for (int count = 0; count < cmdArguments.Length; count++)
		{
			if (cmdActions.ContainsKey(cmdArguments[count]))
			{
				actionCache = cmdActions[cmdArguments[count]];
				actionCache(cmdArguments[count + 1]);
			}
		}

		if (string.IsNullOrEmpty(_destinationPath))
		{
			_destinationPath = Path.GetDirectoryName(Application.dataPath);
		}
	}

	private static void GeneralSettings()
	{
		PlayerSettings.Android.bundleVersionCode = 3;
		PlayerSettings.bundleVersion = "3.0.0";
		PlayerSettings.companyName = "HTC Corp.";
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
	}

	static void BuildApkAll()
	{
		CustomizedCommandLine();
		BuildApk(_destinationPath + "/", false, true, true, true);
		BuildApk(_destinationPath + "/armv7", false);
		BuildApk(_destinationPath + "/arm64", false, true, false, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build wvr_unity_hellovr.apk/32+64bit")]
	static void BuildApk()
	{
		BuildApk(null, false, true, true, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build wvr_unity_hellovr.apk/32bit")]
	static void BuildApk32()
	{
		BuildApk("armv7", false);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build wvr_unity_hellovr.apk/64bit")]
	static void BuildApk64()
	{
		BuildApk("arm64", false, true, false, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Run wvr_unity_hellovr.apk/32+64bit")]
	static void BuildAndRunApk()
	{
		BuildApk(null, true, true, true, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Run wvr_unity_hellovr.apk/32bit")]
	static void BuildAndRunApk32()
	{
		BuildApk("armv7", true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Run wvr_unity_hellovr.apk/64bit")]
	static void BuildAndRunApk64()
	{
		BuildApk("arm64", true, true, false, true);
	}

	public static void BuildApk(string destPath, bool run, bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
	{
		string[] levels = { "Assets/Samples/HelloVR/Scenes/HelloVR.unity" };
		BuildApkInner("hellovr.unity", "wvr_unity_hellovr.apk", destPath, run, levels, isIL2CPP, isSupport32, isSupport64);
	}

	private static void BuildApkInner(string idName, string apkName, string destPath, bool run, string[] levels, bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
	{
		if (!isSupport32 && !isSupport64)
			isSupport32 = true;

		GeneralSettings();

		PlayerSettings.productName = "HelloVR";
#if UNITY_5_6_OR_NEWER
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.htc.vr.samples." + idName);
#else
		PlayerSettings.bundleIdentifier = "com.htc.vr.samples.hellovr.unity";
#endif
		Texture2D icon1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/HelloVR/wave_hellovr_unity_app_icon/res/mipmap-mdpi/wave_hellovr_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/HelloVR/wave_hellovr_unity_app_icon/res/mipmap-hdpi/wave_hellovr_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/HelloVR/wave_hellovr_unity_app_icon/res/mipmap-xhdpi/wave_hellovr_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon4 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/HelloVR/wave_hellovr_unity_app_icon/res/mipmap-xxhdpi/wave_hellovr_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon5 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/HelloVR/wave_hellovr_unity_app_icon/res/mipmap-xxxhdpi/wave_hellovr_unity_app_icon.png", typeof(Texture2D));

		if (icon5 == null)
			Debug.LogError("Fail to read app icon");

		Texture2D[] group = { icon5, icon4, icon3, icon2, icon1, icon1 };

		PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, group);
		PlayerSettings.gpuSkinning = false;
#if UNITY_2017_2_OR_NEWER
		PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);
#else
		PlayerSettings.mobileMTRendering = true;
#endif
		PlayerSettings.graphicsJobs = true;

		if (isIL2CPP)
			PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
		else
			PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
		if (isSupport32)
		{
			if (isSupport64)
				PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
			else
				PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
		}
		else
		{
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
		}

		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
		PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel26;

		string outputFilePath = string.IsNullOrEmpty(destPath) ? apkName
			: destPath + "/" + apkName;
		BuildPipeline.BuildPlayer(levels, outputFilePath, BuildTarget.Android, run ? BuildOptions.AutoRunPlayer : BuildOptions.None);
	}
}
