using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addTarget :MonoBehaviour
{

	public Transform tableTrans;

	public GameObject forceCube;

	// add GestureTarget to all buildings for children
	void Start ()
	{
		int i = 0;
		foreach (Transform child in transform) {
			foreach (Transform grandChild in child) {
				if (grandChild.gameObject.name.Contains ("Building")) {
					GameObject gragrandChild = GameObject.Instantiate (forceCube);
					gragrandChild.transform.parent = grandChild.transform;
					gragrandChild.transform.localPosition = new Vector3 ();

					GestureTarget gt = gragrandChild.GetComponent <GestureTarget> ();
					gt.Label = "GT" + i.ToString ();
					gt.table = tableTrans;

//					TextMesh tm = gragrandChild.gameObject. .GetComponent <TextMesh> ();
//					tm.text = "";
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
