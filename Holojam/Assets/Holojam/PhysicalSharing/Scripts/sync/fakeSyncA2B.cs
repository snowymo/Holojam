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
			
			if (synchronizedString == "t") {
				transform.position = Vector3.Lerp(transform.position, synchronizedVector3,Time.deltaTime * 10f);
				fakeObj.GetComponent<MeshRenderer> ().enabled = true;
				realObj.GetComponent<MeshRenderer> ().enabled = false;
			} else if (synchronizedString == "f") {
				transform.position = Vector3.Lerp(transform.position, synchronizedVector3,Time.deltaTime * 10f);
				fakeObj.GetComponent<MeshRenderer> ().enabled = false;
				realObj.GetComponent<MeshRenderer> ().enabled = true;
			}
		}
	}
}
