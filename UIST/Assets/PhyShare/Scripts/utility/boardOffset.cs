﻿ // hehe: add offset to put the table in the correct place based on where the tracker is

using UnityEngine;
using System.Collections;

public class boardOffset : MonoBehaviour {

	public Vector3 offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.parent.transform.position + offset;
	}
}