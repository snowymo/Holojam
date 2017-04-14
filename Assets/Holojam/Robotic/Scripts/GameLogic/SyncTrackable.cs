using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTrackable : Holojam.Tools.SynchronizableTrackable {

	[SerializeField] string label = "password";
	[SerializeField] string scope = "";

	[SerializeField] bool host = true;
	[SerializeField] bool autoHost = false;

	public override string Label { get { return label; } }
	public override string Scope { get { return scope; } }

	public override bool Host { get { return host; } }
	public override bool AutoHost { get { return autoHost; } }

	// Add the scale vector to Trackable
	public override void ResetData() {
		data = new Holojam.Network.Flake(1,1);
	}

	protected override void Sync() {
		if (Host) {
			data.vector3s[0] = transform.position;
			data.vector4s[0] = transform.rotation;
		} else {
			transform.position = data.vector3s[0];
			transform.rotation = data.vector4s[0];
		}
	}

	protected override void Update() {
		if (autoHost) host = Sending; // Lock host flag
		base.Update();
	}
}
