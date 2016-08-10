using UnityEngine;
using System.Collections;

public class MaskCtrlA : MonoBehaviour {

	public GameObject trackedObj,trackedRotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// apply offset
		transform.position = trackedObj.transform.position + Offset.getInst().getOffset();
		transform.rotation = trackedRotation.transform.rotation;
	}
}
