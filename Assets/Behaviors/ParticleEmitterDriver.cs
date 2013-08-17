using UnityEngine;
using System.Collections;

public class ParticleEmitterDriver : MonoBehaviour
{
	public enum WhenToEmit { Awake, OnEnable, Manual }
	public enum WhenToDestroy { OffScreen, Timed }
	#region Attributes
	public WhenToEmit whenToEmit;
	public WhenToDestroy whenToDestroy;
	public int quantityToEmit;
	public float duration;
	#endregion
	
	#region Unity
	void Awake()
	{
		emitter = GetComponent<ParticleEmitter>();
		
		if (whenToEmit == WhenToEmit.Awake)
			Emit(quantityToEmit);
	}
	
	void OnEnable()
	{
		if (whenToEmit == WhenToEmit.OnEnable)
			Emit(quantityToEmit);
	}
	
	void OnBecameInvisible()
	{
		if (whenToDestroy == WhenToDestroy.OffScreen)
			StopEmitting();
	}
	#endregion
	
	#region Private
	private ParticleEmitter emitter;
	
	void Emit(int quantity)
	{
		if (emitter == null)
		{
			if (Debug.isDebugBuild)
				Debug.LogError("No ParticleEmitter attached to this GameObject!", gameObject);
		}
		else
		{
			emitter.Emit(quantity);
			if (whenToDestroy == WhenToDestroy.Timed)
				Invoke("StopEmitting", duration);
		}
	}
	
	void StopEmitting()
	{
		SpawnPool.Instance.Destroy(gameObject);
	}
	#endregion
}
