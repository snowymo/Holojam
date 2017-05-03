using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class envSwitchCtrl : MonoBehaviour {

	public GameObject[] faded;
	public GameObject[] displayed;
	public GameObject mask;

	public bool doit;
	public bool win;

	Color maskColor;

	// Use this for initialization
	void Start () {
		doit = false;
		maskColor = new Color (0,0,0,0.0f);
    mask.GetComponent<Renderer>().material.color = maskColor;
		foreach (GameObject d in displayed) {
			changeVisibility (d,false);
		}
		win = false;
		
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
      doit = true;
			//SwitchEnv ();
		}
		if (doit)
			SwitchEnv ();
	}

	void changeVisibility(GameObject g, bool vis){
		Renderer[] renderers = g.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers)
		{
			r.enabled = vis;
		}
	}
		
		
	void SwitchEnv(){
		if (maskColor.a < 1f && faded[0].GetComponent<Renderer> ().enabled) {
			// create a mask to cover the camera
			maskColor.a += Time.deltaTime/1;
      //maskColor.r += Time.deltaTime/10;
      //print("mask color " + maskColor);
			mask.GetComponent<Renderer> ().material.color = maskColor;
      //print("mat color " + mask.GetComponent<Renderer> ().material.GetColor("_Color"));
			// disable faded and enable display
			if(maskColor.a >= 0.9995f){
				foreach (GameObject d in faded) {
					changeVisibility (d,false);
					//d.GetComponent<Renderer> ().enabled = false;
//					d.SetActive (false);
				}
				foreach (GameObject d in displayed) {
					changeVisibility (d,true);
				//	d.GetComponent<Renderer> ().enabled = true;
				}	
			}
		}
		else if(maskColor.a >= 0.05f && win) {
			maskColor.a -= Time.deltaTime/1;
			mask.GetComponent<Renderer> ().material.SetColor ("_Color",maskColor);
		}
		// then disappear the camera
	}
}
