using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorCtrl : MonoBehaviour {

	public wallSync syncWallInfo;

	public Transform trackedRoomba;

	Vector3 lastPos;
	Quaternion lastRot;

  Vector3 offset;

	// Use this for initialization
	void Start () {
    //offset = new Vector3(-0.15f,0,0);
	}
	
	// Update is called once per frame
	void Update () {
		if (syncWallInfo.moveDoor > 0) {
			if (lastPos != new Vector3 ()) {
        Vector3 roombaPos = trackedRoomba.position;
        roombaPos.y = 0;
        //print("lastPos:" + lastPos);
        //print("roombaPos:" + roombaPos);
        transform.position = Vector3.Lerp (lastPos, roombaPos, Time.deltaTime);
        Vector3 tmp = transform.localPosition;
        tmp.y = 0;
        transform.localPosition = tmp;
        transform.rotation = Quaternion.Lerp (lastRot, trackedRoomba.rotation, Time.deltaTime);
        //print("transform.rotation:" + transform.rotation);
				//transform.Translate (trackedRoomba - lastPos);
				//transform.rotation = transform.rotation * (Quaternion.FromToRotation (lastRot, trackedRoomba.rotation));
			}

      float angle = Vector3.Angle (Vector3.forward, trackedRoomba.rotation * Vector3.forward);
      //        print ("sync wall angle:" + angle + " trackedRoomba:" + trackedRoomba);
      //        print ("position:" + transform.position);
      if (angle < 25) {
        lastPos = new Vector3 (trackedRoomba.position.x, 0, transform.position.z) + offset;
      } else if (angle > 155) {
        lastPos = new Vector3 (trackedRoomba.position.x, 0, transform.position.z) - offset;
      } else {
        lastPos = new Vector3 (trackedRoomba.position.x, 0, transform.position.z);
      }

			//lastPos = trackedRoomba.position;
			lastRot = trackedRoomba.rotation;
		} 
	}
}
