using UnityEngine;
using System.Collections;

public class TelekinesisBeerCtrl : robotCtrl
{

	public GameObject Rbt;
	// rbt[0] in room A
	public GameObject[] gestureHand;

	private m3piComm m3piCtrl;
	public int step;
	int isFirst;

	int stableTimeCount = 10;
	public int stableTime;

	public int myTS;

	public Vector3 destination;

	void createM3pi ()
	{
		m3piCtrl = new m3piComm ();
		m3piCtrl.setName ("B");
		print ("create ctrl");
	}

	void initialAttr ()
	{
		isFirst = 0;
		step = 0;
		stableTime = 0;
	}

	// only once
	void myStart ()
	{
		if (isFirst == 2) {
			isFirst = 3;
		} else if (isFirst < 2)
			isFirst++;
	}

	// Use this for initialization
	void Start ()
	{
		createM3pi ();
		initialAttr ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		myTS = Utility.getInst ().getMyTS ();
		// check if tracked
		if (Rbt.GetComponent<Holojam.Network.View> ().tracked)
			myStart ();
		
		// sync up
		if (isFirst == 3) {
			print ("step:\t" + step);
//			if ((step == 0 && (stableTime >= stableTimeCount))
//			    || step > 0) {
			if (step > 0) {
				// sync rbt in room A with beer in room B
				//step = Mathf.Max (step, 1);
				//TODO assign destination from outside
				sync (Rbt, destination);
				//stableTime = 0;
			}
			if (step == 0) {
				//++stableTime;
				foreach(GameObject gh in gestureHand)
					gh.GetComponent<GestureListener> ().robotSynced = true;
			}
		}
	}

	public void setDestination(Vector3 vec){
		destination = vec;
		step = 1;
	}

	protected void ignoreYPos (GameObject local, Vector3 remote, ref Vector3 localPos, ref Vector3 remotePos)
	{
		// get local position
		localPos = local.transform.position;
		remotePos = remote;
		// ignore y information
		localPos.y = 0;
		remotePos.y = 0;
	}

	protected void sync (GameObject local, Vector3 remote, bool isLocal = false)
	{

		if (!Utility.getInst ().checkRtnMsg2 (m3piCtrl))
			return;

		Utility.getInst ().drawRays (local.transform, remote, isLocal);

		Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();
		ignoreYPos (local, remote, ref localPos, ref remotePos);

		m3piCtrl.clear ();

		// send command
		if (step != 0) {
			switch (step) {
			case 1:
				// check distance first
				if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
					step = 0;
				} else {
					if (turnAround (local, remote, m3piCtrl, isLocal)) {
						//print ("move index:\t" + index + "\tstep:\t" + step);
						goStraight (local, remote, m3piCtrl, isLocal);
						step = 2;
					}
				}
				break;
			case 2:
				// moved car with going straight
				if (goStraight (local, remote, m3piCtrl, isLocal)) {
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
}
