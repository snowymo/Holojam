using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syncRoomba : Holojam.Tools.SynchronizableTrackable {

	[SerializeField] string label = "roomba";
	[SerializeField] string scope = "";

	[SerializeField] bool host = true;
	[SerializeField] bool autoHost = false;

	public Vector3 command;

	public override string Label { get { return label; } }
	public override string Scope { get { return scope; } }

	public override bool Host { get { return host; } }
	public override bool AutoHost { get { return autoHost; } }

	protected override void  Awake(){
		base.Awake ();
		command = new Vector3 (0, 0, 0);
	}

	public Vector3 monitor;


	// Add the scale vector to Trackable
	public override void ResetData() {
		data = new Holojam.Network.Flake(1);
	}

	protected override void Sync() {
		//base.Sync();

		if (Host) {
			data.vector3s [0] = command;
		} 
		else {
			//data.vector3s [0] = new Vector3(0,0,0);
		}

		monitor = data.vector3s [0];
		//host = false;
	}

	public void setStraight(float straightSp){
		command.x = straightSp;
		command.y = 0;
	}

	public void setTurn(float turnSp){
		command.y = turnSp;
		command.x = 0;
	}

//	protected override void Update() {
//		if (autoHost) host = Sending; // Lock host flag
//		base.Update();
//	}
}
