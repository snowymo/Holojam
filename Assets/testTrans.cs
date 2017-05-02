using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTrans : MonoBehaviour {


  public GameObject mask;

  Color maskColor;

  // Use this for initialization
  void Start () {
    
    maskColor = new Color (0.5f,0.5f,0.5f,0.5f);
    mask.GetComponent<Renderer>().material.color = maskColor;
   

  }

  // Update is called once per frame
  void Update () {
    if (Input.GetKeyDown (KeyCode.Space)) {
      
      SwitchEnv ();
    }

  }

  void changeVisibility(GameObject g, bool vis){
    Renderer[] renderers = g.GetComponentsInChildren<Renderer>();
    foreach (Renderer r in renderers)
    {
      r.enabled = vis;
    }
  }


  void SwitchEnv(){
    if (maskColor.a < 1f ) {
      // create a mask to cover the camera
      maskColor.a += Time.deltaTime;
      maskColor.r += Time.deltaTime;
      print("mask color " + maskColor);
      mask.GetComponent<Renderer> ().material.color = maskColor;
      print("mat color " + mask.GetComponent<Renderer> ().material.GetColor("_Color"));
      // disable faded and enable display

    }
    else if(maskColor.a >= 0.05f ) {
      maskColor.a -= Time.deltaTime;
      mask.GetComponent<Renderer> ().material.SetColor ("_Color",maskColor);
    }
    // then disappear the camera
  }
}
