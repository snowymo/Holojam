using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAndDoorCtrl : MonoBehaviour
{

	public Transform trackedRoomba;

	public wallSync syncWallInfo;

	public Vector3 offset;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (syncWallInfo.isReady) {
			if (syncWallInfo.moveDoor == 0) {
				float angle = Vector3.Angle (Vector3.forward, trackedRoomba.rotation * Vector3.forward);
//				print ("sync wall angle:" + angle + " trackedRoomba:" + trackedRoomba);
//				print ("position:" + transform.position);
				if (angle < 25) {
					transform.position = new Vector3 (trackedRoomba.position.x, transform.position.y, transform.position.z) + offset;
				} else if (angle > 155) {
					transform.position = new Vector3 (trackedRoomba.position.x, transform.position.y, transform.position.z) - offset;
				} else {
					transform.position = new Vector3 (trackedRoomba.position.x, transform.position.y, transform.position.z);
				}
//				transform.position = new Vector3 (trackedRoomba.position.x, transform.position.y, transform.position.z);
			}
		}
	}
}
