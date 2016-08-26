using UnityEngine;
using System.Collections;

public class cokeCtrl : MonoBehaviour
{

	public GameObject carA;
	public GameObject carRbtA;
	public GameObject carB;
	public GameObject carRbtB;

	public GameObject copyRbtA;
	public GameObject copyRbtB;

	public GameObject carRbtBRef;

	private m3piComm[] m3piCtrls;

	Vector3[] defaultRBT;
	Vector3[] lastPos;

	public int countNo;
	private int[] count;
	public int step;
	int[] isFirst;

	float yThreshold;
	int lastIdx;

	// Use this for initialization
	void Start ()
	{
		m3piCtrls = new m3piComm[2];
		m3piCtrls [0] = new m3piComm ();
		m3piCtrls [0].setName ("A");
		m3piCtrls [1] = new m3piComm ();
		m3piCtrls [1].setName ("B");

		count = new int[2];
		count [0] = count [1] = 0;
		countNo = 20;

		step = 1;

		isFirst = new int[2];
		isFirst [0] = isFirst [1] = 0;

		lastPos = new Vector3[2];
		lastPos [0] = lastPos [1] = new Vector3 ();
		defaultRBT = new Vector3[2];
		defaultRBT [0] = defaultRBT [1] = new Vector3 ();

		yThreshold = 0.07f;
		lastIdx = 0;
	}

	// only once
	void myStart (int index, GameObject obj)
	{
		if (isFirst [index] == 2) {
			// record default height of ROBOT B
			lastPos [index] = obj.transform.position;
			defaultRBT [index] = obj.transform.position;
			//print ("default y\t" + defaultRBTB.y.ToString ("F3"));
			isFirst [index] = 3;
		} else if (isFirst [index] < 2)
			isFirst [index]++;
	}

	void checkHeight (int index, GameObject obj, GameObject holdingObj)
	{
		if (Vector3.Distance (defaultRBT [index], new Vector3 ()) > 0.1) {
			if (Mathf.Abs (obj.transform.position.y - defaultRBT [index].y) > yThreshold) {
				print ("being hold.\t" + obj.transform.position.y.ToString ("F3"));
				step = 0;
				this.GetComponent<SyncMsg> ().sentMsg = "stop";
				holdingObj.SetActive (true);
				obj.SetActive (false);
			} else {
				this.GetComponent<SyncMsg> ().sentMsg = "sync";
				holdingObj.SetActive (false);
				obj.SetActive (true);
			}
		}
	}


	// Update is called once per frame
	void Update ()
	{
		if (carRbtA.GetComponent<Holojam.Network.HolojamView> ().IsTracked)
			myStart (0, carRbtA);
		
		if (carRbtB.GetComponent<Holojam.Network.HolojamView> ().IsTracked)
			myStart (1, carRbtB);

		// check the current pos with last pos to decide sync RBTA basedon RBTB or the other way
		if (isFirst [0] == 3 && isFirst [1] == 3) {
			// if previous state is sync up
			if (step == 0) {
				float moveB = Vector3.Distance (lastPos [0], carRbtB.transform.position);
				float moveA = Vector3.Distance (lastPos [1], carRbtA.transform.position);
				//Debug.Log ("last dis 0:\t" + lastPos [0].ToString("F4"));
				//Debug.Log ("last dis 1:\t" + lastPos [1].ToString("F4"));
				if (moveB > (moveA + Utility.getInst ().disError)) {
					Debug.Log ("move \"B\"");
					step = 1;
					sync (carRbtA, copyRbtB, 1);
					lastIdx = 1;
					// update previous location
					lastPos [0] = carRbtB.transform.position;
					lastPos [1] = carRbtA.transform.position;
				} else if (moveA > (moveB + Utility.getInst ().disError)) {
					Debug.Log ("move \"A\"");
					step = 1;
					sync (carRbtB, copyRbtA, 0);

					lastIdx = 0;
					// update previous location
					lastPos [0] = carRbtB.transform.position;
					lastPos [1] = carRbtA.transform.position;
				}
			} else {
				// still doing the sync up
				if (lastIdx == 0)
					sync (carRbtB, copyRbtA, 0);
				else
					sync (carRbtA, copyRbtB, 1);
				
				// update previous location
				lastPos [0] = carRbtB.transform.position;
				lastPos [1] = carRbtA.transform.position;
			}
		}



		// if RBT A is being hold, then change the model to coke
		checkHeight (0, carRbtA, carRbtBRef);
	}

