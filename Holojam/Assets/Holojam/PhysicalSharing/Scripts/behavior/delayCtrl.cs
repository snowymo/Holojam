using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class delayCtrl : MonoBehaviour {

	public GameObject _boardRbtCtrl;

	Queue<Vector3> movements;

	public GameObject opponentCtrl;

	float startTime;

	Transform parent;

	public GameObject currentTable;
	public GameObject referenceTable;

	public GameObject fakeModel;
	public GameObject realModel;
	MeshRenderer fakeMr,realMr;

	public GameObject parentObject;

	public int fakeIdx;

	// Use this for initialization
	void Start () {
		movements = new Queue<Vector3> ();
		parent = transform.parent;
		fakeMr = fakeModel.GetComponent<MeshRenderer> ();
		realMr = realModel.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_boardRbtCtrl.GetComponent<boardRbtCtrl>().step != 0) {
			if (movements.Count == 0)
				startTime = Time.time;
			// if lastIdx is 1 means new robot is moving, so we need to record the old controller's movement
//			print(_boardRbtCtrl.GetComponent<boardRbtCtrl>().lastIdx);
			if (_boardRbtCtrl.GetComponent<boardRbtCtrl> ().lastIdx == fakeIdx) {
				movements.Enqueue (opponentCtrl.transform.position);
			//	print ("enqueue:\t" + opponentCtrl.transform.position);
			}
		}
		if (Time.time - startTime > 2) {
			if (movements.Count > 0) {
				// dequeue the position to assign to fakecontroller
				//transform.position = movements.Dequeue();
				fakeMr.enabled = true;
				realMr.enabled = false;
				transform.position = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation *
				(movements.Dequeue () - currentTable.transform.position) + referenceTable.transform.position;
			} 
		} else {
			// don't move
			fakeMr.enabled = false;
			realMr.enabled = true;
		}



		transform.rotation =  Quaternion.Inverse(currentTable.transform.rotation) * referenceTable.transform.rotation * parentObject.transform.rotation;
	}
}
