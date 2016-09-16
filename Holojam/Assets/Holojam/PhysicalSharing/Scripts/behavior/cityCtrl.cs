// cityCtrl.cs
// random or purposely pick up one building, and then send command to closest robot to stand by.

using UnityEngine;
using System.Collections;

public class cityCtrl : MonoBehaviour
{

	public GameObject pickedBuilding;
	// should be chosen through some method like random generating/user selection

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.P)) {
			// unlink everything
			if (pickedBuilding != null) {
				pickedBuilding.GetComponent<cityBlockCtrl> ().setLinkIdx (-1);
			}
			GetComponent<cityRbtCtrl> ().setDestination (pickedBuilding.transform.position);
			print ("select the building");
		}

		for (int i = 0; i < 2; i++)
			if (Vector3.Distance (pickedBuilding.transform.position, GetComponent<cityRbtCtrl> ().Rbts [i].transform.position) < Utility.getInst ().disError) {
				pickedBuilding.GetComponent<cityBlockCtrl> ().setLinkIdx (i);
				print ("set link idx:\t" + i);
			}
	}

	public void setPickBuilding(GameObject go){
		pickedBuilding = go;
	}
}
