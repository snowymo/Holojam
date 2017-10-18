﻿// hehe: receive sync chess msg if it is viewer
// hehe: send sync chess msg if it is master
// hehe: select (show cross or circle) based on sync chess msg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCtrl : MonoBehaviour
{

    public GameObject chessModel;

    public string username;
    public GameObject[] chesses;

    public GameObject chessSyncGameObject;

    //Transform ctrler;
    protected GameObject ctrlGo;

    public ViewCtrl viewCtrl;

    public GameObject enemyBoard;

    public string remoteAssignFlag;

    public boardRbtCtrl boardrbtctrl;
    private int roomNum;

    // Use this for initialization
    void Start()
    {
        // create nine chess at the beginning
        chesses = new GameObject[9];
        for (int i = 0; i < chesses.Length; i++)
        {
            chesses[i] = (GameObject)GameObject.Instantiate(chessModel);
            chesses[i].name = "chess" + i.ToString("00");
            chesses[i].AddComponent<ChessCtrl>();
            chesses[i].transform.parent = transform;
            chesses[i].transform.localPosition = new Vector3(i % 3 - 1, 0, i / 3 - 1) * 0.1f;
            //chesses [i].transform.localScale = new Vector3 (0.08f, 0.08f, 0.08f);
        }

        // assign control gameobject
        Transform[] ts = transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.gameObject.name == "controller")
            {
                //ctrler = t;
                ctrlGo = t.gameObject;
                break;
            }
        }
        if(transform.Find("ChessControllerA") != null) {
            roomNum = 0;
            ctrlGo = transform.Find("ChessControllerA").gameObject;
        }
        else
        {
            roomNum = 1;
            ctrlGo = transform.Find("ChessControllerB").gameObject;
        }            
        remoteAssignFlag = "";
        //isViewer = false;
    }

    // Update is called once per frame
    void Update()
    {
        receiveAssignment();
        assignChess();
    }

    void assignChess()
    {
        // if ctrl hits one chess more than two seconds
        float dis = Vector3.Distance(chesses[0].transform.position, chesses[8].transform.position);
        int indexselect = -1;
        //print (ctrlGo.transform.position);
        // choose one of the highlights to set time
        for (int i = 0; i < chesses.Length; i++)
        {
            if (chesses[i].GetComponent<ChessCtrl>().isSelect())
            {
                float d = Vector3.Distance(chesses[i].transform.position, ctrlGo.transform.position);
                if (d < dis)
                {
                    indexselect = i;
                    dis = d;
                }

            }
        }
        if (indexselect != -1)
        {
            if (remoteAssignFlag != "")
            {
                string[] msg = remoteAssignFlag.Split(splitChar, 2);
                if (indexselect == int.Parse(msg[0]))
                {
                    chesses[int.Parse(msg[0])].GetComponent<ChessCtrl>().select(msg[1]);
                    if(msg[1] == "cross")
                    {
                        boardrbtctrl.whoseturn = 1;
                    }else
                    {
                        boardrbtctrl.whoseturn = 0;
                    }
                    remoteAssignFlag = "";
                }
            }
            else if ((boardrbtctrl.whoseturn == 2 || boardrbtctrl.whoseturn == roomNum) &&  GetComponent<BoardCtrl>().chesses[indexselect].GetComponent<ChessCtrl>().select(username))
            {
                if (username == "cross")
                {
                    boardrbtctrl.whoseturn = 1;
                }
                else
                {
                    boardrbtctrl.whoseturn = 0;
                }
                // select the same in other table
                if (viewCtrl.viewType == ViewCtrl.VIEWTYPE.master)
                {
                    // manually select
                    enemyBoard.GetComponent<BoardCtrl>().remoteAssignFlag = indexselect.ToString() + "-" + username;
                    //later
                    //enemyBoard.GetComponent<boardCtrl> ().chesses [indexselect].GetComponent<chessCtrl> ().select (username);
                    // send msg
                    chessSyncGameObject.GetComponent<chessSync>().sentMsg = indexselect.ToString() + "-" + username;
                }
            }
        }
    }

    protected char[] splitChar = { '-' };
    void receiveAssignment()
    {
        if (viewCtrl.viewType != ViewCtrl.VIEWTYPE.master)
        {
            // use msg 
            chessSync cs = chessSyncGameObject.GetComponent<chessSync>();
            if (cs.rcvMsg != "" && cs.rcvMsg != null)
            {
                remoteAssignFlag = cs.rcvMsg;
                //string[] msg = cs.text.Split (splitChar, 2);
                //chesses [int.Parse (msg [0])].GetComponent<chessCtrl> ().select (msg [1]);
            }
        }
    }

}