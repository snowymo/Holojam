using UnityEngine;
using System.Collections;

public class robotCtrl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void ignoreYPos (GameObject local, GameObject remote, ref Vector3 localPos, ref Vector3 remotePos)
	{
		// get local position
		localPos = local.transform.localPosition;
		remotePos = remote.transform.localPosition;
		// ignore y information
		localPos.y = 0;
		remotePos.y = 0;
	}

	protected void sync (GameObject local, GameObject remote, m3piComm m3piCtrl, ref int step)
	{
		if (!Utility.getInst ().checkRtnMsg2 (m3piCtrl))
			return;

		//		print ("local\t" + local.transform.position + "\t" + local.transform.localPosition);
		//		print ("remote\t" + remote.transform.position + "\t" + remote.transform.localPosition);
		Utility.getInst ().drawRays (local.transform, remote.transform, true);

		Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();
		ignoreYPos (local, remote, ref localPos, ref remotePos);

		m3piCtrl.clear ();

		// send command
		if (step != 0) {
			//print ("move index:\t" + index + "\tstep:\t" + step);
			switch (step) {
			case 1:
				// check distance first
				if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
					step = 0;
				} else {
					if (turnAround (local, remote, m3piCtrl, true)) {
						//						print ("move index:\t" + index + "\tstep:\t" + step);
						goStraight (local, remote, m3piCtrl, true);
						step = 2;
					}
				}
				break;
			case 2:
				// moved car with going straight
				if (goStraight (local, remote, m3piCtrl, true)) {
					step = 0;
					//print ("move index:\t" + index + "\tstep:\t" + step);
				} else {
					step = 1;
					//print ("move index:\t" + index + "\tstep:\t" + step);
				}
				break;
			default:
				break;
			}
		}
	}

	void setAngleHelp (m3piComm m3piCtrl, ref float angle, float thres, int sp, int wt, ref bool lft, bool abs = false)
	{
		//while (angle > thres) {
		while((!abs && (angle > thres)) || 
			(abs && (Mathf.Abs(angle - thres) < angle))){
			m3piCtrl.setSpeed (sp);
			m3piCtrl.setWaitTime (wt);
			if (lft)
				m3piCtrl.left ();
			else
				m3piCtrl.right ();
			angle -= thres;
			if (angle < 0)
				lft = !lft;
			angle = Mathf.Abs (angle);
		}

	}

	void setAngle (bool lft, float angle, m3piComm m3piCtrl)
	{
		if (angle < 0)
			lft = !lft;
		angle = Mathf.Abs (angle);

		for (int i = 0; i < m3piCtrl.angleHelpArray.Length; i++)
			setAngleHelp (m3piCtrl, ref angle, 
				m3piCtrl.angleHelpArray [i].angle, m3piCtrl.angleHelpArray [i].sp, m3piCtrl.angleHelpArray [i].wt, 
				ref lft,i == (m3piCtrl.angleHelpArray.Length-1));

		m3piCtrl.run2 (Time.time);

	}

	void setSpeedWaitHelp (m3piComm m3piCtrl, ref float dis, float thres, int sp, int wt, ref bool fw, bool abs = false)
	{
		while((!abs && (dis > thres)) || 
			(abs && (Mathf.Abs(dis - thres) < dis))){
//		while (dis > thres) {
			m3piCtrl.setSpeed (sp);
			m3piCtrl.setWaitTime (wt);
			if (fw)
				m3piCtrl.forward ();
			else
				m3piCtrl.backward ();
			dis -= thres;
			if (dis < 0)
				fw = !fw;
			dis = Mathf.Abs (dis);
		}

	}

	void setSpeedWait (float dis, bool fw, m3piComm m3piCtrl)
	{
		for (int i = 0; i < m3piCtrl.posHelpArray.Length; i++)
			setSpeedWaitHelp (m3piCtrl, ref dis, 
				m3piCtrl.posHelpArray [i].dis, m3piCtrl.posHelpArray [i].sp, m3piCtrl.posHelpArray [i].wt, 
				ref fw, i == (m3piCtrl.posHelpArray.Length-1));

		m3piCtrl.run2 (Time.time);
	}

	protected bool turnAround (GameObject local, GameObject remote, m3piComm m3piCtrl, bool isLocal = false)
	{
		Vector3 vFacing, vCur;
		if (isLocal) {
			vFacing = remote.transform.localPosition - local.transform.localPosition;
			vCur = local.transform.localRotation * Vector3.forward;
		} else {
			vFacing = remote.transform.position - local.transform.position;
			vCur = local.transform.rotation * Vector3.forward;
		}

		vCur.y = 0;
		vFacing.y = 0;
		float angle = Vector3.Angle (vCur, vFacing);

		if (angle > 90.0f)
			angle = angle - 180.0f;

		if (Mathf.Abs (angle) % 180.0f > Utility.getInst().angleError) {
			Vector3 vUp = Vector3.Cross (vCur, vFacing);
//			print ("turnAround:\t" + angle + "\tupVector:\t" + vUp.ToString ("F2"));
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
	protected bool goStraight (GameObject local, GameObject remote, m3piComm m3piCtrl, bool isLocal = false)
	{
		Vector3 localPos, remotePos;
		if (isLocal) {
			localPos = local.transform.localPosition;
			remotePos = remote.transform.localPosition;
		}
		else{
			localPos = local.transform.position;
			remotePos = remote.transform.position;
		}

		localPos.y = 0;
		remotePos.y = 0;

		Vector3 dis = remotePos - localPos;
//		print ("goStraight\tdis:\t" + dis.magnitude.ToString ("F3") + "\tref:\t" +
//			remotePos.ToString ("F3") + "\tcur:\t" + localPos.ToString ("F3"));

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

	void OnDestroy ()
	{
		StreamSingleton.getInst ().minusThread ();
		StreamSingleton.getInst ().minusThread (true);
	}
}
