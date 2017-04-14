using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class escapeCtrl : MonoBehaviour {

	public Transform trackedRoomba;

	public wallSync syncWallInfo;

	//public GameObject[] doors;

	//private int count;

	private float startTime, endTime;

	public envSwitchCtrl switchCtrl;

	// Use this for initialization
	void Start () {
		//count = 0;
		startTime = 0;
		endTime = 0;
//		win = false;
	}

	void OnCollisionEnter(Collision col){
		if(syncWallInfo.moveDoor == 0){
			if (col.transform.parent.name.Contains ("what")) {
				print ("collide enter" + col.transform.parent.name);
				if (startTime == 0) {
					startTime = Time.time;
				}
//					endTime = Time.time;
			}
		}
	}

	void OnCollisionStay(Collision col){
		if (syncWallInfo.moveDoor == 0) {
			if (col.transform.parent.name.Contains ("what")) {
				print ("collide stay" + col.transform.parent.name);
				endTime = Time.time;
				if (endTime - startTime > 6) {
					// move the door
					syncWallInfo.moveDoor = 1;
					//hit.transform.parent.gameObject.AddComponent<doorCtrl> ();
					col.transform.parent.gameObject.GetComponent<doorCtrl> ().enabled = true;
					if (col.transform.parent.name.Contains ("what")) {
						switchCtrl.win = true;
					}
					endTime = 0;
					startTime = 0;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
//		if (syncWallInfo.isReady) {
//			
//
//			// detect if user push the door
////			foreach (GameObject door in doors) {
////				if (Vector3.Distance (trackedHand, door.transform.position) < ) {
////					
////				}
////			}
//			if(syncWallInfo.moveDoor == 0){
//				transform.position = new Vector3 (trackedRoomba.position.x, transform.position.y, transform.position.z);
//				RaycastHit hit;
//				if(Physics.Raycast(trackedHand.position, Vector3.forward,out hit)){
//					print ("hit" + hit.transform.parent.name);
//					if (hit.transform.parent.name.Contains ("door")) {
//						if (startTime == 0) {
//							startTime = Time.time;
//						}
//						endTime = Time.time;
//					}
//				}
//				if (endTime - startTime > 3) {
//					// move the door
//					syncWallInfo.moveDoor = 1;
//					//hit.transform.parent.gameObject.AddComponent<doorCtrl> ();
//					hit.transform.parent.gameObject.GetComponent<doorCtrl>().enabled = true;
//					endTime = 0;
//					startTime = 0;
//				}
//			}
//		}
	}
}
