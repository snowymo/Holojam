//hehe: control fake animation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DelayCtrl : MonoBehaviour
{

    public GameObject _boardRbtCtrl;

    public Queue<Vector3> movements;

    public GameObject opponentCtrl;

    float startTime;

    //Transform parent;

    public GameObject currentTable;
    public GameObject referenceTable;

    public GameObject fakeModel;
    public GameObject realModel;
    MeshRenderer fakeMr, realMr;

    public GameObject currentCtrl;

    public int fakeIdx;

    public ViewCtrl viewCtrl;

    // Use this for initialization
    void Start()
    {
        movements = new Queue<Vector3>();

        fakeMr = fakeModel.GetComponent<MeshRenderer>();
        realMr = realModel.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation * opponentCtrl.transform.rotation;
        if (transform.position.magnitude < 0.01f && opponentCtrl.GetComponentInParent<TrackableComponent>().Tracked)
            transform.position = Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation *
            (opponentCtrl.transform.position - currentTable.transform.position) + referenceTable.transform.position;
        //if (referenceTable.GetComponent<BoardCtrl>().isViewer)
        if ((referenceTable.name.Substring(referenceTable.name.Length - 1)[0] - 'A') != (int)(viewCtrl.viewRoom))
            return;
        if (_boardRbtCtrl.GetComponent<boardRbtCtrl>().step != 0)
        {
            if (movements.Count == 0)
            {
                startTime = Time.time;
                //
                transform.position = currentCtrl.transform.position;
            }
            // if lastIdx is 1 means new robot is moving, so we need to record the old controller's movement
            //			print(_boardRbtCtrl.GetComponent<boardRbtCtrl>().lastIdx);
            if ((_boardRbtCtrl.GetComponent<boardRbtCtrl>().readystart) && (_boardRbtCtrl.GetComponent<boardRbtCtrl>().lastIdx == fakeIdx))
            {
                movements.Enqueue(opponentCtrl.transform.position);
                // 
                fakeMr.enabled = true;
                realMr.enabled = false;
                //				transform.position = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation *
                //				(movements.Peek () - currentTable.transform.position) + referenceTable.transform.position;
                //				transform.position = currentCtrl.transform.position;
            }
        }
        else
        {
            if (movements.Count > 0)
            {
                //				//robots arrived first
                robotArriveFirst();
            }

            if (movements.Count == 0)
            {
                startTime = Time.time;
                // lerp to current robot's place
                if (Vector3.Distance(transform.position, currentCtrl.transform.position) > Utility.getInst().disError)
                {
                    transform.position = Vector3.Lerp(transform.position, currentCtrl.transform.position, Time.deltaTime * 8f);
                }
                else
                {
                    // two robots are in the same place
                    fakeMr.enabled = false;
                    realMr.enabled = true;
                }
                // send (0,0,0,fakeIdx) to them to tell fakeidx is not showing
            }
        }
        if (Time.time - startTime > 2)
        {
            if (movements.Count > 0)
            {
                // dequeue the position to assign to fakecontroller
                //transform.position = movements.Dequeue();
                fakeMr.enabled = true;
                realMr.enabled = false;
                transform.position = Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation *
                (movements.Dequeue() - currentTable.transform.position) + referenceTable.transform.position;
                // send position+fakeidx to them and set fake true
            }
        }

    }

    void robotArriveFirst()
    {
        Vector3[] restMovement;
        restMovement = new Vector3[movements.Count];
        movements.CopyTo(restMovement, 0);
        int finalsize = 0;
        for (int i = movements.Count - 1; i > 0; i--)
        {
            Vector3 lastVec = restMovement[i];
            Vector3 lastVec2 = restMovement[i - 1];
            float dis = Vector3.Distance(lastVec, lastVec2);
            if (dis < 0.01f)
            {
                ++finalsize;
            }
            else
                break;
        }
        while (finalsize > 0)
        {
            movements.Dequeue();
            --finalsize;
        }
    }
}
