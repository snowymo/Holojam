﻿using UnityEngine;
using System.Collections;
using System.Threading;

public class DataCollection : MonoBehaviour
{

	m3piComm m_inst;

	Quaternion rot;

	Vector3 pos;

	public int roundTest;

	public int speed;

	public int waitTime;

	public string robotName;

	// Use this for initialization
	void Start ()
	{
		m_inst = m3piComm.getInst ();
		m_inst.open ();
		m_inst.setName (robotName);
		rot = this.transform.rotation;
		pos = this.transform.position;
		//Debug.Log (pos);
	}

	bool checkRtnMsg ()
	{
		// check if there is return msg already
		float executeTime = Time.time - m_inst.m_runTime;
		if (!m_inst.m_bRtn) {
			// check if it is already too long then return and sync up them again
			if (executeTime < (m_inst.m_cmdTime + 0.5f))
				return false;
			else {
				print ("wait too long:\t" + executeTime);
				m_inst.m_exStop = true;
				return true;
			}
		}

		if (m_inst.m_returnMsg.Length > 0)
			print (m_inst.m_returnMsg);
		m_inst.m_returnMsg = "";
		if (StreamSingleton.getInst ().getReceiveThread () != null)
			StreamSingleton.getInst ().getReceiveThread ().Abort ();
		return true;
	}

	// forward 0.05 0.025 0.02
	// left 15 6 3
	// Update is called once per frame
	void Update ()
	{
		
		if (Input.GetKeyDown (KeyCode.F)) {
			//m_inst.forwardTest (roundTest);
			if (Utility.getInst ().checkRtnMsg2 (m_inst)) {
				m_inst.clear ();
				m_inst.setSpeed (speed);
				m_inst.setWaitTime (waitTime);
				m_inst.forward ();
				m_inst.run2 ();
				Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);

				Debug.Log (Vector3.Distance (pos, this.transform.position).ToString ("F8"));
				Debug.Log ("rot:" + Quaternion.Angle (rot, this.transform.rotation).ToString ("F8"));
				rot = this.transform.rotation;
				pos = this.transform.position;
			} else {
				print ("busy");
			}
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			//	m_inst.backwardTest (roundTest);
			//	Debug.Log (roundTest);
			if (Utility.getInst ().checkRtnMsg2 (m_inst)) {
				m_inst.clear ();
				m_inst.setSpeed (speed);
				m_inst.setWaitTime (waitTime);
				m_inst.backward ();
				m_inst.run2 ();
				Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);

				Debug.Log (Vector3.Distance (pos, this.transform.position).ToString ("F8"));
				Debug.Log ("rot:" + Quaternion.Angle (rot, this.transform.rotation).ToString ("F8"));
				rot = this.transform.rotation;
				pos = this.transform.position;
			}
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			if (Utility.getInst ().checkRtnMsg2 (m_inst)) {
				m_inst.clear ();
				m_inst.setSpeed (speed);
				m_inst.setWaitTime (waitTime);
				m_inst.left ();
				m_inst.run2 (Time.time);
//				// test threads
//				Thread receiveThread = new Thread (m_inst.receive);
//				receiveThread.Start ();

				Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
				Debug.Log (Quaternion.Angle (rot, this.transform.rotation).ToString ("F8"));
				pos = this.transform.position;
				rot = this.transform.rotation;
			} else {
				print ("busy");
			}
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			if (Utility.getInst ().checkRtnMsg2 (m_inst)) {
				m_inst.clear ();
				m_inst.setSpeed (speed);
				m_inst.setWaitTime (waitTime);
				m_inst.right ();
				m_inst.run2 ();
				Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
				Debug.Log (Quaternion.Angle (rot, this.transform.rotation).ToString ("F8"));
				pos = this.transform.position;
				rot = this.transform.rotation;
			}
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			m_inst.stop ();
			m_inst.run2 ();
			//Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
			//Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			//pos = this.transform.position;
			//rot = this.transform.rotation;
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
			Debug.Log ("pos:\t" + this.transform.position.ToString ("F8"));
			Debug.Log ("rot:\t" + this.transform.rotation.ToString ("F8"));
		}
//		if (Input.GetKeyDown (KeyCode.Z)) {
//			m_inst.left ();
//			Debug.Log (roundTest);
//			//Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
//			Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
//			pos = this.transform.position;
//			rot = this.transform.rotation;
//		}
//		if (Input.GetKeyDown (KeyCode.Y)) {
//			m_inst.right ();
//			Debug.Log (roundTest);
//			//Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
//			Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
//			pos = this.transform.position;
//			rot = this.transform.rotation;
//		}
//		if (Input.GetKeyDown (KeyCode.H)) {
//			m_inst.high ();
//			Debug.Log ("high");
//		}
//		if (Input.GetKeyDown (KeyCode.M)) {
//			m_inst.median ();
//			Debug.Log ("median");
//		}
//		if (Input.GetKeyDown (KeyCode.L)) {
//			m_inst.low ();
//			Debug.Log ("low");
//		}

		// test receive data
		if (m_inst.m_bRtn) {
			if (m_inst.m_returnMsg.Length > 0)
				print (m_inst.m_returnMsg);
			m_inst.m_returnMsg = "";

		}
	}
}
