using UnityEngine;
using System.Collections;

/*
 * Spawns trash ahead of the player
 */

public class TrashSpawner : MonoBehaviour
{
	public float frequency;
	public GameObject[] newspapers;
	
	void Awake()
	{
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
	}
	
	void HandleGameControllerOnGameStarted()
	{
		StartCoroutine(SpawnNewspapers());
	}
	
	IEnumerator SpawnNewspapers()
	{
		WaitForSeconds wait = new WaitForSeconds(frequency);
		while(true)
		{
			Vector3 spawnPosition = FindRandomPositionOnTrackBelowCameraFrustrum();
			int prefabToSpawn = Random.Range(0, newspapers.Length);
			SpawnPool.Instance.Spawn(newspapers[prefabToSpawn], spawnPosition, Random.rotation);
			yield return wait;
		}
	}
	
	Vector3 FindRandomPositionOnTrackBelowCameraFrustrum()
	{
		return Camera.mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, 25), Random.Range(5, 40)));
	}
}
