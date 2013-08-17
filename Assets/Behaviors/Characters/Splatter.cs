using UnityEngine;
using System.Collections;

public class Splatter : SingletonMonobehaviour<Splatter>
{
	public enum Type { Blood };
	
	#region Attributes
	public ParticleSystem bloodSplatter;
	#endregion
	
	#region Actions
	public void Splat(Type type, float velocity)
	{
		switch (type)
		{
		case Type.Blood:
			int emitCount = Mathf.CeilToInt(velocity);
			bloodSplatter.Emit(emitCount);
			break;
		}
	}
	#endregion
}
