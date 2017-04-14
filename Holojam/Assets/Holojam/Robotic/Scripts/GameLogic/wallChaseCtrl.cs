using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallChaseCtrl : MonoBehaviour {

	public Transform trackedWall;

	public wallSync syncWallInfo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (syncWallInfo.isReady){
			transform.position = new Vector3 (trackedWall.position.x, transform.position.y, transform.position.z);
			transform.position = trackedWall.position;

		}
		else {
			transform.position = trackedWall.position;
		}
	}
}
