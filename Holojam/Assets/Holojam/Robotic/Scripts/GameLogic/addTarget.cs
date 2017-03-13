using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addTarget :MonoBehaviour
{

	// add GestureTarget to all buildings for children
	void Start ()
	{
		int i = 0;
		foreach (Transform child in transform) {
			foreach (Transform grandChild in child) {
				if (grandChild.gameObject.name.Contains ("Building")) {
					GestureTarget gt = child.gameObject.AddComponent <GestureTarget> ();
					gt.Label = "GT" + i.ToString ();
					i++;
					break;
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

}
