using UnityEngine;
using System.Collections;

public class magicObjCtrl : MonoBehaviour
{

	public bool _isSelected;

	public GameObject[] allObjs;

	public GameObject _wings;

	bool _isMoving;

	GameObject _table;

	bezier _path;

	public float _damping;

	bool _hasRbtArrived;

	bool _isLinkRbt;

	public GameObject rbtObj;

	public bool _DEBUG = false;

	// Use this for initialization
	void Start ()
	{
		_isSelected = false;
		_isMoving = false;
		_table = GameObject.Find ("Table");
		_damping = 4f;
		_hasRbtArrived = false;
		_isLinkRbt = false;
		_DEBUG = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// moving when selected
		moving ();
		// update position when linked
		if (_isLinkRbt) {
			if(rbtObj.GetComponent<Holojam.Network.HolojamView>().IsTracked)
				transform.position = rbtObj.transform.position;
		}

	}

	public void SetLink (bool b)
	{
		_isLinkRbt = b;
	}

	public void highlight ()
	{
		_isSelected = true;
		// show the wings
		foreach (GameObject obj in allObjs)
			obj.GetComponent<magicObjCtrl> ().hideWings ();
		showWings ();
	}

	public void showWings ()
	{
		_wings.SetActive (true);
	}

	public void hideWings ()
	{
		_wings.SetActive (false);
	}

	public void moveWanimation ()
	{
		// generate a target with position and rotation, then lerp and slerp to the target
		print(gameObject.name + "\tmoving bezier");
		generateTarget ();
		_isMoving = true;
		//_hasRbtArrived = false;
		// call the robot to move NOT FOR VIEWER
		rbtObj.GetComponent<HPSCtrl>().setDestination(_path.getPoint(1.0f));
	}

	void generateTarget ()
	{
		// generate two points for bezier generation and
		float deltax = Random.Range (-25, 25) / 100.0f;
		float deltay = Random.Range (-25, 25) / 100.0f;
		float tableSize = 1f;	// TODO
		Vector3 des = new Vector3 (deltax * tableSize, 0, deltay * tableSize) + _table.transform.position;
		//ONLY FOR TEST

		if(_DEBUG){
			des = _table.transform.position;
		}

		// another two points
		float disx = Random.Range (-2, 2);
		float disy = Random.Range (-2, 2);
		Vector3 p2 = (transform.position + des) / 2 + new Vector3 (disx, disy, 0);
		Vector3 p3 = (transform.position + des) / 2 - new Vector3 (disx, disy, 0);

		// generate bezier path
		_path = new bezier (transform.position, p2, p3, des);

		// generate the first destination
		moveDestination = _path.getPoint(0.0f);

	}

	// for movement control
	float t = 0.01f;
	Vector3 startPos;
	Quaternion startRot;
	float journey;
	float speed = 0.15f;
	//Quaternion rot;
	Vector3 finalDest;
	Vector3 moveDestination;
	Quaternion moveRotation;

	public void setFinalDest (Vector3 pos)
	{
		finalDest = pos;
		_hasRbtArrived = true;

		moveRotation = Quaternion.identity;
		moveDestination = finalDest;
	}

	void moving ()
	{
		if (_isMoving) {
			showWings ();
			//print ("moving:\t" + t);
			float curDis = Vector3.Distance (transform.position, moveDestination);
			if (curDis < 0.01f) {
				t += 0.03f;
				if (t > 1.1f) {
					_isMoving = false;
					_hasRbtArrived = false;
					_isLinkRbt = true;
					hideWings ();
					print ("arrived");
					return;
				}

				if (!_hasRbtArrived) {
					moveRotation = transform.rotation * Quaternion.Euler (new Vector3 (Random.Range (0, 20), Random.Range (-0, 35), Random.Range (-0, 30)));
					moveDestination = _path.getPoint (t);
					if (t > 1f) {
						moveRotation = Quaternion.identity;
						//print ("moving rot:\t" + moveRotation);
					}
				}
				startPos = transform.position;
				startRot = transform.rotation;
			}
			movingTo ();
		} 
	}

	// when robots arrived, speed up the speed. Or change the path to a straight line
	void movingTo ()
	{	
		//print ("moving to:\t" + moveDestination + "\t" + moveRotation);
		journey = Vector3.Distance (startPos, moveDestination);
		float curDis = Vector3.Distance (transform.position, moveDestination);
		if (journey != 0) {
			transform.position = Vector3.Lerp (startPos, moveDestination, 1 - curDis / journey * speed);
			//transform.position = Vector3.Lerp (startPos, destination, Time.deltaTime * _damping);
			transform.rotation = Quaternion.Slerp (startRot, moveRotation, 1 - curDis / journey * speed);
		}
	}

}