	void ignoreYPos (GameObject local, GameObject remote, ref Vector3 localPos, ref Vector3 remotePos)
	{
		localPos = local.transform.position;
		remotePos = remote.transform.position;
		// ignore y information
		localPos.y = 0;
		remotePos.y = 0;
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

//		setAngleHelp (m3piCtrl, ref angle, 21.0f, 6, 1, lft);
//		setAngleHelp (m3piCtrl, ref angle, 15.0f, 4, 1, lft);
//		setAngleHelp (m3piCtrl, ref angle, 8.0f, 1, 1, lft);

		setAngleHelp (m3piCtrl, ref angle, 51.0f, 15, 2, lft);
		setAngleHelp (m3piCtrl, ref angle, 35.0f, 10, 2, lft);
		setAngleHelp (m3piCtrl, ref angle, 28f, 8, 2, lft);
		setAngleHelp (m3piCtrl, ref angle, 20f, 6, 2, lft);
		setAngleHelp (m3piCtrl, ref angle, 16f, 5, 2, lft);
		setAngleHelp (m3piCtrl, ref angle, 8f, 3, 2, lft);
		setAngleHelp (m3piCtrl, ref angle, 3.4f, 2, 2, lft);
		m3piCtrl.run ();
		//m_returnMsg = m3piCtrlB.m_returnMsg;
		//Debug.Log ("receive from m3pi:\t" + m_returnMsg);
	}

	bool turnAround (GameObject local, GameObject remote, m3piComm m3piCtrl)
	{
		Vector3 vFacing = remote.transform.position - local.transform.position;
		Vector3 vCur = local.transform.rotation * Vector3.forward;
		vCur.y = 0;
		vFacing.y = 0;
		float angle = Vector3.Angle (vCur, vFacing);

		if (angle > 90.0f)
			angle = angle - 180.0f;

		if (Mathf.Abs (angle) % 180.0f > 8.0f) {
			Vector3 vUp = Vector3.Cross (vCur, vFacing);
			print("turnRound:\tupVector:\t" + vUp.ToString("F2"));
			if (vUp.y > 0.00005)
				setAngle (false, angle, m3piCtrl);
			else if (vUp.y < -0.00005)
				setAngle (true, angle, m3piCtrl);
			else
				return true;
			return false;
		} else {
			return true;
		}
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
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.25f, 7, 8, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.13f, 6, 6, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.06f, 4, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.022f, 3, 2, fw);

		setSpeedWaitHelp (m3piCtrl, ref dis, 0.19f, 25, 3, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.167f, 20, 3, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.126f, 15, 3, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.093f, 10, 3, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.06f, 6, 3, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.033f, 3, 3, fw);

		m3piCtrl.run ();
		//m_returnMsg = m3piCtrlB.m_returnMsg;
		//Debug.Log ("receive from m3pi:\t" + m_returnMsg);
	}

	bool goStraight (GameObject local, GameObject remote, m3piComm m3piCtrl)
	{
		Vector3 localPos = local.transform.position;
		Vector3 remotePos = remote.transform.position;
		localPos.y = 0;
		remotePos.y = 0;

		Vector3 dis = remotePos - localPos;
		print ("goStraight\tdis:\t" + dis.magnitude.ToString ("F3") + "\tref:\t" +
		remotePos.ToString ("F3") + "\tcur:\t" + localPos.ToString ("F3"));

		if (dis.magnitude > Utility.getInst ().disError) {
			Vector3 vCur = local.transform.rotation * Vector3.forward;
			//Vector3 vUp = Vector3.Cross (dis, vCur);//print ("goStraight\tvCur:\t" + vCur.ToString ("F2") + "\tvUp:\t" + vUp.ToString ("F2"));
			float angle = Vector3.Angle (vCur, dis);
			bool isForward = (vCur.x * dis.x >= 0) || (vCur.z * dis.z >= 0);
			if ((angle > 90.0f) || (angle < -90.0f))
				isForward = false;
			else
				isForward = true;
			setSpeedWait (dis.magnitude, isForward, m3piCtrl);
			return false;
		} else {
			return true;
		}
	}

	bool checkRtnMsg(int index){
		// check if there is return msg already
		if (!m3piCtrls [index].m_bRtn)
			return false;

		if(m3piCtrls[index].m_returnMsg.Length > 0)
			print (m3piCtrls[index].m_returnMsg);
		m3piCtrls[index].m_returnMsg = "";

		return true;
	}

	void sync (GameObject local, GameObject remote, int index)
	{
		if (!checkRtnMsg (index))
			return;

		Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();

		Utility.getInst ().drawRays (local.transform, remote.transform);

		ignoreYPos (local, remote, ref localPos, ref remotePos);

		// count for delay of serial control
//		if (count [index]++ != countNo)
//			return;
//		count [index] = 0;

		// send command
		if (step != 0) {
			print ("index:\t" + index + "\tstep:\t" + step);
			switch (step) {
			case 1:
				// check distance first
				if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
					step = 0;
					//m3piCtrls [index].stop ();
				} else {
					if (turnAround (local, remote, m3piCtrls [index])) {
						goStraight (local, remote, m3piCtrls [index]);
						step = 2;
					}
				}
				break;
			case 2:
				// moved car with going straight
				if (goStraight (local, remote, m3piCtrls [index])) {
					step = 0;
				} else {
					step = 1;
				}
				break;
			default:
				break;
			}
		}
	}
}
