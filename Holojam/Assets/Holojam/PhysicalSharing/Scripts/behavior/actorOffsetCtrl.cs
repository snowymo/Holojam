using UnityEngine;
using System.Collections;

public class actorOffsetCtrl : MonoBehaviour
{

	//Transform parent;

	public GameObject currentTable;
	public GameObject referenceTable;

	 Vector3 localPos;

	public GameObject originalObj;
	public GameObject originalHead;

	// Use this for initialization
	void Start ()
	{
		//parent = transform.parent;
		transform.position = referenceTable.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		//Vector3 nextPos = Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation * 
		//	(parent.position - currentTable.transform.position) + referenceTable.transform.position;
		localPos = (originalObj.transform.position - currentTable.transform.position);
//		Vector3 nextPos = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation * localPos
//		                  + referenceTable.transform.position;
		transform.position = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation * localPos
			+ referenceTable.transform.position;
//		transform.rotation = parent.rotation * referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);
		transform.rotation = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation * 
			originalHead.transform.rotation;
//		transform.localPosition = - currentTable.transform.position + referenceTable.transform.position;
//		transform.localRotation = referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);

	}
}
