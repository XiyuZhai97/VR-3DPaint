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

public class BuildVRTestApp
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
		PlayerSettings.Android.bundleVersionCode = 1;
		PlayerSettings.bundleVersion = "2.0.0";
		PlayerSettings.companyName = "HTC Corp.";
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
	}

	static void BuildApkAll()
	{
		CustomizedCommandLine();
		BuildApk(_destinationPath + "/", false, false, true, true, true);
		BuildApk(_destinationPath + "/armv7", false, false);
		BuildApk(_destinationPath + "/arm64", false, false, true, false, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build wvr_unity_vrtestapp.apk/32+64bit")]
	static void BuildApk()
	{
		BuildApk(null, false, false, true, true, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build wvr_unity_vrtestapp.apk/32bit")]
	static void BuildApk32()
	{
		BuildApk("armv7", false, false);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build wvr_unity_vrtestapp.apk/64bit")]
	static void BuildApk64()
	{
		BuildApk("arm64", false, false, true, false, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Run wvr_unity_vrtestapp.apk/32+64bit")]
	static void BuildAndRunApk()
	{
		BuildApk(null, true, false, true, true, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Run wvr_unity_vrtestapp.apk/32bit")]
	static void BuildAndRunApk32()
	{
		BuildApk("armv7", true, false);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Run wvr_unity_vrtestapp.apk/64bit")]
	static void BuildAndRunApk64()
	{
		BuildApk("arm64", true, false, true, false, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Dev+Run wvr_unity_vrtestapp.apk/32+64bit")]
	static void BuildDevAndRunApk()
	{
		BuildApk(null, true, true, true, true, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Dev+Run wvr_unity_vrtestapp.apk/32bit")]
	static void BuildDevAndRunApk32()
	{
		BuildApk("armv7", true, true);
	}

	[UnityEditor.MenuItem("WaveVR/Build Android APK/Build+Dev+Run wvr_unity_vrtestapp.apk/64bit")]
	static void BuildDevAndRunApk64()
	{
		BuildApk("arm64", true, true, true, false, true);
	}

	public static void BuildApk(string destPath, bool run, bool development, bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
	{
		string[] levels = {
			"Assets/Samples/VRTestApp/scenes/VRTestApp.unity",
			"Assets/Samples/SeaOfCube/scenes/Main.unity",
			"Assets/Samples/SeaOfCube/scenes/Main_Help.unity",
			"Assets/Samples/SeaOfCube/scenes/SeaOfCubeWithTwoHead.unity",
			"Assets/Samples/SeaOfCube/scenes/SeaOfCubeWithTwoHead_Help.unity",
			"Assets/Samples/CameraTexture_Test/scenes/CameraTexture_Test.unity",
			"Assets/Samples/PermissionMgr_Test/scenes/PermissionMgr_Test.unity",
			"Assets/Samples/MotionController_Test/Scenes/MotionController_Test.unity",
			"Assets/Samples/ControllerInputMode_Test/ControllerInputMode_Test.unity",
			"Assets/Samples/ControllerInputModule_Test/scenes/VRInputModule_Test.unity",
			"Assets/Samples/ControllerInputModule_Test/scenes/MouseInputModule_Test.unity",
			"Assets/Samples/InAppRecenter_Test/scene/InAppRecenter.unity",
			"Assets/Samples/Battery_Test/Scenes/Battery_Test.unity",
			"Assets/Samples/PassengerMode_Test/scenes/PassengerMode_Test.unity",
			"Assets/Samples/InteractionMode_Test/scene/InteractionMode_Test.unity",
			"Assets/Samples/Resource2_Test/Scenes/Resource_Test1.unity",
			"Assets/Samples/Resource2_Test/Scenes/Resource_Test1_Help.unity",
			"Assets/Samples/ScreenShot_Test/Scenes/ScreenShot_Test.unity",
			"Assets/Samples/RenderModel_Test/scenes/RenderModel_test.unity",
			"Assets/Samples/RenderModel_Test/scenes/RenderModel_scene2.unity",
			"Assets/Samples/RenderModel_Test/scenes/RenderModel_test_Help.unity",
			"Assets/Samples/ControllerInstanceMgr_Test/scenes/ControllerInstanceSence_test1.unity",
			"Assets/Samples/ControllerInstanceMgr_Test/scenes/ControllerInstanceSence_test2.unity",
			"Assets/Samples/ControllerInstanceMgr_Test/scenes/ControllerInstanceSence_test1_Help.unity",
			"Assets/Samples/ControllerTips_Test/Scenes/ControllerTips_Test.unity",
			"Assets/Samples/ControllerTips_Test/Scenes/ControllerTips_Test2.unity",
			"Assets/Samples/ControllerTips_Test/Scenes/ControllerTips_Test_Help.unity",
			"Assets/Samples/RenderMask_Test/Scene/RenderMask_Test.unity",
			"Assets/Samples/Teleport_Test/Scenes/Teleport_Test.unity",
			"Assets/Samples/Button_Test/Scenes/Button_Test.unity",
			"Assets/Samples/SoftwareIpd_Test/Scenes/SoftwareIpd_Test.unity",
			"Assets/Samples/DynamicResolution_Test/Scenes/DynamicResolutionScene1_Test.unity",
			"Assets/Samples/DynamicResolution_Test/Scenes/DynamicResolutionScene2_Test.unity",
			"Assets/Samples/SystemRecording_Test/Scenes/SystemRecording_Test.unity",
			"Assets/Samples/Overlay_Test/Scenes/Overlay_Test.unity",
			"Assets/Samples/PosePredict_Test/Scenes/PosePredict_Test.unity"
        };
		BuildApkInner(destPath, run, development, levels, isIL2CPP, isSupport32, isSupport64);
	}

	// Independent this function because the command-line need run this eariler than buildApk to take effect.
	[UnityEditor.MenuItem("WaveVR/Build Android APK/Apply VRTestApp PlayerSettings", priority = 0)]
	static void ApplyVRTestAppPlayerSettingsDefault()
	{
		ApplyVRTestAppPlayerSettings();
	}

	static void ApplyVRTestAppPlayerSettings(bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
	{
		Debug.Log("ApplyVRTestAppPlayerSettings");

		GeneralSettings();

		if (!isSupport32 && !isSupport64)
			isSupport32 = true;

		PlayerSettings.productName = "VRTestApp";

#if UNITY_5_6_OR_NEWER
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.vrm.unity.VRTestApp");
#else
		PlayerSettings.bundleIdentifier = "com.vrm.unity.VRTestApp";
#endif
		Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Samples/VRTestApp/Textures/test.png", typeof(Texture2D));
		if (icon == null)
			Debug.LogError("Fail to read app icon");

		Texture2D[] group = { icon, icon, icon, icon, icon, icon };

		PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, group);
		PlayerSettings.gpuSkinning = false;
#if UNITY_2017_2_OR_NEWER
		PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);
#else
		PlayerSettings.mobileMTRendering = true;
#endif
		PlayerSettings.graphicsJobs = true;

		// This can help check the Settings by text editor
		EditorSettings.serializationMode = SerializationMode.ForceText;

		// Enable VR support and singlepass
		WaveVR_Settings.SetVirtualRealitySupported(BuildTargetGroup.Android, true);
		var list = WaveVR_Settings.GetVirtualRealitySDKs(BuildTargetGroup.Android);
		if (!ArrayUtility.Contains<string>(list, WaveVR_Settings.WVRSinglePassDeviceName))
		{
			ArrayUtility.Insert<string>(ref list, 0, WaveVR_Settings.WVRSinglePassDeviceName);
		}
		WaveVR_Settings.SetVirtualRealitySDKs(BuildTargetGroup.Android, list);
		PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
		var symbols = WaveVR_Settings.GetDefineSymbols(BuildTargetGroup.Android);
		WaveVR_Settings.SetSinglePassDefine(BuildTargetGroup.Android, true, symbols);

		// Force use GLES31
		PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
		UnityEngine.Rendering.GraphicsDeviceType[] apis = { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 };
		PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, apis);
		PlayerSettings.openGLRequireES31 = true;
		PlayerSettings.openGLRequireES31AEP = true;

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

		AssetDatabase.SaveAssets();
	}

	private static void BuildApkInner(string destPath, bool run, bool development, string[] levels, bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
	{
		var apkName = "wvr_unity_vrtestapp.apk";
		ApplyVRTestAppPlayerSettings(isIL2CPP, isSupport32, isSupport64);

		string outputFilePath = string.IsNullOrEmpty(destPath) ? apkName 
			: destPath + "/" + apkName;
		BuildPipeline.BuildPlayer(levels, outputFilePath, BuildTarget.Android, (run ? BuildOptions.AutoRunPlayer : BuildOptions.None) | (development ? BuildOptions.Development : BuildOptions.None));
	}
}
