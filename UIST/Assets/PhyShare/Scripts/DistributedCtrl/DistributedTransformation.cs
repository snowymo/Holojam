using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributedTransformation : MonoBehaviour
{

    //Transform parent;

    public Transform currentTable;
    public Transform referenceTable;

    Vector3 localPos;

    public GameObject originalObj;
    public GameObject originalHead;

    public Transform remoteObj;
    public Transform localObj;

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
        localPos = (remoteObj.position - currentTable.position);
        print(gameObject.name + " local pos:" + localPos);
        //		Vector3 nextPos = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation * localPos
        //		                  + referenceTable.transform.position;
        localObj.position = Quaternion.Inverse(currentTable.rotation) * referenceTable.rotation * localPos
            + referenceTable.position;
        //		localObj.rotation = parent.rotation * referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);
        localObj.rotation = Quaternion.Inverse(currentTable.rotation) * referenceTable.rotation * remoteObj.rotation;
        //		transform.localPosition = - currentTable.transform.position + referenceTable.transform.position;
        //		transform.localRotation = referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);

        Matrix4x4 localMatrix = Matrix4x4.TRS(referenceTable.position, referenceTable.rotation, referenceTable.localScale) *
            Matrix4x4.TRS(currentTable.position, currentTable.rotation, currentTable.localScale).inverse 
            * Matrix4x4.TRS(remoteObj.position, remoteObj.rotation, remoteObj.localScale);
        localObj.rotation = localMatrix.GetRotation();
        localObj.position = localMatrix.GetPosition();
        localObj.localScale = localMatrix.GetScale();
    }
}
