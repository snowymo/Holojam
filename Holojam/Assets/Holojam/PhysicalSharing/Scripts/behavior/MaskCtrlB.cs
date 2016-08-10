using UnityEngine;
using System.Collections;

public class MaskCtrlB : MonoBehaviour {

	public GameObject trackedObj;

	public GameObject trackedRotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// apply offset
		transform.position = trackedObj.transform.position - Offset.getInst().getOffset();
		transform.rotation = trackedRotation.transform.rotation;
	}
}
