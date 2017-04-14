using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syncDoIt : Holojam.Tools.SynchronizableTrackable{

	[SerializeField] string label = "doit";
	[SerializeField] string scope = "";

	[SerializeField] bool host = true;
	[SerializeField] bool autoHost = false;

	public override string Label { get { return label; } }
	public override string Scope { get { return scope; } }

	public override bool Host { get { return host; } }
	public override bool AutoHost { get { return autoHost; } }


	// Add the scale vector to Trackable
	public override void ResetData() {
		data = new Holojam.Network.Flake(0,0,0,2);
	}

	protected override void Sync() {
		if (Host) {
			data.ints[0] = (GetComponent<envSwitchCtrl>().doit) ? 1 : 0;
			data.ints[1] = (GetComponent<envSwitchCtrl>().win) ? 1 : 0;
		} else {
			GetComponent<envSwitchCtrl>().doit = (data.ints[0] > 0);
			GetComponent<envSwitchCtrl>().win = (data.ints[1] > 0);
		}
	}

	protected override void Update() {
		if (autoHost) host = Sending; // Lock host flag
		base.Update();
	}
}
