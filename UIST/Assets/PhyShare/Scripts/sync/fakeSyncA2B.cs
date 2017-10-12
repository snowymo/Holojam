using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class fakeSyncA2B : SynchronizableTrackable {

	public GameObject fakeObj;
	public GameObject realObj;

	public Vector3 syncvec;
	public string syncst;

  [SerializeField] string label = "fakeAtoB";
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
    data = new Holojam.Network.Flake(1, 0, 0, 0, 0, true);
  }

  // Override Sync() to include the scale vector
  protected override void Sync() {
    base.Sync();

    if (Sending) {
      data.vector3s[0] = transform.position;
      if (fakeObj.GetComponent<MeshRenderer>().enabled)
        data.text = "t";
      else
        data.text = "f";
      syncvec = data.vector3s[0];
      syncst = data.text;
    } else {
      if(data.text == "t") {
        transform.position = Vector3.Lerp(transform.position, data.vector3s[0], Time.deltaTime * 10f);
        fakeObj.GetComponent<MeshRenderer>().enabled = true;
        realObj.GetComponent<MeshRenderer>().enabled = false;
      }
      else if(data.text == "f") {
        transform.position = Vector3.Lerp(transform.position, data.vector3s[0], Time.deltaTime * 10f);
        fakeObj.GetComponent<MeshRenderer>().enabled = false;
        realObj.GetComponent<MeshRenderer>().enabled = true;
      }
    }
  }
}
