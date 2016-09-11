using UnityEngine;
using System.Collections;

public class boardRbtCtrl : robotCtrl
{

	public GameObject RbtA;
	public GameObject RbtB;
	private GameObject[] Rbts;

	private m3piComm[] m3piCtrls;
	public int step;
	int[] isFirst;

	float yThreshold;
	int lastIdx;
	Vector3[] lastPos;
	Vector3[] defaultRBT;

	int stableTimeCount = 10;
	public int stableTime;

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

		lastPos = new Vector3[2];
		lastPos [0] = lastPos [1] = new Vector3 ();
		defaultRBT = new Vector3[2];
		defaultRBT [0] = defaultRBT [1] = new Vector3 ();

		yThreshold = 0.07f;
		lastIdx = 1;

		Rbts = new GameObject[2];
		Rbts [0] = RbtA;	//old
		Rbts [1] = RbtB;	//new

		step = 1;

		stableTime = 0;
	}

	// only once
	void myStart (int index, GameObject obj)
	{
		if (isFirst [index] == 2) {
			// record default height of ROBOT B
			lastPos [index] = obj.transform.position;
			defaultRBT [index] = obj.transform.position;
			print ("default y\t" + index + "\t" + defaultRBT [index].y.ToString ("F3"));
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
	
	// Update is called once per frame
	void Update ()
	{
		myTS = Utility.getInst ().getMyTS ();
		// check if tracked
		for (int index = 0; index < 2; index++) {
			if (Rbts [index].GetComponent<Holojam.Network.HolojamView> ().IsTracked)
				myStart (index, Rbts [index]);
		}
		// sync up
		if (isFirst [0] == 3 && isFirst [1] == 3) {
			// if previous state is sync up
//			print("stable count:\t" + stableTime);
			if (step == 0 && (stableTime == stableTimeCount)) {
				float moveOld = Vector3.Distance (lastPos [0], Rbts [0].transform.position);
				float moveNew = Vector3.Distance (lastPos [1], Rbts [1].transform.position);
				//print ("old diff:\t" + moveOld + "\tnew diff:\t" + moveNew);
				if (moveOld > (moveNew + Utility.getInst ().disError)) {
					//Debug.Log ("move \"New\"" + Rbts [1].transform.localPosition + "\t" + Rbts [0].transform.localPosition);
					step = 1;
					sync (Rbts [1], Rbts [0], m3piCtrls[1],ref step);
					updatePosnIdx(1);
				} else if (moveNew > (moveOld + Utility.getInst ().disError)) {
					//Debug.Log ("move \"Old\"");
					step = 1;
					sync (Rbts [0], Rbts [1], m3piCtrls[0],ref step);
					// update previous location
					updatePosnIdx(0);
				}
				stableTime = 0;
			} else if(step != 0) {
				// still doing the sync up
				sync (Rbts [lastIdx], Rbts [1-lastIdx], m3piCtrls[lastIdx],ref step);
				// update previous location
				updatePosnIdx(lastIdx);
			}
			if (step == 0)
				++stableTime;
		}
	}

	void updatePosnIdx(int idx){
		// update previous location
		lastPos [0] = Rbts [0].transform.position;
		lastPos [1] = Rbts [1].transform.position;
		lastIdx = idx;
	}




//	void OnDestroy ()
//	{
//		StreamSingleton.getInst ().minusThread ();
//		StreamSingleton.getInst ().minusThread (true);
//	}
}
