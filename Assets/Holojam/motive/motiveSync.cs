using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motiveSync : Holojam.Tools.Synchronizable {

  [SerializeField]
  string label = "Example";
  [SerializeField]
  string scope = "ChalkTalk";

  public override string Label { get { return label; } }
  public override string Scope { get { return scope; } }

  public Vector3 delta = new Vector3(0.0f,0,0.0f);
  public Vector3 angle = new Vector3();

  // Expose auto hosting to the inspector
  [SerializeField] bool autoHost;

  public override bool Host {
    get { return false; }
  }
  public override bool AutoHost {
    get { return autoHost; }
  }


  // Proxies
  public Vector3 Position {
    get { return data.vector3s[0]; }
    set { data.vector3s[0] = value; }
  }
  public Quaternion Rotation {
    get { return data.vector4s[0]; }
    set { data.vector4s[0] = value; }
  }

  public override void ResetData() {
    //data = new Holojam.Network.Flake(2, 1);
    data = new Holojam.Network.Flake(1,1);
  }

  protected override void Sync() {
    if (Sending) {
      Position = transform.position;
      Rotation = transform.rotation;
     
    } else {
      //Debug.Log(data.vector3s.Length);
     
      transform.position = Position + delta;
      transform.rotation = Rotation * Quaternion.Euler(angle);
      //transform.localScale = Scale;
    }
  }
}
