using UnityEngine;
using System.Collections;

public enum CameraEffectType { Impact, Speed };

public struct CameraEffectArgs
{
	public CameraEffectType effectType;
	public float intensity;
	public float duration;
}


public abstract class CameraEffect : MonoBehaviour
{
	public CameraEffectType effectType;
	public abstract void Play(CameraEffectArgs args);
}
