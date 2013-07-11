using UnityEngine;
using System.Collections;

[RequireComponent(typeof (AudioSource))]
public class ActerAudioController : MonoBehaviour
{
	#region Attributes
	public AudioClip splatSound;
	
	public float playSplatSoundVelocityDelta;
	#endregion
	
	#region Unity
	void Awake()
	{
		player = GetComponent<Player>();
		player.OnHitHazard += HandlePlayerOnHitHazard;
	}
	#endregion
	
	#region Handlers
	void HandlePlayerOnHitHazard(float relativeVelocity)
	{
		if (relativeVelocity > playSplatSoundVelocityDelta)
		{
			audio.volume = relativeVelocity * 0.1f; // temp magic number
			audio.PlayOneShot(splatSound);
		}
	}
	#endregion
	
	#region Private
	private Player player;
	#endregion
}
