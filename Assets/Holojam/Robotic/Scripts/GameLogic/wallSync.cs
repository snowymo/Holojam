using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallSync : MonoBehaviour
{

	public bool isReady;

	public Transform player;

	public Transform roomba;

	public syncRoomba communicator;

	public bool isPhone = false;

	//private float minDis = 0.01f, minAng = 1f;

	Vector3 startPoint;

	private float straightSpeed, turnSpeed;

	public float straightMax = 320;

	public int moveDoor;

	public envSwitchCtrl switchCtrl;

	// Use this for initialization
	void Start ()
	{
		isReady = true;
		moveDoor = 0;
		//straightMax = 320;

		startPoint = new Vector3 ();
		if (Mathf.Abs (player.position.z) < 2) {
			startPoint.z = player.position.z;
		}
	}

	void goStraight(Transform src, Vector3 vDes){
		Vector3 vCur = src.rotation * Vector3.forward;
		Vector3 vDis = vDes - src.position;
		vCur.y = 0;
		vDis.y = 0;
		float angle = Vector3.Angle (vCur, vDis);
		if ((angle > 90.0f) || (angle < -90.0f)) {
//			print ("go backward angle" + angle);
			straightSpeed = straightMax;
			communicator.setStraight (straightSpeed);
		} else {
//			print ("go forward + angle" + angle);
			straightSpeed = straightMax;
			communicator.setStraight (-straightSpeed);
		}
	}

	void stopRoomba(){
		// minus the speed slowly to protect the wall

	}

	bool turnAround(Transform src, Vector3 vDes){
		Vector3 vForward = src.rotation * Vector3.forward;
		Vector3 vDis = vDes - src.position;
		vForward.y = 0;
		vDis.y = 0;
		float angle = Vector3.Angle (vForward, vDis);

		if (angle > 90)
			angle = 180 - angle;
		if (Mathf.Abs (angle) < Utility.getInst ().angleRoombaError) {
			// direction is correct
			communicator.setTurn(0);
			return true;
		} else {
			print ("vForward:" + vForward);
			print ("vDis:" + vDis);
			Vector3 cross = Vector3.Cross (vForward.normalized, vDis.normalized);
			//print ("cross:" + cross + cross.y);
			//bool isClockwise = (Vector3.Cross (vForward, vDis).y < 0);
			if (cross.y < -0.00001 && cross.y > -0.99999) {
//				print ("turn right");
				communicator.setTurn (40);
			} else if (cross.y > 0.00001 && cross.y < 0.99999) {
//				print ("turn left");
				communicator.setTurn (-40);
			} else {
				communicator.setTurn(0);
				return true;
			}
		}
		return false;
	}

	void turnBack(bool forward){
		// turn qSrc back to 0,0,1
    if (forward) {
			print ("turn right");
			communicator.setTurn (40);
		} else {
			print ("turn left");
			communicator.setTurn (-40);
		}
	}

	void getReady ()
	{
		Utility.getInst ().drawRays (roomba.transform, startPoint);
		// close enough then check turn back
		if (Vector3.Distance (roomba.position, startPoint) < Utility.getInst ().disRoombaError) {
			isReady = turnAround (roomba,Vector3.forward);
		} else {
			// check facing direction
			Vector3 vForward = roomba.rotation * Vector3.forward;
			Vector3 vDis = startPoint - roomba.position;
			vForward.y = 0;
			vDis.y = 0;
			float angle = Vector3.Angle (vForward, vDis);
//			print ("angle:" + angle);
			if (angle > 90)
				angle = 180 - angle;
			if (Mathf.Abs (angle) > Utility.getInst ().angleRoombaError){
				turnAround (roomba, vDis);
			} else {
				// go straight
				goStraight(roomba,startPoint);
			}

		}
	}

	IEnumerator moveTheDoor(){
		Quaternion startRotate = roomba.rotation;
		Quaternion endRotate = roomba.rotation * Quaternion.Euler (0, 110, 0);
		while (true) {
			float angle = Quaternion.Angle (roomba.rotation, endRotate);
			print ("move the door:" + angle);
			if (angle < 20)
				break;
			communicator.setTurn (100);
			yield return new WaitForSeconds (0.3f);
			communicator.setStraight (30);
			yield return new WaitForSeconds (0.05f);
		}
		communicator.setTurn (0);
		communicator.setStraight (0);
		if(switchCtrl != null)
			switchCtrl.doit = true;
		// turn
		// wait 0.5 sec
		// go straight
		// wait 0.5 sec
		// turn
	}

  bool checkWallAngle(float angle){
    if (player.position.x > roomba.position.x) {
      angle -= 180f;
    }
    print("check wall angle:" + angle);
    angle = Mathf.Abs(angle);
    if (angle > Utility.getInst().angleRoombaError) {
      return true;
    } else
      return false;
  }
	
	// Update is called once per frame
	void Update ()
	{
		if (isPhone)
			return;
		
		// when not ready, move roomba to (0, 0, user.z), face -z or z
		if (!isReady) {
			if(roomba.gameObject.GetComponent<TrackableComponent>().Tracked && roomba.position.magnitude > 0 )
				getReady ();
		}
		else {
			if (roomba.gameObject.GetComponent<TrackableComponent> ().Tracked && player.gameObject.GetComponent<ActorAvatar> ().Tracked) {
				if (moveDoor == 0) {
					// move with player
          Vector3 des = new Vector3 (player.position.x, roomba.position.y, roomba.position.z);
					Utility.getInst ().drawRays (roomba.transform, des);
					float dis = Vector3.Distance (roomba.position, des);
//					print ("dis between player " + dis);
          // add for adjust the angle every frame
          float angle = Quaternion.Angle(Quaternion.identity,roomba.rotation);
//          print("current angle " + angle);
          if (checkWallAngle(angle)) {
            if (player.position.z < roomba.position.z) {
              angle -= 180f;
            }
            //turnBack(angle < 0);
          }
					 if (dis > Utility.getInst ().disRoombaError) {
						goStraight (roomba, des);
					} else {
            print("stop");
						communicator.setStraight (0);
					}
				}else{
					if (moveDoor == 1) {
						// turn when door get touched
						StartCoroutine (moveTheDoor ());
					}
					moveDoor = 2;
				}
			} else {
				communicator.setStraight (0);
			}
		}
	}
}
