using UnityEngine;
using System.Collections;

public class CameraShake : CameraEffect
{
	#region Attributes
	public float shakeRange;
	#endregion
	
	#region Unity
	void Awake()
	{
		effectType = CameraEffectType.Impact;
	}
	
	void OnEnable()
	{
		GameController.OnGameEnded += HandleGameControllerOnGameEnded;
	}
	
	void OnDisable()
	{
		GameController.OnGameEnded -= HandleGameControllerOnGameEnded;
	}
	#endregion
	
	#region Actions
	public override void Play(CameraEffectArgs args)
	{
		StartCoroutine(Shake(args));
	}
	#endregion
	
	#region Handlers
	void HandleGameControllerOnGameEnded()
	{
		enabled = false;
	}
	#endregion
	
	#region Private
	private IEnumerator Shake(CameraEffectArgs args)
	{
		if (args.duration == 0)
		{
			DoShake(args.intensity);
		}
		else
		{
			float endTime = Time.time + args.duration;
			while (Time.time < endTime)
			{
				DoShake(args.intensity);
				yield return null;
			}
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
