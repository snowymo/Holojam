using UnityEngine;
using System.Collections;

public class tableCtrl : MonoBehaviour {

	public GameObject trackedObj;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		this.transform.position = trackedObj.transform.position + new Vector3(0,0,0.5f);
		this.transform.rotation = trackedObj.transform.rotation;// * Quaternion.Euler(Vector3.up * (30));
		//trackedObj.transform.rotation = trackedObj.transform.rotation * Quaternion.Euler(new Vector3(0,0,180));
	}
}
