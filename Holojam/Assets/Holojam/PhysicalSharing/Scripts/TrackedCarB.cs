using UnityEngine;
using System.Collections;

public class TrackedCarB : MonoBehaviour {

	public bool isReadyToMove = false;

	public GameObject refObj;

	public GameObject carGeo;

	RandAnimation _ra;

	//CarCtrl _cc;

	// Use this for initialization
	void Start () {
		_ra = GameObject.Find ("RandomAnimation").GetComponent<RandAnimation> ();
		//_cc = gameObject.GetComponent<CarCtrl> ();
	}
	
	// Update is called once per frame
	void Update () {
		//float angle = Quaternion.Dot(transform.rotation, refObj.transform.rotation);
		//float dis = Vector3.Distance (new Vector3 (transform.position.x, 0, transform.position.z), new Vector3 (refObj.transform.position.x, 0, refObj.transform.position.z));
		if (Utility.getInst().checkMatchV2(transform.position, refObj.transform.position)
		//	&& (Mathf.Abs (Mathf.Abs (angle) - 1.0f) < rotateError)
		) {
			_ra.disappear(transform.position);	
			Debug.LogWarning ("disappear");
		} else {
			
			_ra.appear (isReadyToMove);
			carGeo.SetActive (false);
		}

		if (_ra.isTimeToShow == 0)
			carGeo.SetActive (true);
	}
}
