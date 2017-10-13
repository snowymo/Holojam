//GestureTarget.cs
//Created by Aaron C Gaudette on 11.09.16

//modified on Zhenyi's computer

using Holojam.Tools;
using UnityEngine;

public class GestureTarget : SynchronizableTrackable {
	public bool locked = false;
	public bool highlight = false;
	public bool loading = false;
	public bool controllable = false, controlled = false;

	public float tableOffset = 0.001f;

	public Transform robot, table; //

	public GameObject glow;
	public TextMesh buffering;

  [SerializeField] string label = "GT0";
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
    data = new Holojam.Network.Flake(1, 1, 0, 1, 0, true);
    
  }


  protected override void Sync ()
	{
    base.Sync();

		if (Sending) {
			if (controllable) {
				float h = robot.position.y - table.position.y;
				//print (h); //
				//controlled = h >= tableOffset;
			} else
				controlled = false;
			if (controllable){
				transform.position = robot.position;
				transform.rotation = robot.rotation;
			}

			data.vector3s[0] = transform.position;
			data.vector4s[0] = transform.rotation;
			data.text = highlight ? "True" : "False";
			data.ints[0] = loading ? 1 : 0;
		} else {
			transform.position = data.vector3s[0];
			transform.rotation = data.vector4s[0];
			highlight = data.text == "True";
			loading = data.ints[0] == 1;
			locked = false;
		}
		if(glow!=null)glow.SetActive (highlight);

		if(buffering==null)return;

		buffering.gameObject.SetActive (loading);
		if (loading) {
			float div = Time.time % 3;
			buffering.text = div < 1 ? ".  " : div >= 2 ? "..." : ".. ";
		}
	}
}