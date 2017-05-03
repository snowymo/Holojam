using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchTex : MonoBehaviour {

  public bool isPhone;

  public Material fence;
  public Material wall;

	// Use this for initialization
	void Start () {
    if (isPhone) {
      GetComponent<Renderer>().material = fence;
    } else {
      GetComponent<Renderer>().material = wall;
    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
