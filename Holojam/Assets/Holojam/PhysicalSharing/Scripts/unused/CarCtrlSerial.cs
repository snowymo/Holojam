﻿using UnityEngine;
using System.Collections;

public class CarCtrlSerial : MonoBehaviour {

	public GameObject referenceObj;

	private Vector3 lastRefPosition;

	private Quaternion lastRefRotation;

	private Vector3 lastPosition;

	private Quaternion lastRotation;

	private bool isLastRound;

	bool isLastStraight;

	public float thescale;

	private int step;

	private int count;

	SerialCommunication serialCtrl;

	public bool isReadyToMove;

	bool testKey;

	public int debugCount = 120;



	public float angleError;

	System.IO.StreamWriter file;

	TrackedCarC tc;

	// Use this for initialization
	void Start () {
		tc = gameObject.GetComponent<TrackedCarC> ();
		lastRefPosition = lastPosition = new Vector3 (0, 0, 0);
		lastRefRotation = lastRotation = Quaternion.identity;
		serialCtrl = SerialCommunication.getInstance();
		serialCtrl.open ();
		testKey = true;
		isLastRound = false;
		isLastStraight = false;
		step = 0;
		count = 0;



		//serialCtrl.median ();
//		System.IO.FileStream fs;
//		if(!System.IO.File.Exists(@"TestCarAngle.txt"))
//			fs = System.IO.File.Create(@"TestCarAngle.txt");
//		file = 
//			System.IO.File.AppendText(@"TestCarAngle.txt");


	}

	void testRefPosRot(){
		this.transform.rotation = Quaternion.Inverse (lastRefRotation) * referenceObj.transform.rotation*this.transform.rotation;
		this.transform.Translate (referenceObj.transform.position - lastRefPosition);
	}

	bool isCloseEnough(){
		Vector3 thispos = new Vector3(this.transform.position.x,0,this.transform.position.z);
		Vector3 refpos = new Vector3(referenceObj.transform.position.x,0,referenceObj.transform.position.z);
		//print("isCloseEnough:\tdis:\t" + ((thispos - refpos).magnitude));
		if ((thispos - refpos).magnitude < Utility.getInst().disError)
			return true;
		return false;

	}

