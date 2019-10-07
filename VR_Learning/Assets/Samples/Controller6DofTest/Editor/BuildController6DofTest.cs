﻿// "WaveVR SDK 
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

public class BuildController6DofTest
{
	private static void GeneralSettings()
	{
		PlayerSettings.Android.bundleVersionCode = 3;
		PlayerSettings.bundleVersion = "3.0.0";
		PlayerSettings.companyName = "HTC Corp.";
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
	}

	//[UnityEditor.MenuItem("WaveVR/Build Android APK/Build Controller6DofTestUnity.apk")]
	static void BuildApk()
	{
		BuildApk(null, false);
	}

	//[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Run Controller6DofTestUnity.apk")]
	static void BuildAndRunApk()
	{
		BuildApk(null, true);
	}

	public static void BuildApk(string destPath, bool run)
	{
		string[] levels = { "Assets/Samples/Controller6DofTest/Scenes/Controller6DofTest.unity" };
		BuildApkInner("Controller6DofTest.unity", "Controller6DofTestUnity.apk", destPath, run, levels);
	}

	private static void BuildApkInner(string idName, string apkName, string destPath, bool run, string[] levels)
	{
		GeneralSettings();

		PlayerSettings.productName = "Controller6DofTest";
#if UNITY_5_6_OR_NEWER
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.htc.vr.samples." + idName);
#else
		PlayerSettings.bundleIdentifier = "com.htc.vr.samples.Controller6DofTest.unity";
#endif
		Texture2D icon1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/Controller6DofTest/wave_Controller6DofTest_unity_app_icon/res/mipmap-mdpi/wave_Controller6DofTest_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/Controller6DofTest/wave_Controller6DofTest_unity_app_icon/res/mipmap-hdpi/wave_Controller6DofTest_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/Controller6DofTest/wave_Controller6DofTest_unity_app_icon/res/mipmap-xhdpi/wave_Controller6DofTest_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon4 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/Controller6DofTest/wave_Controller6DofTest_unity_app_icon/res/mipmap-xxhdpi/wave_Controller6DofTest_unity_app_icon.png", typeof(Texture2D));
		Texture2D icon5 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/Controller6DofTest/wave_Controller6DofTest_unity_app_icon/res/mipmap-xxxhdpi/wave_Controller6DofTest_unity_app_icon.png", typeof(Texture2D));

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
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
		PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel26;

		string outputFilePath = string.IsNullOrEmpty(destPath) ? apkName : destPath + "/" + apkName;
		BuildPipeline.BuildPlayer(levels, outputFilePath, BuildTarget.Android, run ? BuildOptions.AutoRunPlayer : BuildOptions.None);
	}
}
