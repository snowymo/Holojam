using UnityEngine;
using System.Collections;

public class CarCtrl : MonoBehaviour {

	public GameObject referenceObj;

	private Vector3 lastRefPosition;

	private Quaternion lastRefRotation;

	private Vector3 lastPosition;

	private Quaternion lastRotation;

	private bool isLastRound;

	public float thescale;

	private int step;

	private int count;

	StepperCommunication serialCtrl;

	public bool isReadyToMove;

	bool testKey;

	public int debugCount = 5;

	public float disError;

	public float angleError;

	System.IO.StreamWriter file;

	TrackedCarC tc;

	// Use this for initialization
	void Start () {
		tc = gameObject.GetComponent<TrackedCarC> ();
		lastRefPosition = lastPosition = new Vector3 (0, 0, 0);
		lastRefRotation = lastRotation = Quaternion.identity;
		serialCtrl = StepperCommunication.getInstance();
		serialCtrl.open ();
		testKey = true;
		isLastRound = false;
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
		if ((thispos - refpos).magnitude < disError)
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
							step = 4;
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
							step = 4;
						} 
						break;
					case 3:
						turnBack ();
						if(isCloseEnough())
							step = 4;
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

	bool isLastLeft = true;
	void turnRobot(bool change){
		if (isLastLeft ^ change) {
			serialCtrl.right (1);
			isLastLeft = true;
		} else {
			serialCtrl.left (1);
			isLastLeft = false;
		}
	}

	// test my rotation ctrl
	void drawRays(){
		Quaternion facing = Quaternion.identity;
		facing.SetFromToRotation (transform.rotation * Vector3.forward, referenceObj.transform.position - transform.position);
		Vector3 vFacing = facing * Vector3.forward;
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
		//serialCtrl.median ();
		if(isLastRound && (transform.position.Equals(lastPosition)))
			return false;
		else{
			Quaternion facing = Quaternion.identity;
			facing.SetFromToRotation (transform.rotation * Vector3.forward, referenceObj.transform.position - transform.position);
			Vector3 vFacing = referenceObj.transform.position-this.transform.position;
			Vector3 vCur = transform.rotation * Vector3.forward;
			Vector3 vUp = Vector3.Cross (vCur, vFacing);

			float angle = Vector3.Angle(vCur, vFacing);
			print ("turnRound:\tvCur:\t" + vCur.ToString ("F2") + "\tvFacing:\t" + vFacing.ToString ("F2"));
			print ("turnRound:\tangle:\t" + angle);
			if (angle % 360.0f > 6.0f) {
				print("turnRound:\tupVector:\t" + vUp.ToString("F2"));
				if (vUp.y > 0.005)
					serialCtrl.right ((int)(angle * 0.15));
				else if (vUp.y < -0.005)
					serialCtrl.left ((int)(angle * 0.15));
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
		Vector3 dis = referenceObj.transform.position - transform.position;
		print ("goStraight\tdis:\t" + dis.ToString("F3") + "\tref:\t" + referenceObj.transform.position.ToString("F3") + "\tcur:\t" + transform.position.ToString("F3") + "\tlastDis:\t" + lastDis.ToString("F2"));
		if (dis.magnitude > disError) {
			Vector3 vCur = transform.rotation * Vector3.forward;
			Vector3 vUp = Vector3.Cross (dis, vCur);
			print ("goStraight\tvCur:\t" + vCur.ToString ("F2") + "\tvUp:\t" + vUp.ToString ("F2"));
			//file.WriteLine ("dis:\t" + dis.magnitude);
			if ((vCur.x * dis.x >= 0) || (vCur.z * dis.z >= 0))
				serialCtrl.forward ((int)(dis.magnitude * 150));
			else
				serialCtrl.backward ((int)(dis.magnitude * 150));
			return false;
		} else
			return true;
	}

	bool lastBack = false;
	void turnBack(){
		if (!lastBack || !lastRotation.Equals (transform.rotation)) {
			Vector3 vCur = transform.rotation * Vector3.forward;
			Vector3 vDes = referenceObj.transform.rotation * Vector3.forward;
			Vector3 vUp = Vector3.Cross (vCur, vDes);
			float angle = Vector3.Angle (vCur, vDes);
			if (angle > angleError) {
				if (vUp.y >= 0)
					serialCtrl.right (1);
				else
					serialCtrl.left (1);
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
				serialCtrl.right ((int)(angle * 0.15));
			else
				serialCtrl.left ((int)(angle * 0.15));
			print ("rot in turn back:\t" + transform.rotation);
			return false;
		} else
			return true;
	}
}
