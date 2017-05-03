using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TangoSwitch : MonoBehaviour {

  public bool isPhone;

  public GameObject TangoDemo;

  public GameObject viveModule;
  public GameObject cameraRig;
  public GameObject env;
  public switchTex switchtex1;
  public switchTex switchtex2;
  public wallSync wallsync;
  public syncRoomba syncrmb;
  public envSwitchCtrl animation;

	// Use this for initialization
  void Awake(){
    if (!isPhone) {
      TangoDemo.SetActive(false);

      viveModule.SetActive(true);
      cameraRig.SetActive(true);
      env.SetActive(true);
      syncrmb.gameObject.SetActive(false);
    } else {
      TangoDemo.SetActive(true);

      viveModule.SetActive(false);
      cameraRig.SetActive(false);
      env.SetActive(false);
      syncrmb.gameObject.SetActive(true);
    }

    switchtex1.isPhone = isPhone;
    switchtex2.isPhone = isPhone;
    wallsync.isPhone = isPhone;
  }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
