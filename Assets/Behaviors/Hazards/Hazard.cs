using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Hazard : MonoBehaviour
{
	#region Attributes
	public Transform toDirectionRef;
	public ForceMode launchForceMode;
	public float launchForce;
	#endregion
	
	#region Actions
	public virtual void ActivateWithArgs(DynamicHazardArgs args)
	{
		gameObject.SetActive(true);
		rigidbody.AddForce((args.toDirection - transform.position) * args.force, args.forceMode);
	}
	#endregion
}
