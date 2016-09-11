using UnityEngine;
using System.Collections;

public class actorOffsetCtrl : MonoBehaviour {

	Transform parent;

	public GameObject currentTable;
	public GameObject referenceTable;

	// Use this for initialization
	void Start () {
		parent = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation * 
			(parent.position - currentTable.transform.position) + referenceTable.transform.position;
//		transform.rotation = parent.rotation * referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);
		transform.rotation =  Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation * parent.rotation;
//		transform.localPosition = - currentTable.transform.position + referenceTable.transform.position;
//		transform.localRotation = referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);

	}
}
