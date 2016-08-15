using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ctrlC : MonoBehaviour
{

	public GameObject carA1;
	public GameObject carA2;
	public GameObject carB1;
	public GameObject carB2;

	private Vector2 vLocal;
	private Vector2 vRemote;

	private bool isLastRound,isLastStraight;

	private m3piComm m3piCtrlB;

	public int countNo;
	private int count;
	public int step;

	Vector3 lastPos;
	Quaternion lastRot;

	public string m_returnMsg;

	// Use this for initialization
	void Start ()
	{
		m3piCtrlB = new m3piComm ();
		m3piCtrlB.setName ("B");
		count = 0;
		step = 1;
		countNo = 20;

		// initialize
		isLastRound = false;
		isLastStraight = false;
		lastPos = new Vector3();
		lastRot = Quaternion.identity;

		//sync (carB2, carA2);
	}
	
	// Update is called once per frame
	void Update ()
	{
		// update them with offset
		//updateOffset ();
		// synchoronize A1 with B1 and sync A2 with B2 all the time
		//sync (carB1, carA1);

		//if (Input.GetKeyDown(KeyCode.T))
		//	testKey = true;
		if (step == 0)
			step = 1;
		sync (carB2, carA2);
	}



	void updateOffset ()
	{
		carA2.transform.position += Offset.getInst ().getOffset ();
		carB1.transform.position -= Offset.getInst ().getOffset ();
	}

	bool isClose (Vector3 pos1, Vector3 pos2)
	{
		return Utility.getInst ().checkMatchV2 (pos1, pos2);
	}

	void drawRays (Transform localTrans, Transform remoteTrans)
	{
		Quaternion facing = Quaternion.identity;
		facing.SetFromToRotation (localTrans.rotation * Vector3.forward, remoteTrans.position - localTrans.position);
		//Vector3 vFacing = facing * Vector3.forward;

		Vector3 vCur = localTrans.rotation * Vector3.forward;

		// test if these two vectors are correct
		Debug.DrawRay (localTrans.position, vCur, Color.green);

		Debug.DrawRay (localTrans.position, remoteTrans.position - localTrans.position, Color.magenta);

		//Debug.DrawRay(this.transform.position,vFacing,Color.red);

		Debug.DrawRay (localTrans.position, facing * new Vector3 (0, 0, -1), Color.cyan);
	}

	bool turnAround(GameObject local, GameObject remote, ref Vector3 lastPosition, ref Quaternion lastRotation){
		// check if turn around last time and the distance is positive
		if(isLastRound
			&& (Quaternion.Angle(transform.rotation,lastRotation) < 1))
			return false;
		else{
			Quaternion facing = Quaternion.identity;
			facing.SetFromToRotation (local.transform.rotation * Vector3.forward, remote.transform.position - local.transform.position);
			Vector3 vFacing = remote.transform.position-local.transform.position;
			Vector3 vCur = local.transform.rotation * Vector3.forward;
			vCur.y = 0;
			vFacing.y = 0;
			float angle = Vector3.Angle(vCur, vFacing);
			Vector3 vUp = Vector3.Cross (vCur, vFacing);

			print ("turnRound:\tvCur:\t" + vCur.ToString ("F2") + "\tvFacing:\t" + vFacing.ToString ("F2"));
			print ("turnRound:\tangle:\t" + angle);

			if(angle > 90.0f)
				angle = angle - 180.0f;
			
			if (Mathf.Abs(angle) % 180.0f > 6.0f) {
				//print("turnRound:\tupVector:\t" + vUp.ToString("F2"));
				if (vUp.y > 0.00005)
					setAngle (false, angle);
				else if (vUp.y < -0.00005)
					setAngle (true, angle);
				else
					return true;
				isLastRound = true;
				return false;
			} else {
				isLastRound = false;
				return true;
			}
//			return true;
		}
	}

	void setSpeedWait(float dis, bool fw){
		while (dis > 0.22) {
			m3piCtrlB.setSpeed (7);
			m3piCtrlB.setWaitTime (9);
			if (fw)
				m3piCtrlB.forward ();
			else
				m3piCtrlB.backward ();
			dis -= 0.21f;
		}
		while (dis > 0.12) {
			m3piCtrlB.setSpeed (6);
			m3piCtrlB.setWaitTime (6);
			if (fw)
				m3piCtrlB.forward ();
			else
				m3piCtrlB.backward ();
			dis -= 0.12f;
		}
		while (dis > 0.06) {
			m3piCtrlB.setSpeed (4);
			m3piCtrlB.setWaitTime (4);
			if (fw)
				m3piCtrlB.forward ();
			else
				m3piCtrlB.backward ();
			dis -= 0.06f;
		}
		while (dis > 0.02) {
			m3piCtrlB.setSpeed (3);
			m3piCtrlB.setWaitTime (2);
			if (fw)
				m3piCtrlB.forward ();
			else
				m3piCtrlB.backward ();
			dis -= 0.02f;
		}
		m3piCtrlB.run ();
		m_returnMsg = m3piCtrlB.m_returnMsg;
		Debug.Log("receive from m3pi:\t" + m_returnMsg);
	}

	void setAngle(bool lft, float angle){
		if (angle < 0)
			lft = !lft;
		angle = Mathf.Abs (angle);

		while (angle > 20.0f) {
			m3piCtrlB.setSpeed (6);
			m3piCtrlB.setWaitTime (1);
			if (lft)
				m3piCtrlB.left ();
			else
				m3piCtrlB.right ();
			angle -= 20.0f;
		}
		while (angle > 10.0f) {
			m3piCtrlB.setSpeed (4);
			m3piCtrlB.setWaitTime (1);
			if (lft)
				m3piCtrlB.left ();
			else
				m3piCtrlB.right ();
			angle -= 10.0f;
		}
		while (angle > 6.0f) {
			m3piCtrlB.setSpeed (1);
			m3piCtrlB.setWaitTime (1);
			if (lft)
				m3piCtrlB.left ();
			else
				m3piCtrlB.right ();
			angle -= 6.0f;
		}
		m3piCtrlB.run ();
		m_returnMsg = m3piCtrlB.m_returnMsg;
		Debug.Log("receive from m3pi:\t" + m_returnMsg);
	}

	bool goStraight(GameObject local, GameObject remote, ref Vector3 lastPosition ){
		Vector3 localPos = local.transform.position;	localPos.y = 0;
		Vector3 remotePos = remote.transform.position;	remotePos.y = 0;
		// check if turn around last time and the distance is positive
		if (isLastStraight && (Vector3.Distance (localPos, lastPosition) < 0.0001f)) {
			isLastStraight = false;
			return false;
		}
		Vector3 dis = remotePos - localPos;
		print ("goStraight\tdis:\t" + dis.magnitude.ToString("F3") + "\tref:\t" + 
			remote.transform.position.ToString("F3") + "\tcur:\t" + local.transform.position.ToString("F3"));
		
		if (dis.magnitude > Utility.getInst ().disError) {
			Vector3 vCur = local.transform.rotation * Vector3.forward;
			//Vector3 vUp = Vector3.Cross (dis, vCur);
			//print ("goStraight\tvCur:\t" + vCur.ToString ("F2") + "\tvUp:\t" + vUp.ToString ("F2"));
			bool isForward = (vCur.x * dis.x >= 0) || (vCur.z * dis.z >= 0);
			setSpeedWait (dis.magnitude, isForward);
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
		drawRays (local.transform, remote.transform);

		Vector3 localPos = local.transform.position;
		Vector3 remotePos = remote.transform.position;

		// ignore y information

		localPos.y = 0;
		remotePos.y = 0;

		if (count++ != countNo)
			return;
		count = 0;

//		if (!testKey)
//			return;
//		testKey = false;

		if (step != 0) {
			switch (step) {
			case 0:
				break;
			case 1:
			case 3:
				// check distance first
				if (isClose (localPos, remotePos)) {
					step = 0;
					m3piCtrlB.stop ();
				} else {
					++step;
				}
				break;
			case 2:
				// moved car with turning
				if (turnAround (local, remote,ref lastPos,ref lastRot))
					step = 3;
				// TODO DEBUG
				//step = 0;
				break;
			case 4:
				// moved car with going straight
				if (goStraight (local, remote, ref lastPos)) {
					step = 0;
				} else
					step = 2;
				break;
			default:
				break;
			}
			lastPos = local.transform.position;
			lastRot = local.transform.rotation;
		}
	}
}
