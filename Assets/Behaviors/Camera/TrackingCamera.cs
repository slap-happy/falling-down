using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TrackingCamera : MonoBehaviour
{
	#region Attributes
	public bool lookAtTarget;
	public float trackingSpeed = 0.5f;
	public bool lockHorizontal;
	#endregion
	
	#region Unity
	/*
	 * Ensures that the component will be initialized to inactive.
	 */
	void Awake()
	{
		startingPosition = transform.position;
		enabled = false;
		SetupEffects();
	}
	
	void OnEnable()
	{
		PlayerInputController.OnInput += HandlePlayerInputControllerOnInput;
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
	}
	
	void OnDisable()
	{
		PlayerInputController.OnInput -= HandlePlayerInputControllerOnInput;
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
	}
	
	/**
	 * Lerps the camera position towards the target's vertical position by the distance from the target.
	 */
	void FixedUpdate()
	{
		Vector3 ourPosition = transform.position;
		Vector3 targetPosition = Target.position;
		targetPosition.z = startingPosition.z;
		if (lockHorizontal)
			targetPosition.x = startingPosition.x;
		
		transform.position = Vector3.SmoothDamp(ourPosition, targetPosition, ref currentVelocity, trackingSpeed);
		
		// experimental: causes the camera to pivot to face the target
		if (lookAtTarget)
			transform.LookAt(Target);
	}
	#endregion
	
	#region Actions
	public void SetPlayer(Player player)
	{
		SetTarget(player.transform);
		player.OnHitHazard += HandlePlayerOnHitHazard;
	}
	
	public void SetTarget(Transform target)
	{
		Target = target;
		if (Debug.isDebugBuild)
			Debug.Log(string.Format("Began tracking target, '{0}'.", Target.name));
	}
	
	public void RemovePlayer(Player player)
	{
		player.OnHitHazard -= HandlePlayerOnHitHazard;
		RemoveTarget();
	}
	
	public void RemoveTarget()
	{
		Target = null;
	}
	#endregion
	
	#region Handlers
	void HandleGameControllerOnGameStarted()
	{
		Vector3 currentPosition = transform.position;
		Vector3 placementPosition = new Vector3(currentPosition.x, Target.position.y, currentPosition.z);
		transform.position = placementPosition;
	}
	
	void HandlePlayerInputControllerOnInput (PlayerInput input)
	{
		EffectDelegate(new CameraEffectArgs
		{
			effectType = CameraEffectType.Speed,
			intensity = input.flareMagnitude,
		});
	}
	
	void HandlePlayerOnHitHazard(float relativeVelocity)
	{
		EffectDelegate(new CameraEffectArgs
		{
			effectType = CameraEffectType.Impact,
			intensity = relativeVelocity,
			duration = 0.2f,
		});
	}
	#endregion
	
	#region Private
	private Transform Target
	{
		get { return target; }
		set
		{
			target = value;
			enabled = target != null;
		}
	}
	
	private Dictionary<CameraEffectType, List<CameraEffect>> cameraEffects;
	private Vector3 startingPosition;
	private Vector3 currentVelocity;
	private Transform target;
	
	void EffectDelegate(CameraEffectArgs args)
	{
		CameraEffectType type = args.effectType;
		if (cameraEffects.ContainsKey(type))
			foreach (CameraEffect effect in cameraEffects[type])
				effect.Play(args);
	}
	
	void SetupEffects()
	{
		cameraEffects = new Dictionary<CameraEffectType, List<CameraEffect>>();
		CameraEffect[] effects = GetComponents<CameraEffect>() as CameraEffect[];
		foreach (CameraEffect e in effects)
		{
			CameraEffectType t = e.effectType;
			if (!cameraEffects.ContainsKey(t))
				cameraEffects.Add(t, new List<CameraEffect>());
			cameraEffects[t].Add(e);
		}
	}
	#endregion
}
