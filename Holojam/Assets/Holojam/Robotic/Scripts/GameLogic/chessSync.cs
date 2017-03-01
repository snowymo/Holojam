using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class chessSync : Synchronizable {

	// Position, rotation, scale
	//public override int tripleCount{get{return 1;}}
	//public override int QuadCount{get{return 1;}}
	public override bool hasText{get{return true;}}

	public override string labelField { get { return label; } }
	public override string scopeField { get { return scope; } }

//
//	void Reset ()
//	{
//		label = "chess";
//		useMasterClient = false;
//	}
//
//	[Space (8)] public string handle = "";

	public string st;

	public string sentMsg = "";

	//const int maxMessages = 8;

	public string text = "";

	// TODO: need to sync with Aaron
	protected override void Sync ()
	{
		if (sending) {
			view.text = sentMsg;
		} else
			text = view.text;
	}

}
