using UnityEngine;
using System.Collections;

public class cityRbtCtrl : robotCtrl
{

	public GameObject[] Rbts;
	private m3piComm[] m3piCtrls;
	public int[] step;
	int[] isFirst;

	int stableTimeCount = 10;
	public int[] stableTime;

	public int myTS;

	void createM3pi ()
	{
		m3piCtrls = new m3piComm[2];
		m3piCtrls [0] = new m3piComm ();
		m3piCtrls [0].setName ("A");
		m3piCtrls [1] = new m3piComm ();
		m3piCtrls [1].setName ("B");
		print ("create 2 ctrls");
	}

	void initialAttr ()
	{
		isFirst = new int[2];
		isFirst [0] = isFirst [1] = 0;

		//		lastPos = new Vector3[2];
		//		lastPos [0] = lastPos [1] = new Vector3 ();
		//		defaultRBT = new Vector3[2];
		//		defaultRBT [0] = defaultRBT [1] = new Vector3 ();
		//
		//		yThreshold = 0.07f;
		//		lastIdx = 1;

		step = new int[2];
		step [0] = step [1] = 0;
		stableTime = new int[2];
		stableTime [0] = stableTime [1] = 0;
	}

	// only once
	void myStart (int index, GameObject obj)
	{
		if (isFirst [index] == 2) {
			// record default height of ROBOT B
			//lastPos [index] = obj.transform.position;
			//defaultRBT [index] = obj.transform.position;
			//print ("default y\t" + index + "\t" + defaultRBT [index].y.ToString ("F3"));
			isFirst [index] = 3;
		} else if (isFirst [index] < 2)
			isFirst [index]++;
	}

	// Use this for initialization
	void Start ()
	{
		createM3pi ();
		initialAttr ();
	}

	int executeIndex = 0;
	Vector3 executeDest;

	// Update is called once per frame
	void Update ()
	{
		myTS = Utility.getInst ().getMyTS ();
		// check if tracked
		for (int index = 0; index < Rbts.Length; index++) {
			if (Rbts [index].GetComponent<Holojam.Network.Controller> ().Tracked)
				myStart (index, Rbts [index]);
		}
		// sync up
		if (isFirst [0] == 3 
		//	&& isFirst [1] == 3
		) {
			if (step [executeIndex] > 0) {
				// sync rbt in room A with beer in room B
				step [executeIndex] = Mathf.Max (step [executeIndex], 1);
				sync (Rbts [executeIndex], executeDest, executeIndex);
				//stableTime [executeIndex] = 0;
			}
			//if (step[executeIndex] == 0)
			//	++stableTime[i];
			if (step[executeIndex] == 0)
				foreach(NearestSelector ns in GetComponent<MapControl>().hands)
					ns.robotSynced = true;
		}
	}

	public bool hasArrived(){
		sync (Rbts [executeIndex], executeDest, executeIndex);
		if (step [executeIndex] == 0)
			return true;
		return false;
	}

	int getClosestRbt (Vector3 destination)
	{
		if (Rbts.Length == 1)
			return 0;
		if ((Vector3.Distance (Rbts [0].transform.position, destination)) <
		    (Vector3.Distance (Rbts [1].transform.position, destination)))
			return 0;
		else
			return 1;
	}

	public void setDestination (Vector3 dest)
	{
		// get the closest rbt index
		executeIndex = getClosestRbt (dest);
		// assign the destination to go
		step [executeIndex] = 1;
		executeDest = dest;
	}

	public Transform getExecuteRbt(){
		return Rbts [executeIndex].transform;
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

	protected void sync (GameObject local, Vector3 remote, int index, bool isLocal = false)
	{

		if (!Utility.getInst ().checkRtnMsg2 (m3piCtrls [index]))
			return;

		//		print ("local\t" + local.transform.position + "\t" + local.transform.localPosition);
		//		print ("remote\t" + remote.transform.position + "\t" + remote.transform.localPosition);
		Utility.getInst ().drawRays (local.transform, remote, isLocal);

		Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();
		ignoreYPos (local, remote, ref localPos, ref remotePos);

		m3piCtrls [index].clear ();

		// send command
		if (step [index] != 0) {
			switch (step [index]) {
			case 1:
				// check distance first
				if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
					step [index] = 0;
				} else {
					if (turnAround (local, remote, m3piCtrls [index], isLocal)) {
						print ("move index:\t" + index + "\tstep:\t" + step [index]);
						goStraight (local, remote, m3piCtrls [index], isLocal);
						step [index] = 2;
					}
				}
				break;
			case 2:
				// moved car with going straight
				if (goStraight (local, remote, m3piCtrls [index], isLocal)) {
					step [index] = 0;
					//print ("move index:\t" + index + "\tstep:\t" + step);
				} else {
					step [index] = 1;
					//print ("move index:\t" + index + "\tstep:\t" + step);
				}
				break;
			default:
				break;
			}
		}
	}
}
