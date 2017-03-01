using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class fakeSyncB2A : Synchronizable {

	//[SerializeField] string label = "fakeBtoA";
	//[SerializeField] string scope = "";

	// Position, rotation, scale
	public override int tripleCount{get{return 1;}}
	//public override int QuadCount{get{return 1;}}
	public override bool hasText{get{return true;}}

	public override string labelField { get { return label; } }
	public override string scopeField { get { return scope; } }


	public GameObject fakeObj;
	public GameObject realObj;

	public Vector3 syncvec;
	public string syncst;

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
