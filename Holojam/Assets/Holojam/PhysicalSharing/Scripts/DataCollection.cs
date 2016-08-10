﻿using UnityEngine;
using System.Collections;

public class DataCollection : MonoBehaviour {

	m3piComm m_inst;

	Quaternion rot;

	Vector3 pos;

	public int roundTest;

	public float speed;

	public float waitTime;

	// Use this for initialization
	void Start () {
		m_inst = m3piComm.getInst ();
		m_inst.open ();
		rot = this.transform.rotation;
		pos = this.transform.position;
		//Debug.Log (pos);
	}

	// forward 0.05 0.025 0.02
	// left 15 6 3
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			//m_inst.forwardTest (roundTest);
			m_inst.setSpeed(speed);
			m_inst.setWaitTime (waitTime);
			m_inst.forward();
			Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);

			Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
			Debug.Log("rot:" + Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			rot = this.transform.rotation;
			pos = this.transform.position;
		}
		if (Input.GetKeyDown (KeyCode.B)) {
		//	m_inst.backwardTest (roundTest);
		//	Debug.Log (roundTest);
			m_inst.setSpeed(speed);
			m_inst.setWaitTime (waitTime);
			m_inst.backward();
			Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);

			Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
			Debug.Log("rot:" + Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			rot = this.transform.rotation;
			pos = this.transform.position;
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			m_inst.setSpeed(speed);
			m_inst.setWaitTime (waitTime);
			m_inst.left ();
			Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
			Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			pos = this.transform.position;
			rot = this.transform.rotation;
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			m_inst.setSpeed(speed);
			m_inst.setWaitTime (waitTime);
			m_inst.right ();
			Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
			Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			pos = this.transform.position;
			rot = this.transform.rotation;
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			m_inst.stop ();
			//Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
			//Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			//pos = this.transform.position;
			//rot = this.transform.rotation;
		}
		if(Input.GetKeyDown(KeyCode.P)){
			Debug.Log ("speed:\t" + speed + "\twait:\t" + waitTime);
			Debug.Log("pos:\t" + this.transform.position.ToString("F8"));
			Debug.Log("rot:\t" + this.transform.rotation.ToString("F8"));
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
	}
}
