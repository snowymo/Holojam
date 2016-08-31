using UnityEngine;
using System.Collections;

public class CokeSyncCtrl : MonoBehaviour {

	//public GameObject robotA;
	public GameObject robotB;

	public GameObject copyRobotA;
	//public GameObject copyRobotB;

	string m_syncMsg = "";

	public string m_stopCmd;

	public GameObject[] beers;
	public GameObject[] cokes;

	// Use this for initialization
	void Start () {
		// disable RBT B
		copyRobotA.SetActive(false);
		//copyRobotB.SetActive(false);
		m_syncMsg = this.GetComponent<SyncMsg> ().text;
		foreach (GameObject can in cokes)
			can.SetActive (false);
	}

	
	// Update is called once per frame
	void Update () {
		m_syncMsg = this.GetComponent<SyncMsg> ().text;
		if (m_syncMsg == null)
			return;
		if (m_syncMsg.Equals (m_stopCmd)) {
			copyRobotA.SetActive (true);
			robotB.SetActive (false);
			changeModel ();
		} 
//		else if (m_syncMsg.Equals ("stopA")) {
//			copyRobotB.SetActive (true);
//			robotA.SetActive (false);
//		} 
		else if (m_syncMsg.Equals ("sync")) {
			robotB.SetActive (true);
			//robotA.SetActive (true);
			copyRobotA.SetActive(false);
			//copyRobotB.SetActive(false);
		}
	}

	void changeModel(){
		foreach (GameObject cup in beers)
			cup.SetActive (false);
		foreach (GameObject can in cokes)
			can.SetActive (true);
	}
}
