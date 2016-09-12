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
		useMasterPC = false;
	}

	protected override void Sync ()
	{
		if (sending) {
			synchronizedVector3 = transform.position;
			if (fakeObj.GetComponent<MeshRenderer> ().enabled)
				synchronizedString = "t";
			else
				synchronizedString = "f";
			syncvec = synchronizedVector3;
			syncst = synchronizedString;
		} else {
			transform.position = Vector3.Lerp(transform.position, synchronizedVector3,Time.deltaTime * 10f);
			if (synchronizedString == "t") {
				fakeObj.GetComponent<MeshRenderer> ().enabled = true;
				realObj.GetComponent<MeshRenderer> ().enabled = false;
			} else if (synchronizedString == "f") {
				fakeObj.GetComponent<MeshRenderer> ().enabled = false;
				realObj.GetComponent<MeshRenderer> ().enabled = true;
			}
		}
	}
}
