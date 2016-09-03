using UnityEngine;
using System.Collections;
using Holojam.Network;
using Holojam.Tools;

public class TrackableCamera : Trackable {

	public float damping;

	void Start(){
		damping = 3.0f;
	}

	protected override void UpdateTracking(){
		if(view.IsTracked){
			transform.position = Vector3.Lerp(transform.position, trackedPosition, Time.deltaTime * damping);
			transform.rotation = Quaternion.Slerp(transform.rotation, trackedRotation, Time.deltaTime);
		}
	}
}
