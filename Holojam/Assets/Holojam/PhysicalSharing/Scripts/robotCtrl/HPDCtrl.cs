using UnityEngine;
using System.Collections;

public class HPDCtrl : HPSCtrl {

	public GameObject[] styluses;

	int[] steps;

	Vector3[] _destinations;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setDestination (Vector3 dest, int index)
	{
		print ("rbt started to move");
		_destinations[index] = dest;
		steps[index] = 1;
	}
}
