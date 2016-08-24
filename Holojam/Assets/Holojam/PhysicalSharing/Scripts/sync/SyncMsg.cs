using UnityEngine;
using System.Collections;
using Holojam;
using System.Collections.Generic;
using Holojam.Tools;

public class SyncMsg : Synchronizable
{

	void Reset ()
	{
		label = "StopSync";
		useMasterPC = false;
	}

	[Space (8)] public string handle = "";

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

//	public void Push (string message)
//	{
//		if (!sending)
//			return;
//		messages.Add (handle + ": " + message + "\n");
//		if (messages.Count > maxMessages)
//			messages.RemoveAt (0);
//	}

}
