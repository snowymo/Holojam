using UnityEngine;
using System.Collections;

public class HPSCtrl : MonoBehaviour
{

	m3piComm _robotCtrl;
	int step;
	Vector3 _destination;
	wandCtrl wCtrl;
	public GameObject stylus;

	public bool _isViewer;

	// Use this for initialization
	void Start ()
	{
		_destination = new Vector3 ();
		if (!_isViewer) {
			_robotCtrl = new m3piComm ();
			_robotCtrl.setName ("B");
		}
		step = 1;
		wCtrl = stylus.GetComponent<wandCtrl> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		moveToDest ();
		// check if already arrived
		if (step == 0) {
			// ask magic obj to speed up
			print ("rbt has arrived");
			wCtrl.objChosen.setFinalDest (_destination);
			step = -1;
		}
	}

	public void setDestination (Vector3 dest)
	{
		print ("rbt started to move");
		_destination = dest;
		step = 1;
	}

	// ignore y information
	void ignoreYPos (GameObject local, Vector3 remote, ref Vector3 localPos, ref Vector3 remotePos)
	{
		localPos = local.transform.position;
		remotePos = remote;

		localPos.y = 0;
		remotePos.y = 0;
	}

	void moveToDest ()
	{
		if (_destination.sqrMagnitude > 0.1) {
			if(!_isViewer)
				if (!Utility.getInst ().checkRtnMsg2 (_robotCtrl))
					return;

			Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();
			//Utility.getInst ().drawRays (transform, _destination);
			ignoreYPos (gameObject, _destination, ref localPos, ref remotePos);

			if (step > 0) {
				switch (step) {
				case 1:
					// check distance first
					if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
						step = 0;
					} else {
						if(!_isViewer)
							if (turnAround (gameObject, remotePos, _robotCtrl)) {
								goStraight (gameObject, remotePos, _robotCtrl);
								step = 2;
							}
					}
					break;
				case 2:
					// viewer will never come here
					// moved car with going straight
					if (goStraight (gameObject, remotePos, _robotCtrl)) {
						step = 0;
					} else
						step = 1;
					break;
				default:
					break;
				}
			}
		}
	}

	// same as other control code
	bool goStraight (GameObject local, Vector3 remotePos, m3piComm m3piCtrl)
	{
		Vector3 localPos = local.transform.position;
		localPos.y = 0;

		Vector3 dis = remotePos - localPos;
		print ("goStraight\tdis:\t" + dis.magnitude.ToString ("F3") + "\tref:\t" +
		remotePos.ToString ("F3") + "\tcur:\t" + localPos.ToString ("F3"));

		if (dis.magnitude > (Utility.getInst ().disError * 1.5f)) {
			Vector3 vCur = local.transform.rotation * Vector3.forward;
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

	bool turnAround (GameObject local, Vector3 remotePos, m3piComm m3piCtrl)
	{
		Vector3 vFacing = remotePos - local.transform.position;
		Vector3 vCur = local.transform.rotation * Vector3.forward;
		vCur.y = 0;
		vFacing.y = 0;
		float angle = Vector3.Angle (vCur, vFacing);

		if (angle > 90.0f)
			angle = angle - 180.0f;

		if (Mathf.Abs (angle) % 180.0f > 8.0f) {
			Vector3 vUp = Vector3.Cross (vCur, vFacing);
			print ("turnAround:\t" + angle + "\tupVector:\t" + vUp.ToString ("F2"));
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

	void OnDestroy ()
	{
		//StreamSingleton.getInst ().minusThread ();
		StreamSingleton.getInst ().minusThread (true);
	}
}
