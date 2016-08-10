using UnityEngine;
using System.Collections;

public class hisCupCtrl : MonoBehaviour {

	Vector3 offsetPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// update the position with offset all the time
		offsetPos = Offset.getInst().getOffset();
		this.transform.position += offsetPos;
	}
}
