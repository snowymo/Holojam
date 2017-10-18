﻿// hehe: sync up two robots at the beginning
// hehe: take turns to sync up the robots from then on
// hehe: whoseturn means currently which one should be moving

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
	public int lastIdx;
	Vector3[] lastPos;
	Vector3[] defaultRBT;

	int stableTimeCount = 10;
	public int stableTime;

	public int myTS;

    public int whoseturn;

	//public bool isViewer;

    // create m3pi robot controllers
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

        whoseturn = 2;// index of whose turn, 2 means any one

    }

	// wait until two robots are tracked for more than 2 frames
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
        Time.fixedDeltaTime = 0.5f;

        createM3pi ();

		initialAttr ();
	}


	
	// Update is called once per frame
	void FixedUpdate  ()
	{
		myTS = Utility.getInst ().getMyTS ();
		// check if tracked
		for (int index = 0; index < 2; index++) {
			if (Rbts [index].GetComponent<Holojam.Network.Controller> ().Tracked)
				myStart (index, Rbts [index]);
		}
		// sync up two robots, based on which one move larger in previous frame
		if (isFirst [0] == 3 && isFirst [1] == 3) {
			// if previous state is sync up
			if (step == 0 && (stableTime >= stableTimeCount)) {
				float moveOld = Vector3.Distance (lastPos [0], Rbts [0].transform.position);
				float moveNew = Vector3.Distance (lastPos [1], Rbts [1].transform.position);
//				print ("old diff:\t" + moveOld + "\tnew diff:\t" + moveNew);
                if(whoseturn == 2)
                {
                    if (moveOld > (moveNew + Utility.getInst().disError))
                    {
                        //	Debug.Log ("move \"New\"" + Rbts [1].transform.localPosition + "\t" + Rbts [0].transform.localPosition);
                        step = 1;
                        //					sync (Rbts [1], Rbts [0], m3piCtrls[1],ref step,1);
                        //sync (Rbts [1], Rbts [0],1,true);
                        //
                        Vector3 vec = Quaternion.Inverse(Rbts[0].transform.parent.transform.rotation) * Rbts[1].transform.parent.transform.rotation *
                            (Rbts[0].transform.position - Rbts[0].transform.parent.transform.position) + Rbts[1].transform.parent.transform.position;
                        sync(Rbts[1], vec, 1);
                        //if(step == 0)
                           // whoseturn = 1;
                    }
                    else if (moveNew > (moveOld + Utility.getInst().disError))
                    {
                        //	Debug.Log ("move \"Old\"");
                        step = 1;
                        // update previous location
                        //sync (Rbts [0], Rbts [1], 0,true);
                        Vector3 vec = Quaternion.Inverse(Rbts[1].transform.parent.transform.rotation) * Rbts[0].transform.parent.transform.rotation *
                            (Rbts[1].transform.position - Rbts[1].transform.parent.transform.position) + Rbts[0].transform.parent.transform.position;
                        sync(Rbts[0], vec, 0);
                        //if(step == 0)
                            //whoseturn = 0;
                    }
                }
                else
                {
                    step = 1;
                    //					sync (Rbts [1], Rbts [0], m3piCtrls[1],ref step,1);
                    //sync (Rbts [1], Rbts [0],1,true);
                    //
                    Vector3 vec = Quaternion.Inverse(Rbts[whoseturn].transform.parent.transform.rotation) * Rbts[1- whoseturn].transform.parent.transform.rotation *
                        (Rbts[whoseturn].transform.position - Rbts[whoseturn].transform.parent.transform.position) + Rbts[1- whoseturn].transform.parent.transform.position;
                    sync(Rbts[1 - whoseturn], vec, 1 - whoseturn);
                    //if (step == 0)
                        //whoseturn = 1-whoseturn;
                }
				stableTime = 0;
			} else if(step != 0) {
				// still doing the sync up, including the first state
				//sync (Rbts [lastIdx], Rbts [1-lastIdx], lastIdx);
				Vector3 vec = Quaternion.Inverse(Rbts [1-lastIdx].transform.parent.transform.rotation) * Rbts [lastIdx].transform.parent.transform.rotation * 
					(Rbts [1-lastIdx].transform.position - Rbts [1-lastIdx].transform.parent.transform.position) + Rbts [lastIdx].transform.parent.transform.position;
				sync (Rbts [lastIdx], vec,lastIdx);
			}
			if (step == 0)
				++stableTime;
		}
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
        //if (!Utility.getInst ().checkRtnMsg2 (m3piCtrls [index]))
//			return;
		
		//		print ("local\t" + local.transform.position + "\t" + local.transform.localPosition);
		//		print ("remote\t" + remote.transform.position + "\t" + remote.transform.localPosition);
		Utility.getInst ().drawRays (local.transform, remote, isLocal);
		
		Vector3 localPos = new Vector3 (), remotePos = new Vector3 ();
		ignoreYPos (local, remote, ref localPos, ref remotePos);
		
		m3piCtrls [index].clear ();

		// update information
		lastPos [0] = Rbts [0].transform.position;
		lastPos [1] = Rbts [1].transform.position;
		lastIdx = index;
		
		// send command
		if (step != 0) {
			switch (step) {
			case 1:
                    // check distance first
                print("move index:\t" + index + "\tstep:\t" + step);
                if (Utility.getInst ().checkMatchV2 (localPos, remotePos)) {
                        print("checkMatchV2 index:\t" + index + "\tstep:\t" + step);
                        step = 0;
				} else {
                    print("turnAround index:\t" + index + "\tstep:\t" + step);
                    if (turnAround(local, remote, m3piCtrls[index], isLocal)) {
						print ("goStraight index:\t" + index + "\tstep:\t" + step);
						goStraight (local, remote, m3piCtrls [index], isLocal);
						step = 2;
					}
				}
				break;
			case 2:
                    print("move index:\t" + index + "\tstep:\t" + step);
                    // moved car with going straight
                    print("goStraight index:\t" + index + "\tstep:\t" + step);
                if (goStraight (local, remote, m3piCtrls [index], isLocal)) {
					step = 0;
					
				} else {
					step = 1;
				}
				break;
			default:
				break;
			}
		}
	}


//	void OnDestroy ()
//	{
//		StreamSingleton.getInst ().minusThread ();
//		StreamSingleton.getInst ().minusThread (true);
//	}
}
