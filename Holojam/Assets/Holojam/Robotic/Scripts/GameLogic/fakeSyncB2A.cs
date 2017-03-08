using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class fakeSyncB2A : Synchronizable {

	[SerializeField] string label = "fakeBtoA";
	[SerializeField] string scope = "";

	[SerializeField] bool host = true;
	[SerializeField] bool autoHost = false;

	public override string Label { get { return label; }set{ label = value; } }
	public override string Scope { get { return scope; } }

	public override bool Host { get { return host; } }
	public override bool AutoHost { get { return autoHost; } }

	public override void ResetData() {
		data = new Holojam.Network.Flake(1,0,0,0,0,true);
	}



	public GameObject fakeObj;
	public GameObject realObj;

	public Vector3 syncvec;
	public string syncst;

	protected override void Update() {
		if (autoHost) host = Sending; // Lock host flag
		base.Update();
	}

	//TODO: check with Aaron
	protected override void Sync ()
	{
		//base.Sync ();
		if (host) {
			data.vector3s[0] = transform.position;
			if (fakeObj.GetComponent<MeshRenderer> ().enabled)
				data.text = "t";
			else
				data.text = "f";
			syncvec = data.vector3s[0];
			syncst = data.text;
		} else {
			if(data.vector3s[0].magnitude > 0.01f)
				transform.position = Vector3.Lerp(transform.position, data.vector3s[0],Time.deltaTime * 10f);
			
			if (data.text == "t") {
				fakeObj.GetComponent<MeshRenderer> ().enabled = true;
				realObj.GetComponent<MeshRenderer> ().enabled = false;
			} else if (data.text == "f") {
				fakeObj.GetComponent<MeshRenderer> ().enabled = false;
				realObj.GetComponent<MeshRenderer> ().enabled = true;
			}
		}
	}
}
