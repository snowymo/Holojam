using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ctrlC : MonoBehaviour
{

	public GameObject carA1;
	public GameObject carA2ref;
	public GameObject carB1;
	public GameObject carB2;
	public GameObject carB2ref;
	public GameObject carA2;

	private Vector2 vLocal;
	private Vector2 vRemote;

	private bool isLastRound, isLastStraight;

	private m3piComm m3piCtrler;

	public int countNo;
	private int count;
	public int step;

	Vector3 lastPos;
	Quaternion lastRot;

	public string m_returnMsg;

	public bool enableTest;

	Vector3 defaultRBTB;
	float yThreshold;

	public string robotName;

	// Use this for initialization
	void Start ()
	{
		m3piCtrler = new m3piComm ();
		m3piCtrler.setName (robotName);
		count = 0;
		step = 1;
		countNo = 40;

		// initialize
		isLastRound = false;
		isLastStraight = false;
		lastPos = new Vector3 ();
		lastRot = Quaternion.identity;


		yThreshold = 0.07f;
		//sync (carB2, carA2);

		carB2ref.SetActive (false);
		//enableTest = true;

	}

	// only once
	int isFirst = 0;

	void myStart ()
	{
		if (isFirst == 2) {
			// record default height of ROBOT B
			defaultRBTB = carB2.transform.position;
			print ("default y\t" + defaultRBTB.y.ToString ("F3"));
			isFirst = 3;
		} else if (isFirst < 2)
			isFirst++;
	}

	void checkHeight ()
	{
		if (Vector3.Distance (defaultRBTB, new Vector3 ()) > 0.1) {
			if (Mathf.Abs (carB2.transform.position.y - defaultRBTB.y) > yThreshold) {
				print ("being hold.\t" + carB2.transform.position.y.ToString ("F3"));
				step = 0;
				this.GetComponent<SyncMsg> ().sentMsg = "stop";
				carB2ref.SetActive (true);
				carA2ref.SetActive (false);
			} else {
				this.GetComponent<SyncMsg> ().sentMsg = "sync";
				carB2ref.SetActive (false);
				carA2ref.SetActive (true);
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		// update them with offset
		//updateOffset ();
		// synchoronize A1 with B1 and sync A2 with B2 all the time
		//sync (carB1, carA1);

		if (Input.GetKeyDown (KeyCode.T))
			testKey = true;

		if (carB2.GetComponent<Holojam.Network.Controller> ().Tracked) {
			myStart ();
		}

		// keep sync
		if (step == 0) {
			step = 1;
		}

		// check height
		checkHeight ();

		if(isFirst == 3)
			sync (carB2, carA2);
	}

	bool isClose (Vector3 pos1, Vector3 pos2)
	{
		return Utility.getInst ().checkMatchV2 (pos1, pos2);
	}


	bool turnAround (GameObject local, GameObject remote, ref Vector3 lastPosition, ref Quaternion lastRotation)
	{
		// check if turn around last time and the distance is positive
		//if (isLastRound
		//	&& (Quaternion.Angle (local.transform.rotation, lastRotation) < 1)
		//)
		//	return false;
		//else {
		Vector3 localPos = local.transform.position;
		localPos.y = 0;
		Vector3 remotePos = remote.transform.position;
		remotePos.y = 0;
		//Quaternion facing = Quaternion.identity;
		//facing.SetFromToRotation (local.transform.rotation * Vector3.forward, remotePos - localPos);
		Vector3 vFacing = remotePos - localPos;
		Vector3 vCur = local.transform.rotation * Vector3.forward;
		vCur.y = 0;
		vFacing.y = 0;
		float angle = Vector3.Angle (vCur, vFacing);


		//print ("turnRound:\tvCur:\t" + vCur.ToString ("F2") + "\tvFacing:\t" + vFacing.ToString ("F2"));
		print ("turnRound:\tangle:\t" + angle);

		if (angle > 90.0f)
			angle = angle - 180.0f;
			
		if (Mathf.Abs (angle) % 180.0f > 6.0f) {
			Vector3 vUp = Vector3.Cross (vCur, vFacing);
			//print("turnRound:\tupVector:\t" + vUp.ToString("F2"));
			if (vUp.y > 0.00005)
				setAngle (false, angle, m3piCtrler);
			else if (vUp.y < -0.00005)
				setAngle (true, angle, m3piCtrler);
			else
				return true;
			isLastRound = true;
			return false;
		} else {
			isLastRound = false;
			return true;
		}
//			return true;
		//}
	}

	void setSpeedWaitHelp (m3piComm m3piCtrl, ref float dis, float thres, int sp, int wt, bool fw)
	{
		while (dis > thres) {
			m3piCtrl.setSpeed (sp);
			m3piCtrl.setWaitTime (wt);
			if (fw)
				m3piCtrl.forward ();
			else
				m3piCtrl.backward ();
			dis -= thres;
		}
	}

	void setSpeedWait (float dis, bool fw, m3piComm m3piCtrl)
	{
		for(int i = 0; i < m3piCtrl.posHelpArray.Length; i++)
			setSpeedWaitHelp (m3piCtrl, ref dis, 
				m3piCtrl.posHelpArray[i].dis, m3piCtrl.posHelpArray[i].sp, m3piCtrl.posHelpArray[i].wt, fw);

		m3piCtrl.run2 (Time.time);
		//m_returnMsg = m3piCtrlB.m_returnMsg;
		//Debug.Log ("receive from m3pi:\t" + m_returnMsg);
	}

	void setAngleHelp (m3piComm m3piCtrl, ref float angle, float thres, int sp, int wt, bool lft)
	{
		while (angle > thres) {
			m3piCtrl.setSpeed (sp);
			m3piCtrl.setWaitTime (wt);
			if (lft)
				m3piCtrl.left ();
			else
				m3piCtrl.right ();
			angle -= thres;
		}
	}

	void setAngle (bool lft, float angle, m3piComm m3piCtrl)
	{
		if (angle < 0)
			lft = !lft;
		angle = Mathf.Abs (angle);

		for(int i = 0; i < m3piCtrl.angleHelpArray.Length; i++)
			setAngleHelp (m3piCtrl, ref angle, 
				m3piCtrl.angleHelpArray[i].angle, m3piCtrl.angleHelpArray[i].sp, m3piCtrl.angleHelpArray[i].wt, lft);
		
		m3piCtrl.run2 (Time.time);
		//m_returnMsg = m3piCtrlB.m_returnMsg;
		//Debug.Log ("receive from m3pi:\t" + m_returnMsg);
	}

	bool goStraight (GameObject local, GameObject remote, ref Vector3 lastPosition)
	{
		Vector3 localPos = local.transform.position;
		Vector3 remotePos = remote.transform.position;
		localPos.y = 0;
		remotePos.y = 0;
		lastPosition.y = 0;

		// check if turn around last time and the distance is positive
		if (isLastStraight && (Vector3.Distance (localPos, lastPosition) < 0.0001f)) {
			isLastStraight = false;
			return false;
		}
		Vector3 dis = remotePos - localPos;
		print ("goStraight\tdis:\t" + dis.magnitude.ToString ("F3") + "\tref:\t" +
		remotePos.ToString ("F3") + "\tcur:\t" + localPos.ToString ("F3"));
		
		if (dis.magnitude > Utility.getInst ().disError) {
			Vector3 vCur = local.transform.rotation * Vector3.forward;
			//Vector3 vUp = Vector3.Cross (dis, vCur);
			//print ("goStraight\tvCur:\t" + vCur.ToString ("F2") + "\tvUp:\t" + vUp.ToString ("F2"));
			float angle = Vector3.Angle (vCur, dis);
			dis.y = 0;
			bool isForward = (vCur.x * dis.x >= 0) || (vCur.z * dis.z >= 0);
			if ((angle > 90.0f) || (angle < -90.0f))
				isForward = false;
			else
				isForward = true;
			setSpeedWait (dis.magnitude, isForward, m3piCtrler);
			isLastStraight = true;
			return false;
		} else {
			isLastStraight = false;
			return true;
		}
	}

	bool testKey = false;



	void sync (GameObject local, GameObject remote)
	{
		//vLocal = transform.rotation * Vector3.forward;
		// TODO: check if tracked
		if (!Utility.getInst().checkRtnMsg2 (m3piCtrler))
			return;
		
		Utility.getInst ().drawRays (local.transform, remote.transform);

		Vector3 localPos = local.transform.position;
		Vector3 remotePos = remote.transform.position;

		// ignore y information
		localPos.y = 0;
		remotePos.y = 0;

//		if (count++ != countNo)
//			return;
//		count = 0;

		if (enableTest) {
			if (!testKey)
				return;
			testKey = false;
		}

		if (step != 0) {
			//print ("step:\t" + step);
			switch (step) {
			case 0:
				break;
			case 1:
				// check distance first
				if (isClose (localPos, remotePos)) {
					step = 0;
					//m3piCtrler.stop ();
				} else {
					if (turnAround (local, remote, ref lastPos, ref lastRot)) {
						goStraight (local, remote, ref lastPos);
						step = 2;
					}
				}
				break;
			case 2:
				// moved car with going straight
				if (goStraight (local, remote, ref lastPos)) {
					step = 0;
				} else {
					step = 1;
				}
				break;
			default:
				break;
			}
			lastPos = local.transform.position;
			lastRot = local.transform.rotation;
		}
	}
	void OnDestroy(){
		
//		StreamSingleton.getInst().getReceiveThread().Abort ();
//		print ("destroy:\t" + StreamSingleton.getInst().getReceiveThread().ThreadState);
		StreamSingleton.getInst().minusThread();
	}

	//unused
	bool checkRtnMsg(){
		// check if there is return msg already
		float executeTime = Time.time - m3piCtrler.m_runTime;
		if (!m3piCtrler.m_bRtn) {
			// check if it is already too long then return and sync up them again
			if (executeTime < (m3piCtrler.m_cmdTime + 0.8f))
				return false;
			else {
				print ("wait too long:\t" + executeTime);
				m3piCtrler.m_exStop = true;
				return true;
			}
		}

		if(m3piCtrler.m_returnMsg.Length > 0)
			print (m3piCtrler.m_returnMsg);
		m3piCtrler.m_returnMsg = "";
		if(StreamSingleton.getInst().getReceiveThread() != null)
			StreamSingleton.getInst().getReceiveThread().Abort ();
		return true;
	}
}
