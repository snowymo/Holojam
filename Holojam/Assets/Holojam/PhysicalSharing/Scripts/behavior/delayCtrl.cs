using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class delayCtrl : MonoBehaviour
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

	// Use this for initialization
	void Start ()
	{
		movements = new Queue<Vector3> ();

		fakeMr = fakeModel.GetComponent<MeshRenderer> ();
		realMr = realModel.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.rotation = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation * opponentCtrl.transform.rotation;
		if(transform.position.magnitude < 0.01f)
			transform.position = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation *
				(opponentCtrl.transform.position - currentTable.transform.position) + referenceTable.transform.position;
		if (referenceTable.GetComponent<boardCtrl> ().isViewer)
			return;
		if (_boardRbtCtrl.GetComponent<boardRbtCtrl> ().step != 0) {
			if (movements.Count == 0)
				startTime = Time.time;
			// if lastIdx is 1 means new robot is moving, so we need to record the old controller's movement
//			print(_boardRbtCtrl.GetComponent<boardRbtCtrl>().lastIdx);
			if (_boardRbtCtrl.GetComponent<boardRbtCtrl> ().lastIdx == fakeIdx) {
				movements.Enqueue (opponentCtrl.transform.position);
				// 
				fakeMr.enabled = true;
				realMr.enabled = false;
				transform.position = Quaternion.Inverse (currentTable.transform.rotation) * referenceTable.transform.rotation *
				(movements.Peek () - currentTable.transform.position) + referenceTable.transform.position;
				transform.position = currentCtrl.transform.position;
			}
		} else {
			// two robots are in the same place
			if (movements.Count == 0) {
				startTime = Time.time;
				// don't move
				fakeMr.enabled = false;
				realMr.enabled = true;
				// send (0,0,0,fakeIdx) to them to tell fakeidx is not showing
			} else {
				//robots arrived first
				transform.position = Vector3.Lerp (transform.position, currentCtrl.transform.position, Time.deltaTime * 10f);
				startTime = Time.time;
				if (Vector3.Distance (transform.position, currentCtrl.transform.position) < Utility.getInst ().disError) {
					fakeMr.enabled = false;
					realMr.enabled = true;
					while (movements.Count > 0)
						movements.Dequeue ();
				}
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
				// send position+fakeidx to them and set fake true
			} 
		} 

	}
}
