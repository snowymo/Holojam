using UnityEngine;
using System.Collections;

public class DataCollection : MonoBehaviour {

	SerialCommunication m_inst;

	Quaternion rot;

	Vector3 pos;

	public int round;

	// Use this for initialization
	void Start () {
		m_inst = SerialCommunication.getInstance ();
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
			m_inst.forward ();
			Debug.Log (round);
			Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
			pos = this.transform.position;
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			m_inst.left ();
			Debug.Log (round);
			//Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
			Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			pos = this.transform.position;
			rot = this.transform.rotation;
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			m_inst.high ();
			Debug.Log ("high");
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			m_inst.median ();
			Debug.Log ("median");
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			m_inst.low ();
			Debug.Log ("low");
		}
	}
}
