using UnityEngine;
using System.Collections;

public class randCtrl : MonoBehaviour
{

	public GameObject curObj;

	public GameObject refObj;

	public GameObject shape;

	public bool isReadyToMove = false;

	RandAnimation randAnime;

	ReadyMsg readyMsg;

	// Use this for initialization
	void Start ()
	{
		randAnime = this.GetComponent<RandAnimation> ();


		readyMsg = this.GetComponent<ReadyMsg> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		// update if it is ready
		if (!isReadyToMove) {
			if (Utility.getInst ().checkMatchV2 (curObj.transform.position, refObj.transform.position)) {
				this.GetComponent<ReadyMsg> ().sentMsg = "ready";
				isReadyToMove = true;
			}
				
//			if (readyMsg.text != null) {
//				if (readyMsg.text.Equals ("ready"))
//					isReadyToMove = true;
//				if (readyMsg.sentMsg.Equals ("ready"))
//					isReadyToMove = true;
//			}
		}

		if (Utility.getInst ().checkMatchV2 (curObj.transform.position, refObj.transform.position)) {
			randAnime.disappear (curObj.transform.position);	
			Debug.LogWarning ("disappear");
		} else {
			randAnime.appear (isReadyToMove);
			shape.SetActive (false);
		}

		if (randAnime.isTimeToShow == 0)
			shape.SetActive (true);
	}
}
