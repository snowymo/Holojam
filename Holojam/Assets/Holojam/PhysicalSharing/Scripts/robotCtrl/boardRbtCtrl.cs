﻿using UnityEngine;
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
		// check if tracked
		for (int index = 0; index < 2; index++) {
			if (Rbts [index].GetComponent<Holojam.Network.HolojamView> ().IsTracked)
				myStart (index, Rbts [index]);
		}
		// sync up
		if (isFirst [0] == 3 && isFirst [1] == 3) {
			// if previous state is sync up
			if (step == 0) {
				float moveOld = Vector3.Distance (lastPos [0], Rbts [0].transform.position);
				float moveNew = Vector3.Distance (lastPos [1], Rbts [1].transform.position);
				//print ("old diff:\t" + moveOld + "\tnew diff:\t" + moveNew);
				if (moveOld > (moveNew + Utility.getInst ().disError)) {
					Debug.Log ("move \"New\"" + Rbts [1].transform.localPosition + "\t" + Rbts [0].transform.localPosition);
					step = 1;
					sync (Rbts [1], Rbts [0], 1);
				} else if (moveNew > (moveOld + Utility.getInst ().disError)) {
					Debug.Log ("move \"Old\"");
					step = 1;
					sync (Rbts [0], Rbts [1], 0);
				}
			} else {
				// still doing the sync up
				if (lastIdx == 0) {
					sync (Rbts [0], Rbts [1], 0);
					//Debug.Log ("move \"Old\"");
				} else {
					sync (Rbts [1], Rbts [0], 1);
					//Debug.Log ("move \"New\"");
				}
			}
		}
	}

	void ignoreYPos (GameObject local, GameObject remote, ref Vector3 localPos, ref Vector3 remotePos)
	{
		// get local position
		localPos = local.transform.localPosition;
		remotePos = remote.transform.localPosition;
		// ignore y information
		localPos.y = 0;
		remotePos.y = 0;
	}

	void sync (GameObject local, GameObject remote, int index)
	{
		if (!Utility.getInst ().checkRtnMsg2 (m3piCtrls [index]))
			return;

		Utility.getInst ().drawRays (local.transform, remote.transform, true);

		Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();
		ignoreYPos (local, remote, ref localPos, ref remotePos);

		// send command
		if (step != 0) {
			print ("move index:\t" + index + "\tstep:\t" + step);
			switch (step) {
			case 1:
			// check distance first
				if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
					step = 0;
				} else {
					if (turnAround (local, remote, m3piCtrls [index], true)) {
						goStraight (local, remote, m3piCtrls [index], true);
						step = 2;
					}
				}
				break;
			case 2:
			// moved car with going straight
				if (goStraight (local, remote, m3piCtrls [index], true)) {
					step = 0;
				} else {
					step = 1;
				}
				break;
			default:
				break;
			}
		}
		// update previous location
		lastPos [0] = Rbts [0].transform.position;
		lastPos [1] = Rbts [1].transform.position;
		lastIdx = index;
	}

	void OnDestroy ()
	{
		StreamSingleton.getInst ().minusThread ();
		StreamSingleton.getInst ().minusThread (true);
	}
}
