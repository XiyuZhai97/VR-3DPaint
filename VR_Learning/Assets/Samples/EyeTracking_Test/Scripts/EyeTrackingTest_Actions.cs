using UnityEngine;
using UnityEngine.SceneManagement;
using WVR_Log;

public class EyeTrackingTest_Actions : MonoBehaviour
{
	private const string LOG_TAG = "EyeTracking_Test";
	private void DEBUG(string msg)
	{
		Log.d (LOG_TAG, msg);
	}

	public void TeleportRandomly () {
		Vector3 direction = UnityEngine.Random.onUnitSphere;
		direction.y = Mathf.Clamp (direction.y, 0.5f, 1f);
		direction.z = Mathf.Clamp (direction.z, 3f, 10f);
		float distance = 2 * UnityEngine.Random.value + 1.5f;
		transform.localPosition = direction * distance;
	}

	public void ExitGame()
	{
		DEBUG ("ExitGame()");
		Application.Quit();
	}

	public void BackToUpLayer()
	{
		DEBUG ("BackToUpLayer()");
		SceneManager.LoadScene (0);
	}
}
