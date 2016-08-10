using UnityEngine;
using System.Collections;

public class MaskCtrlA : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// apply offset
		transform.position += Offset.getInst().getOffset();
	}
}
