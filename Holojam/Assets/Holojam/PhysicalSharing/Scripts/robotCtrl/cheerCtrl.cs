using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class cheerCtrl : MonoBehaviour {

	public GameObject A1;
	public GameObject B1;
	public GameObject RbtA;
	public GameObject RbtB;

	private m3piComm[] m3piCtrls;
	private m3piComm m3piCtrlA;
	private m3piComm m3piCtrlB;

	public int countNo;
	private int[] count;
	public int[] step;

	Vector3 lastPosA, lastPosB;
	Quaternion lastRotA, lastRotB;

	// Use this for initialization
	void Start () {
		m3piCtrls = new m3piComm[2];
		m3piCtrls[0] = new m3piComm ();
		m3piCtrls[0].setName ("A");
		m3piCtrls[1] = new m3piComm ();
		m3piCtrls[1].setName ("B");

		count = new int[2];
		count [0] = count [1] = 0;
		countNo = 20;

		step = new int[2];
		step [0] = step [1] = 0;
	}
	
	// Update is called once per frame
	void Update () {

		// keep sync
		if (step[0] == 0)
			step[0] = 1;
		if (step[1] == 0)
			step[1] = 1;
		
		sync (A1, RbtB, 1);
		sync (B1, RbtA, 0);
	}

	void ignoreYPos(GameObject local, GameObject remote, ref Vector3 localPos, ref Vector3 remotePos){
		localPos = local.transform.position;
		remotePos = remote.transform.position;
		// ignore y information
		localPos.y = 0;
		remotePos.y = 0;
	}

	void setAngleHelp(m3piComm m3piCtrl, ref float angle, float thres, int sp, int wt, bool lft){
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

		setAngleHelp(m3piCtrl, ref angle, 21.0f, 6, 1, lft);
		setAngleHelp(m3piCtrl, ref angle, 15.0f, 4, 1, lft);
		setAngleHelp(m3piCtrl, ref angle, 8.0f, 1, 1, lft);
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
			//print("turnRound:\tupVector:\t" + vUp.ToString("F2"));
			if (vUp.y > 0.00005)
				setAngle (false, angle,m3piCtrl);
			else if (vUp.y < -0.00005)
				setAngle (true, angle,m3piCtrl);
			else
				return true;
			return false;
		} else {
			return true;
		}
	}

	void setSpeedWaitHelp(m3piComm m3piCtrl, ref float dis, float thres, int sp, int wt, bool fw){
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

	void setSpeedWait (float dis, bool fw,m3piComm m3piCtrl)
	{
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.25f, 7, 8, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.13f, 6, 6, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.06f, 4, 3, fw);
		setSpeedWaitHelp (m3piCtrl, ref dis, 0.022f, 3, 2, fw);

		m3piCtrl.run ();
		//m_returnMsg = m3piCtrlB.m_returnMsg;
		//Debug.Log ("receive from m3pi:\t" + m_returnMsg);
	}

	bool goStraight (GameObject local, GameObject remote ,m3piComm m3piCtrl)
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
			float angle = Vector3.Angle(vCur,dis);
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

	void sync (GameObject local, GameObject remote, int index)
	{
		Vector3 localPos = new Vector3(), remotePos = new Vector3();

		Utility.getInst().drawRays (local.transform, remote.transform);

		ignoreYPos(local, remote, ref localPos, ref remotePos);

		// count for delay of serial control
		if (count[index]++ != countNo)
			return;
		count[index] = 0;

		// send command
		if (step[index] != 0) {
			print ("step:\t" + step[index]);
			switch (step[index]) {
			case 0:
				break;
			case 1:
				// check distance first
				if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
					step[index] = 0;
					m3piCtrlB.stop ();
				} else {
					if (turnAround (local, remote, m3piCtrls[index])) {
						goStraight (local, remote, m3piCtrls[index]);
						step[index] = 2;
					}
				}
				break;
			case 2:
				// moved car with going straight
				if (goStraight (local, remote , m3piCtrls[index])) {
					step[index] = 0;
				} 
				else {
					step[index] = 1;
				}
				break;
			default:
				break;
			}
		}
	}
}
