using UnityEngine;
using System.Collections;

public class cityBlockCtrl : MonoBehaviour {

	int linkedIdx;	// -1 means no link
	public GameObject[] linkedObjects;

	// Use this for initialization
	void Start () {
		linkedIdx = -1;
	}

	public void setLinkIdx(int idx){
		linkedIdx = idx;
	}
	
	// Update is called once per frame
	void Update () {
		if (linkedIdx > -1) {
			print ("move under rbt");
			transform.position = linkedObjects [linkedIdx].transform.position;
			transform.rotation = linkedObjects [linkedIdx].transform.rotation;
		}
	}
}
