using UnityEngine;
using System.Collections;

public class DataCollection : MonoBehaviour {

	StepperCommunication m_inst;

	Quaternion rot;

	Vector3 pos;

	public int round;

	// Use this for initialization
	void Start () {
		m_inst = StepperCommunication.getInstance ();
		m_inst.open ();
		rot = this.transform.rotation;
		pos = this.transform.position;
		//Debug.Log (pos);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			m_inst.forward (round);
			Debug.Log (round);
			Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
			pos = this.transform.position;
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			m_inst.left (round);
			Debug.Log (round);
			//Debug.Log (Vector3.Distance(pos,this.transform.position).ToString("F8"));
			Debug.Log(Quaternion.Angle(rot,this.transform.rotation).ToString("F8"));
			pos = this.transform.position;
			rot = this.transform.rotation;
		}
	}
}
