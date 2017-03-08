using UnityEngine;
using System.Collections;
using Holojam.Tools;

// maybe not SynchronizableTrackable, maybe just synchronizable, it might change this obj's position, as long as I don't need it, it doesnot matter
public class fakeSyncA2B : Synchronizable
{
	[SerializeField] string label = "fakeAtoB";
	[SerializeField] string scope = "";

	[SerializeField] bool host = true;
	[SerializeField] bool autoHost = false;

	public override string Label { get { return label; } set { label = value; } }
	public override string Scope { get { return scope; } }

	public override bool Host { get { return host; } }
	public override bool AutoHost { get { return autoHost; } }

	public override void ResetData() {
		data = new Holojam.Network.Flake(1,0,0,0,0,true);
	}

	//public string label = "fakeAtoB";
	//public string scope = "";

	// Position, rotation, scale
//	public override int tripleCount{get{return 1;}}
//	//public override int quadCount{get{return 1;}}
//	public override bool hasText{get{return true;}}
//
//	public override string labelField { get { return label; } }
//	public override string scopeField { get { return scope; } }

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


	protected override void Update() {
		if (autoHost) host = Sending; // Lock host flag
		base.Update();
	}

	// TODO: check with Aaron
	protected override void Sync ()
	{
		//base.Sync();
		if (Host) {
//			Position = transform.position;
//			Rotation = transform.rotation;
//			Scale = transform.localScale;

			data.vector3s[0] = transform.position;
			if (fakeObj.GetComponent<MeshRenderer> ().enabled)
				data.text = "t";
			else
				data.text = "f";
			syncvec = data.vector3s[0];
			syncst = data.text;
		} else {
			
			if (data.text == "t") {
				transform.position = Vector3.Lerp(transform.position, data.vector3s[0],Time.deltaTime * 10f);
				fakeObj.GetComponent<MeshRenderer> ().enabled = true;
				realObj.GetComponent<MeshRenderer> ().enabled = false;
			} else if (data.text == "f") {
				transform.position = Vector3.Lerp(transform.position, data.vector3s[0],Time.deltaTime * 10f);
				fakeObj.GetComponent<MeshRenderer> ().enabled = false;
				realObj.GetComponent<MeshRenderer> ().enabled = true;
			}
		}
	}
}
