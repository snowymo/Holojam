using UnityEngine;
using System.Collections;
using Holojam;
using System.Collections.Generic;
using Holojam.Tools;

public class SyncMsg : SynchronizableTrackable {

  // As an example, expose all the Synchronizable properties in the inspector.
  // In practice, you probably want to control some or all of these manually in code.

  [SerializeField] string label = "StopSync";
  [SerializeField] string scope = "";

  [SerializeField] bool host = true;
  [SerializeField] bool autoHost = false;

  // As an example, allow all the Synchronizable properties to be publicly settable
  // In practice, you probably want to control some or all of these manually in code.

  public void SetLabel(string label) { this.label = label; }
  public void SetScope(string scope) { this.scope = scope; }

  public void SetHost(bool host) { this.host = host; }
  public void SetAutoHost(bool autoHost) { this.autoHost = autoHost; }

  // Point the property overrides to the public inspector fields

  public override string Label { get { return label; } }
  public override string Scope { get { return scope; } }

  public override bool Host { get { return host; } }
  public override bool AutoHost { get { return autoHost; } }

  // Add the scale vector to Trackable, which by default only contains position/rotation
  public override void ResetData() {
    data = new Holojam.Network.Flake(0, 0, 0, 0, 0, true);
  }

  // Override Sync() to include the scale vector
  protected override void Sync() {
    base.Sync();

    if (Sending) {
      data.text = sentMsg;
    } else {
      rcvMsg = data.text;
    }
  }

  public string sentMsg = "";
  public string rcvMsg = "";

}
