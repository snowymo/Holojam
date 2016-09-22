using UnityEngine;
using System.Collections;
using Holojam.Tools;

public class chessSync : Synchronizable {

	void Reset ()
	{
		label = "chess";
		useMasterPC = false;
	}

	[Space (8)] public string handle = "";

	public string st;

	public string sentMsg = "";

	//const int maxMessages = 8;

	public string text = "";

	protected override void Sync ()
	{
		if (sending) {
			synchronizedString = sentMsg;
		} else
			text = synchronizedString;
	}

}
