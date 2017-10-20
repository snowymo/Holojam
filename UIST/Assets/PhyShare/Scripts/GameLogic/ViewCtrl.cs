using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCtrl : MonoBehaviour {

    public enum VIEWTYPE { viewer, master};
    public enum VIEWROOM { rooma, roomb };

    public VIEWTYPE viewType;
    public VIEWROOM viewRoom;

    public GameObject[] tobeDisabledRoomA;
    public BoardCtrl tobeDisableCtrlA;
    public GameObject[] tobeDisabledRoomB;
    public BoardCtrl tobeDisableCtrlB;

    public Camera[] cameras; // 0 for vive and 1 for gvr

    // Use this for initialization
    void Awake () {
        if(viewType == VIEWTYPE.viewer &&  viewRoom == VIEWROOM.roomb)
        {
            foreach (GameObject go in tobeDisabledRoomA)
            {
                go.SetActive(false);
            }
            if(tobeDisableCtrlA != null)
                tobeDisableCtrlA.enabled = false;
        }
        if (viewType == VIEWTYPE.viewer && viewRoom == VIEWROOM.rooma)
        {
            foreach (GameObject go in tobeDisabledRoomB)
            {
                go.SetActive(false);
            }
            if (tobeDisableCtrlB != null)
                tobeDisableCtrlB.enabled = false;
        }
        if(viewType == VIEWTYPE.master)
            cameras[1].enabled = false;
        else
            cameras[0].enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
