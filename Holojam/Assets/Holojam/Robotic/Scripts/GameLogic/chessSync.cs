using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class chessSync : Synchronizable {

	void Reset ()
	{
		label = "chess";
		useMasterClient = false;
	}

	[Space (8)] public string handle = "";

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
