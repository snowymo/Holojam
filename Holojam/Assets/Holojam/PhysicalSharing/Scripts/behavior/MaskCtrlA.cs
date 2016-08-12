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
		transform.position = trackedObj.transform.position + Offset.getInst().getOffset() + new Vector3(0,0.4f,0);
		transform.rotation = trackedRotation.transform.rotation;
	}
}
