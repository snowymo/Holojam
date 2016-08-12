using UnityEngine;
using System.Collections;

public class hisCupCtrlB : MonoBehaviour {

	public GameObject trackedObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = trackedObject.transform.position - Offset.getInst ().getOffset () - new Vector3(0,0.14f,0);
	}
}
