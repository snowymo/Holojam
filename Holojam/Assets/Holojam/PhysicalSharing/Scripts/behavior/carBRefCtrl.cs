﻿using UnityEngine;
using System.Collections;

public class carBRefCtrl : MonoBehaviour {

	public GameObject trackedObj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = trackedObj.transform.position + Offset.getInst().getOffset();
		transform.rotation = trackedObj.transform.rotation;
	}
}
