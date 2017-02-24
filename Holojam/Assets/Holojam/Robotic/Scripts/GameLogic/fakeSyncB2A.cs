using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class fakeSyncB2A : Synchronizable {

	public GameObject fakeObj;
	public GameObject realObj;

	public Vector3 syncvec;
	public string syncst;

	void Reset ()
	{
		label = "fakeBtoA";		//robot A is moving
		useMasterClient = false;
	}
	//TODO: check with Aaron
	protected override void Sync ()
	{
		if (sending) {
			view.triples[0] = transform.position;
			if (fakeObj.GetComponent<MeshRenderer> ().enabled)
				view.text = "t";
			else
				view.text = "f";
			syncvec = view.triples[0];
			syncst = view.text;
		} else {
			if(view.triples[0].magnitude > 0.01f)
				transform.position = Vector3.Lerp(transform.position, view.triples[0],Time.deltaTime * 10f);
			
			if (view.text == "t") {
				fakeObj.GetComponent<MeshRenderer> ().enabled = true;
				realObj.GetComponent<MeshRenderer> ().enabled = false;
			} else if (view.text == "f") {
				fakeObj.GetComponent<MeshRenderer> ().enabled = false;
				realObj.GetComponent<MeshRenderer> ().enabled = true;
			}
		}
	}
}
