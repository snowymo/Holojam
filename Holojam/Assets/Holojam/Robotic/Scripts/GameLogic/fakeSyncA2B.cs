using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class fakeSyncA2B : Synchronizable
{

	//public string label = "fakeAtoB";
	//public string scope = "";

	// Position, rotation, scale
	public override int tripleCount{get{return 1;}}
	//public override int quadCount{get{return 1;}}
	public override bool hasText{get{return true;}}

	public override string labelField { get { return label; } }
	public override string scopeField { get { return scope; } }

	// Proxies
//	public Vector3 Position{
//		get{return GetTriple(0);}
//		set{SetTriple(0,value);}
//	}
//	public Quaternion Rotation{
//		get{return GetQuad(0);}
//		set{SetQuad(0,value);}
//	}
//	public Vector3 Scale{
//		get{return GetTriple(1);}
//		set{SetTriple(1,value);}
//	}

	public GameObject fakeObj;
	public GameObject realObj;

	public Vector3 syncvec;
	public string syncst;


	// TODO: check with Aaron
	protected override void Sync ()
	{
		if (sending) {
//			Position = transform.position;
//			Rotation = transform.rotation;
//			Scale = transform.localScale;

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
