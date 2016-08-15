using UnityEngine;
using System.Collections;

public class hisCupCtrl : MonoBehaviour {

	public Vector3 offset;

	public GameObject trackedObj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// update the position with offset all the time
		this.transform.position = Offset.getInst().getOffset() + trackedObj.transform.position + offset;
		this.transform.rotation = trackedObj.transform.rotation;
	}
}
