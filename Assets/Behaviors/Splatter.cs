using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splatter : SingletonMonobehaviour<Splatter>
{
	public enum Type { BloodSplatter };
	
	#region Actions
	public void Splat(Type type)
	{
		SpawnPool.Instance.Spawn(Resources.Load(type.ToString()) as GameObject, transform.position, Quaternion.identity);
	}
	#endregion
	
	#region Private
	private Dictionary<Type, ParticleEmitter> particleEmitters;
	
	private ParticleEmitter GetParticleEmitter(Type type)
	{
		if (particleEmitters == null)
			particleEmitters = new Dictionary<Type, ParticleEmitter>();
		else if (particleEmitters.ContainsKey(type))
			return particleEmitters[type];
		
		
//		GameObject go = Instantiate(Resources.Load(type.ToString())) as GameObject;
//		if (go != null)
//		{
//			go.transform.parent = transform;
//			go.transform.localScale = Vector3.one;
//			go.transform.localPosition = Vector3.zero;
//			ParticleEmitter ps = go.GetComponent<ParticleEmitter>();
//			particleEmitters.Add(type, ps);
//		}
//		else if (Debug.isDebugBuild)
//			Debug.LogError(string.Format("The Prefab {0} was not found in a Resources folder", type.ToString()));
		
		return particleEmitters[type];
	}
	#endregion
}