	Vector3 vLast = new Vector3();
	Vector3 vCur;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Q))
			testKey = true;
		if (Input.GetKey (KeyCode.Escape)) {
			file.Close ();
			Application.Quit();
		}

		vCur = transform.rotation * Vector3.forward;
		//file.WriteLine("cur dis:\t" + Vector3.Distance (vLast, vCur));
		vLast = vCur;
		if(tc.isReadyToMove){
			// move according to invisible tracked objects
			if(!lastRefPosition.Equals(new Vector3(0,0,0))){
				drawRays();			// test rotation
				// move as reference move
				if (testKey) {
					// do it every 10 frames
					++count;
					if (count != debugCount) {
						return;
					}
					count = 0;
					testKey = true;

					vCur = transform.rotation * Vector3.forward;
					print ("update:\tvCur:\t" + vCur.ToString ("F4") );
					print ("update:\tangle:\t" + Vector3.Angle (vLast, vCur));
					// write to the file
					//file.WriteLine("cur angle:\t" + Vector3.Angle (vLast, vCur));
					//file.WriteLine("cur dis:\t" + Vector3.Distance (vLast, vCur));
					vLast = vCur;

					switch (step) {
					case 0:
						if (isCloseEnough ())
							step = 0;
						else {
							if (turnRound ()) {
								++step;
								// for test
								//step = 0;
							}
						}
						break;
					case 1:
						if (goStraight ()) {
							++step;
						} else
							step = 0;
						break;
					case 2:
						if (isCloseEnough ()) {
							// do not need to turn to the right face
							step = 0;
						} 
						break;
					case 3:
						turnBack ();
						if(isCloseEnough())
							// do not need to turn to the right face
							step = 0;
						break;
					case 4:
						if (turnFace ()) {
							step = 0;
						}
						break;
					default:
						break;
					}
				}
			}
			//testKey = false;
			lastRefPosition = referenceObj.transform.position;
			lastRefRotation = referenceObj.transform.rotation;
			lastPosition = transform.position;
			lastRotation = transform.rotation;
			//print ("pos:\t" + transform.position);
			//testKey = false;
		}
	}

	// test my rotation ctrl
	void drawRays(){
		Quaternion facing = Quaternion.identity;
		facing.SetFromToRotation (transform.rotation * Vector3.forward, referenceObj.transform.position - transform.position);
		//Vector3 vFacing = facing * Vector3.forward;
		Vector3 vCur = transform.rotation * Vector3.forward;
		// test if these two vectors are correct
		Debug.DrawRay(this.transform.position,vCur,Color.green);
		Debug.DrawRay(this.transform.position,referenceObj.transform.position-this.transform.position,Color.magenta);
		//Debug.DrawRay(this.transform.position,vFacing,Color.red);
		Debug.DrawRay(this.transform.position,facing * new Vector3(0,0,-1),Color.cyan);
		//print ("this:\t" + transform.position.ToString ("F2") + "ref:\t" + referenceObj.transform.position.ToString ("F2"));
	}

	//step 0: facing the destination
	float lastAngle = 180;
	bool turnRound(){
		Debug.LogWarning ("islastround:\t" + isLastRound + "\tdis:\t" + Vector3.Distance (transform.position, lastPosition)
		+ "\trot:\t" + Quaternion.Angle (transform.rotation, lastRotation));
		if(isLastRound && (Vector3.Distance(transform.position,lastPosition) < 0.0001f) 
			&& (Quaternion.Angle(transform.rotation,lastRotation) < 1))
			return false;
		else{
			Quaternion facing = Quaternion.identity;
			facing.SetFromToRotation (transform.rotation * Vector3.forward, referenceObj.transform.position - transform.position);
			Vector3 vFacing = referenceObj.transform.position-this.transform.position;
			Vector3 vCur = transform.rotation * Vector3.forward;
			float angle = Vector3.Angle(vCur, vFacing);
			Vector3 vUp = Vector3.Cross (vCur, vFacing);

			print ("turnRound:\tvCur:\t" + vCur.ToString ("F2") + "\tvFacing:\t" + vFacing.ToString ("F2"));
			print ("turnRound:\tangle:\t" + angle);

			if(angle > 90.0f)
				angle = angle - 180.0f;
			if (angle % 180.0f > 6.0f) {
				//print("turnRound:\tupVector:\t" + vUp.ToString("F2"));
				if (vUp.y > 0.005)
					serialCtrl.right (angle);
					//serialCtrl.right ();
				else if (vUp.y < -0.005)
					serialCtrl.left (angle);
					//serialCtrl.left ();
				else
					return true;
				lastAngle = angle;
				isLastRound = true;
				return false;
			} else {
				isLastRound = false;
				return true;
			}
		}

	}

	// step 1: go to the destination
	float lastDis = 180;

	bool goStraight(){
		//serialCtrl.median ();
		if (isLastStraight && (Vector3.Distance (transform.position, lastPosition) < 0.0001f)
		   && (Quaternion.Angle (transform.rotation, lastRotation) < 1)) {
			isLastStraight = false;
			return false;
		}
		Vector3 dis = referenceObj.transform.position - transform.position;
		print ("goStraight\tdis:\t" + dis.ToString("F3") + "\tref:\t" + referenceObj.transform.position.ToString("F3") + "\tcur:\t" + transform.position.ToString("F3") + "\tlastDis:\t" + lastDis.ToString("F2"));
		if (dis.magnitude > Utility.getInst ().disError) {
			Vector3 vCur = transform.rotation * Vector3.forward;
			Vector3 vUp = Vector3.Cross (dis, vCur);
			print ("goStraight\tvCur:\t" + vCur.ToString ("F2") + "\tvUp:\t" + vUp.ToString ("F2"));
			if ((vCur.x * dis.x >= 0) || (vCur.z * dis.z >= 0))
				serialCtrl.forward (dis.magnitude);
				//serialCtrl.forward ();
			else
				serialCtrl.backward (dis.magnitude);
			//serialCtrl.backward ();
			isLastStraight = true;
			return false;
		} else {
			isLastStraight = false;
			return true;
		}
	}

	bool lastBack = false;
	void turnBack(){
		if(!lastBack || (Vector3.Distance(transform.position,lastPosition) >= 0.0001f) 
			&& (Quaternion.Angle(transform.rotation,lastRotation) >= 1))
		//if (!lastBack || !lastRotation.Equals (transform.rotation)) 
		{
			Vector3 vCur = transform.rotation * Vector3.forward;
			Vector3 vDes = referenceObj.transform.rotation * Vector3.forward;
			Vector3 vUp = Vector3.Cross (vCur, vDes);
			float angle = Vector3.Angle (vCur, vDes);
			if (angle > angleError) {
				if (vUp.y >= 0)
					serialCtrl.right (Mathf.Abs(angle));
				else
					serialCtrl.left (Mathf.Abs(angle));
				print ("rot in turn back:\t" + transform.rotation);
				lastBack = true;
			} else
				lastBack = false;
		}
	}

	float lastAngle2 = 180;
	bool turnFace(){
		Vector3 vCur = transform.rotation * Vector3.forward;
		Vector3 vDes = referenceObj.transform.rotation * Vector3.forward;
		Vector3 vUp = Vector3.Cross (vCur, vDes);
		float angle = Vector3.Angle (vCur, vDes);
		print ("turnFace:\tangle:\t" + angle);
		//file.WriteLine ("angle:\t" + angle);
		if (angle > angleError) {
			if (vUp.y > 0)
				serialCtrl.right (Mathf.Abs(angle));
			else
				serialCtrl.left (Mathf.Abs(angle));
			print ("rot in turn back:\t" + transform.rotation);
			return false;
		} else
			return true;
	}
}
