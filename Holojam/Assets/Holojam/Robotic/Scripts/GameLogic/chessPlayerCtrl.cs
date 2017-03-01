using UnityEngine;
using System.Collections;

public class chessPlayerCtrl : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter (Collider collisionInfo)
	{
		print ("Detected collision between " + gameObject.name + " and " + collisionInfo.gameObject.name);
		//print("There are " + collisionInfo.contacts.Length + " point(s) of contacts");
		//print("Their relative velocity is " + collisionInfo.relativeVelocity);
		if (collisionInfo.gameObject.transform.parent.name.Contains ("chess")
		   && (collisionInfo.gameObject.transform.parent.parent == transform.parent.transform.parent)) {
			// highight the chess
			// only when delay mesh is off, which means the animation is over
			if (gameObject.GetComponent<MeshRenderer> ().enabled) {
				collisionInfo.gameObject.transform.parent.GetComponent<chessCtrl> ().highLight ();
			}
			collisionInfo.gameObject.transform.parent.GetComponent<chessCtrl> ().setStartCollide ();
		}
	}

	void OnTriggerStay (Collider collisionInfo)
	{
//		print(gameObject.name + " and " + collisionInfo.gameObject.name + " are still colliding");
		// after two seconds, change patterns
		if (collisionInfo.gameObject.transform.parent.name.Contains ("chess")
		    && (collisionInfo.gameObject.transform.parent.parent == transform.parent.transform.parent)) {
			if (gameObject.GetComponent<MeshRenderer> ().enabled)
				collisionInfo.gameObject.transform.parent.GetComponent<chessCtrl> ().setEndCollide ();
		}
	}

	void OnTriggerExit (Collider collisionInfo)
	{
		print (gameObject.name + " and " + collisionInfo.gameObject.name + " are no longer colliding");
		if (collisionInfo.gameObject.transform.parent.name.Contains ("chess")
		   && (collisionInfo.gameObject.transform.parent.parent == transform.parent.transform.parent)) {
			// highight the chess
			//if (gameObject.GetComponent<MeshRenderer> ().enabled)
				collisionInfo.gameObject.transform.parent.GetComponent<chessCtrl> ().reset ();
		}
	}

}
