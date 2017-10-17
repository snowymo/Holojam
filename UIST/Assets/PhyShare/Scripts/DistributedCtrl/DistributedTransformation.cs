using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributedTransformation : MonoBehaviour
{

    //Transform parent;

    public Transform currentTable;
    public Transform referenceTable;

    Vector3 localPos;

    //public GameObject originalObj;
    //public GameObject originalHead;

    public Transform[] remoteObjs;
    public Transform[] localObjs;

    // Use this for initialization
    void Start()
    {
        //parent = transform.parent;
        transform.position = referenceTable.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        //Vector3 nextPos = Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation * 
        //	(parent.position - currentTable.transform.position) + referenceTable.transform.position;
        for(int i = 0; i < remoteObjs.Length; i++)
        {
            localPos = (remoteObjs[i].position - currentTable.position);
            //print(gameObject.name + " local pos:" + localPos);
            //		Vector3 nextPos = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation * localPos
            //		                  + referenceTable.transform.position;
            localObjs[i].position = Quaternion.Inverse(currentTable.rotation) * referenceTable.rotation * localPos
                + referenceTable.position;
            //		localObj.rotation = parent.rotation * referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);
            localObjs[i].rotation = Quaternion.Inverse(currentTable.rotation) * referenceTable.rotation * remoteObjs[i].rotation;
            //		transform.localPosition = - currentTable.transform.position + referenceTable.transform.position;
            //		transform.localRotation = referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);

            Matrix4x4 localMatrix = Matrix4x4.TRS(referenceTable.position, referenceTable.rotation, referenceTable.localScale) *
                Matrix4x4.TRS(currentTable.position, currentTable.rotation, currentTable.localScale).inverse
                * Matrix4x4.TRS(remoteObjs[i].position, remoteObjs[i].rotation, remoteObjs[i].localScale);
            localObjs[i].rotation = localMatrix.GetRotation();
            localObjs[i].position = localMatrix.GetPosition();
            localObjs[i].localScale = localMatrix.GetScale();
        }
        
    }
}
