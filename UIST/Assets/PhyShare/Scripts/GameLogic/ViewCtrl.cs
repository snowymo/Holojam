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

    // Use this for initialization
    void Start () {
        if(viewType == VIEWTYPE.viewer &&  viewRoom == VIEWROOM.roomb)
        {
            foreach (GameObject go in tobeDisabledRoomA)
            {
                go.SetActive(false);
            }
            tobeDisableCtrlA.enabled = false;
        }
        if (viewType == VIEWTYPE.viewer && viewRoom == VIEWROOM.rooma)
        {
            foreach (GameObject go in tobeDisabledRoomB)
            {
                go.SetActive(false);
            }
            tobeDisableCtrlB.enabled = false;
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
