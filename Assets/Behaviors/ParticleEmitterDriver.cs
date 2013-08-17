using UnityEngine;
using System.Collections;

public class ParticleEmitterDriver : MonoBehaviour
{
	public enum WhenToEmit { Awake, OnEnable, Manual }
	#region Attributes
	public WhenToEmit whenToEmit;
	public int quantityToEmit;
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
		}
	}
	#endregion
}
