using UnityEngine;
using System.Collections;

public class robotCtrl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

		for (int i = 0; i < m3piCtrl.angleHelpArray.Length; i++)
			setAngleHelp (m3piCtrl, ref angle, 
				m3piCtrl.angleHelpArray [i].angle, m3piCtrl.angleHelpArray [i].sp, m3piCtrl.angleHelpArray [i].wt, lft);

		m3piCtrl.run2 (Time.time);

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
		for (int i = 0; i < m3piCtrl.posHelpArray.Length; i++)
			setSpeedWaitHelp (m3piCtrl, ref dis, 
				m3piCtrl.posHelpArray [i].dis, m3piCtrl.posHelpArray [i].sp, m3piCtrl.posHelpArray [i].wt, fw);

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

		if (Mathf.Abs (angle) % 180.0f > 8.0f) {
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
}
