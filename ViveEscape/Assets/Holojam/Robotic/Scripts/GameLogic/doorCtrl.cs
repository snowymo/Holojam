using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorCtrl : MonoBehaviour {

	public wallSync syncWallInfo;

	public Transform trackedRoomba;

	Vector3 lastPos;
	Quaternion lastRot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (syncWallInfo.moveDoor > 0) {
			if (lastPos != new Vector3 ()) {
				transform.position = Vector3.Lerp (lastPos, trackedRoomba.position, Time.deltaTime);
				transform.rotation = Quaternion.Lerp (lastRot, trackedRoomba.rotation, Time.deltaTime);
				//transform.Translate (trackedRoomba - lastPos);
				//transform.rotation = transform.rotation * (Quaternion.FromToRotation (lastRot, trackedRoomba.rotation));
			}
			lastPos = trackedRoomba.position;
			lastRot = trackedRoomba.rotation;
		} 
	}
}
