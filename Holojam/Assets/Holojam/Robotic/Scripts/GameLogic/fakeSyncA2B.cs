using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class fakeSyncA2B : Synchronizable
{

	public GameObject fakeObj;
	public GameObject realObj;

	public Vector3 syncvec;
	public string syncst;

	void Reset ()
	{
		label = "fakeAtoB";		//robot B is moving
		useMasterClient = false;
	}
	// TODO: check with Aaron
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
			
			if (view.text == "t") {
				transform.position = Vector3.Lerp(transform.position, view.triples[0],Time.deltaTime * 10f);
				fakeObj.GetComponent<MeshRenderer> ().enabled = true;
				realObj.GetComponent<MeshRenderer> ().enabled = false;
			} else if (view.text == "f") {
				transform.position = Vector3.Lerp(transform.position, view.triples[0],Time.deltaTime * 10f);
				fakeObj.GetComponent<MeshRenderer> ().enabled = false;
				realObj.GetComponent<MeshRenderer> ().enabled = true;
			}
		}
	}
}
