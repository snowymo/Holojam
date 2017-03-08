using UnityEngine;
using System.Collections;


public class chessSync : Holojam.Tools.Synchronizable {
	[SerializeField] string label = "chess";
	[SerializeField] string scope = "";

	[SerializeField] bool host = true;
	[SerializeField] bool autoHost = false;

	public override string Label { get { return label; } set{ label = value; }}
	public override string Scope { get { return scope; } }

	public override bool Host { get { return host; } }
	public override bool AutoHost { get { return autoHost; } }

	public override void ResetData() {
		data = new Holojam.Network.Flake(0,0,0,0,0,true);
	}


	// Position, rotation, scale
	//public override int tripleCount{get{return 1;}}
	//public override int QuadCount{get{return 1;}}
	//public override bool hasText{get{return true;}}

	public string st;

	public string sentMsg = "";

	//const int maxMessages = 8;

	public string text = "";

	protected override void Sync() {
		//base.Sync();

		if (Host) {
			data.text = sentMsg;
		} else {
			text = data.text;
		}
	}

	protected override void Update() {
		if (autoHost) host = Sending; // Lock host flag
		base.Update();
	}
}
