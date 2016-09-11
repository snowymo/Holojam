using UnityEngine;
using System.Collections;

public class handOffsetCtrl : MonoBehaviour {

	Transform parent;
	Transform grandParent;

	public GameObject currentTable;
	public GameObject referenceTable;

	// Use this for initialization
	void Start () {
		parent = transform.parent;
		grandParent = parent.parent;
	}
	
	// Update is called once per frame
	void Update () {
		parent.position = grandParent.position - currentTable.transform.position + referenceTable.transform.position;
		parent.rotation = grandParent.rotation * referenceTable.transform.rotation * Quaternion.Inverse(currentTable.transform.rotation);
	}
}
