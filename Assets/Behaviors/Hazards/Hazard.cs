using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Hazard : MonoBehaviour
{
	#region Actions
	public virtual void ActivateWithArgs(DynamicHazardArgs args)
	{
		gameObject.SetActive(true);
		rigidbody.AddForce((args.toDirection - transform.position), args.forceMode);
	}
	#endregion
}
