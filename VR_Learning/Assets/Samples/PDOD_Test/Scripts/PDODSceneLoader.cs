using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WVR_Log;

public class PDODSceneLoader : MonoBehaviour {

	private const string TAG = "PDODSceneLoader";
	private bool isSceneLoaded = false;
	private bool isLoadingScene = false;
	private string currentScene = "";

	public string backgroundScene = "";
	private int sceneIndex = -1;

	[SerializeField]
	private List<string> scenes = new List<string>();

	[Tooltip("Only for test.  Need set the load ScenePeriod in second.  Every loadScenePeriod second will load next scene")]
	public bool loadSceneInUpdate = false;
	[Range(5, 60)]
	public int loadScenePeriod = 8;
	private float sinceLastLoadingTime = 0;

	private void Update()
	{
		if (isLoadingScene)
			return;

		var unscaledDeltaTime = Time.unscaledDeltaTime;
		sinceLastLoadingTime += unscaledDeltaTime;
		if (sinceLastLoadingTime > loadScenePeriod)
		{
			sinceLastLoadingTime = 0;
			LoadNextSceneInternal();
		}
	}

	static PDODSceneLoader instance = null;
	public static void LoadNextScene()
	{
		if (instance)
			instance.LoadNextSceneInternal();
	}

	public void LoadNextSceneInternal()
	{
		StartCoroutine(LoadNextSceneCoroutine());
	}

	IEnumerator LoadNextSceneCoroutine()
	{
		if (scenes.Count == 0)
			yield break;

		while (isLoadingScene)
			yield return null;

		isLoadingScene = true;

		if (isSceneLoaded)
		{
			SceneManager.UnloadSceneAsync(scenes[sceneIndex]);
			isSceneLoaded = false;
		}

		sceneIndex++;
		if (sceneIndex < 0)
			sceneIndex = 0;
		if (sceneIndex == scenes.Count)
			sceneIndex = 0;

		currentScene = scenes[sceneIndex];
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive);

		while (!asyncLoad.isDone) {
			yield return null;
		}

		Log.d(TAG, "Scene " + currentScene + "is loaded.");
		isSceneLoaded = true;
		isLoadingScene = false;
	}

	static List<string> CheckScenes(List<string> scenes)
	{
		List<string> scenesInBuild = new List<string>();
		for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			int lastSlash = scenePath.LastIndexOf("/");
			string sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);
			if (scenes.Contains(sceneName))
				scenesInBuild.Add(sceneName);
		}

		Log.d(TAG, "Scenes exist in build list:");
		foreach (string n in scenesInBuild)
			Log.d(TAG, n);
		return scenesInBuild;
	}

	private void Awake()
	{
		if (instance != null)
		{
			enabled = false;
			return;
		}
		instance = this;

		if (!string.IsNullOrEmpty(backgroundScene))
			SceneManager.LoadSceneAsync(backgroundScene, LoadSceneMode.Additive);

		// Remove the scenes were not included in build.
		scenes = CheckScenes(scenes);

		// Load first scene
		isSceneLoaded = false;
		isLoadingScene = false;
		LoadNextSceneInternal();
	}
}
