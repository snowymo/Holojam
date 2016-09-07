using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class cheerCtrl : MonoBehaviour
{

	public GameObject A;
	public GameObject B;
	public GameObject RbtA;
	public GameObject RbtB;

	private m3piComm[] m3piCtrls;

	public int countNo;
	private int[] count;
	public int[] step;

	Vector3 lastPosA, lastPosB;
	Quaternion lastRotA, lastRotB;

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

		step = new int[2];
		step [0] = step [1] = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{

		// keep sync
		//print ("step A:\t" + step [0]);
		//print ("step B:\t" + step [1]);
		if (step [0] == 0)
			step [0] = 1;
		if (step [1] == 0)
			step [1] = 1;
		
		sync (RbtA, A, 0);
		sync (RbtB, B, 1);
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
		for (int i = 0; i < m3piCtrl.angleHelpArray.Length; i++) {
			setAngleHelp (m3piCtrl, ref angle, m3piCtrl.angleHelpArray [i].angle, m3piCtrl.angleHelpArray [i].sp, m3piCtrl.angleHelpArray [i].wt, lft);
		}
//		setAngleHelp (m3piCtrl, ref angle, 55.0f, 5, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 44.0f, 3, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 34.7f, 2, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 24.7f, 5, 1, lft);
//		setAngleHelp (m3piCtrl, ref angle, 17.98f, 3, 1, lft);
//		setAngleHelp (m3piCtrl, ref angle, 9.6f, 1, 1, lft);
//		setAngleHelp (m3piCtrl, ref angle, 5.0f, 0, 1, lft);
//		setAngleHelp (m3piCtrl, ref angle, 51.0f, 15, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 35.0f, 10, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 28f, 8, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 20f, 6, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 16f, 5, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 8f, 3, 2, lft);
//		setAngleHelp (m3piCtrl, ref angle, 3.4f, 2, 2, lft);
		m3piCtrl.run2 (Time.time);
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
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.21f, 20, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.17f, 15, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.11f, 15, 2, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.074f, 10, 2, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.04f, 5, 2, fw);
		for (int i = 0; i < m3piCtrl.posHelpArray.Length; i++)
			setSpeedWaitHelp (m3piCtrl, ref dis, m3piCtrl.posHelpArray [i].dis, m3piCtrl.posHelpArray [i].sp, m3piCtrl.posHelpArray [i].wt, fw);
		
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.19f, 25, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.167f, 20, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.126f, 15, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.093f, 10, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.06f, 6, 3, fw);
//		setSpeedWaitHelp (m3piCtrl, ref dis, 0.033f, 3, 3, fw);

		m3piCtrl.run2 (Time.time);
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

	void sync (GameObject local, GameObject remote, int index)
	{
		if (!Utility.getInst().checkRtnMsg2 (m3piCtrls[index]))
			return;
		
		Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();

		Utility.getInst ().drawRays (local.transform, remote.transform);

		ignoreYPos (local, remote, ref localPos, ref remotePos);

		// count for delay of serial control
//		if (count [index]++ != countNo)
//			return;
//		count [index] = 0;

		// send command
		if (step [index] != 0) {
			
			switch (step [index]) {
			case 0:
				break;
			case 1:
				// check distance first
				if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
					step [index] = 0;
					//m3piCtrls [index].stop ();
				} else {
					if (turnAround (local, remote, m3piCtrls [index])) {
						goStraight (local, remote, m3piCtrls [index]);
						step [index] = 2;
					}
				}
				break;
			case 2:
				// moved car with going straight
				if (goStraight (local, remote, m3piCtrls [index])) {
					step [index] = 0;
				} else {
					step [index] = 1;
				}
				break;
			default:
				break;
			}
		}
	}
	void OnDestroy(){
		StreamSingleton.getInst ().minusThread ();
		StreamSingleton.getInst ().minusThread ();
	}
}
