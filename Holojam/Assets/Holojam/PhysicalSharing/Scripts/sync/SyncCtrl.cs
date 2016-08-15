using UnityEngine;
using System.Collections;

public class SyncCtrl : MonoBehaviour {

	public GameObject robotB;

	public GameObject A2;

	string m_syncMsg = "";

	// Use this for initialization
	void Start () {
		// disable RBT B
		robotB.SetActive(false);
		m_syncMsg = this.GetComponent<SyncMsg> ().text;
	}
	
	// Update is called once per frame
	void Update () {
		m_syncMsg = this.GetComponent<SyncMsg> ().text;
		if (m_syncMsg == null)
			return;
		if (m_syncMsg.Equals ("stop")) {
			robotB.SetActive (true);
			A2.SetActive (false);
		} else if (m_syncMsg.Equals ("sync")) {
			A2.SetActive (true);
			robotB.SetActive (false);
		}
	}
}
