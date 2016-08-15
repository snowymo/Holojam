using UnityEngine;
using System.Collections;

public class hisCupCtrlB : MonoBehaviour {

	public GameObject trackedObject;

	public Vector3 remoteYOffset;

	// Use this for initialization
	void Start () {
		remoteYOffset = new Vector3 (0, 0.145f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = trackedObject.transform.position - Offset.getInst ().getOffset () - remoteYOffset;
	}
}
