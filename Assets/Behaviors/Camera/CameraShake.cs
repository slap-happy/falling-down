using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	#region Attributes
	public float shakeRange;
	#endregion
	
	#region Unity
	void OnEnable()
	{
		GameController.OnGameEnded += HandleGameControllerOnGameEnded;
		Player.Instance.OnHitHazard += HandlePlayerOnHitHazard;
	}
	
	void OnDisable()
	{
		GameController.OnGameEnded -= HandleGameControllerOnGameEnded;
		Player.Instance.OnHitHazard -= HandlePlayerOnHitHazard;
	}
	#endregion
	
	#region Actions
	public void Shake(float intensity, float duration)
	{
		if (duration == 0)
			DoShake(intensity);
		else
			StartCoroutine(ShakeCoroutine(intensity, duration));
	}
	#endregion
	
	#region Handlers
	void HandleGameControllerOnGameEnded()
	{
		enabled = false;
	}
	
	void HandlePlayerOnHitHazard(float relativeVelocity)
	{
		Shake(relativeVelocity, 0.1f);
	}
	#endregion
	
	#region Private
	IEnumerator ShakeCoroutine(float intensity, float duration)
	{
		float endTime = Time.time + duration;
		while (Time.time < endTime)
		{
			DoShake(intensity);
			yield return null;
		}
	}
	
	void DoShake(float intensity)
	{
		Vector3 newPosition = transform.position;
		newPosition.x += Random.Range(-shakeRange, shakeRange) * intensity;
		newPosition.y += Random.Range(-shakeRange, shakeRange) * intensity;
		newPosition.z += Random.Range(-shakeRange, shakeRange) * intensity;
		transform.position = newPosition;
	}
	#endregion
}
