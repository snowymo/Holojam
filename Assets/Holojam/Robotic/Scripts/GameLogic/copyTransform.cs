using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyTransform : MonoBehaviour {

  public Transform reference;

  public bool isPos;

  public bool isIK;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    if(isPos)
      transform.position = reference.position;//new Vector3(reference.position.x,0,reference.position.z);
    //transform.rotation = reference.rotation;
    if (isIK) {
      transform.position = new Vector3(reference.position.x,0,reference.position.z);
    }
	}
}
