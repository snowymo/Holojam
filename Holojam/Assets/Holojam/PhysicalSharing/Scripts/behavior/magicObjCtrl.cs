using UnityEngine;
using System.Collections;

public class magicObjCtrl : MonoBehaviour
{

	public bool _isSelected;

	public GameObject[] allObjs;

	public GameObject _wings;

	bool _isMoving;

	GameObject _table;

	public bezier _path;

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
		_DEBUG = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// moving when selected
		moving ();
		// update position when linked
		if (_isLinkRbt) {
			if (rbtObj.GetComponent<Holojam.Network.HolojamView> ().IsTracked)
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
		//print (gameObject.name + "\tmoving bezier");
		Vector3 des4Rbt = generateTarget ();
		_isMoving = true;
		//_hasRbtArrived = false;
		// call the robot to move NOT FOR VIEWER
		rbtObj.GetComponent<HPSCtrl> ().setDestination (des4Rbt);
	}

	Vector3 generateTarget ()
	{
		// generate according to the robot's position
		//Vector3 robotDis = rbtObj.transform.position - _table.transform.position;
		Vector3 mir = 2 * _table.transform.position - rbtObj.transform.position;
		float tableSize = 1f;
		// generate two points for bezier generation and
		// algorithms3
		float deltax = Random.Range (Mathf.Max(-20f + _table.transform.position.x * 100f,-15 + mir.x * 100f),
			Mathf.Min(20f + _table.transform.position.x * 100f, 15 + mir.x * 100f)) / 100.0f;
		float deltaz = Random.Range (Mathf.Max(-20f + _table.transform.position.z * 100f,-15 + mir.z * 100f),
			Mathf.Min(20f + _table.transform.position.z * 100f, 15 + mir.z * 100f)) / 100.0f;
		print ("dx:\t" + deltax + "\tdy:\t" + deltaz + "\trbt:\t" + rbtObj.transform.position + "\ttbl:\t" + _table.transform.position);

		while (Vector3.Distance (rbtObj.transform.position, new Vector3 (deltax, rbtObj.transform.position.y, deltaz)) < 0.2f) {
			print ("redo");
			deltax = Random.Range (-20, 20) / 100.0f + _table.transform.position.x;
			deltaz = Random.Range (-20, 20) / 100.0f + _table.transform.position.z;
		}
		deltax = Mathf.Min (_table.transform.position.x + 0.2f, deltax);
		deltax = Mathf.Max (_table.transform.position.x - 0.2f, deltax);
		deltaz = Mathf.Min (_table.transform.position.z + 0.2f, deltaz);
		deltaz = Mathf.Max (_table.transform.position.z - 0.2f, deltaz);

		Vector3 des = new Vector3 (deltax * tableSize, _table.transform.position.y, deltaz * tableSize);// + _table.transform.position;
		print ("des:\t" + des.ToString("F3"));
		
		//ONLY FOR TEST
		if (_DEBUG) {
			des = _table.transform.position;
		}

		// try two way animation that it goes to some floating place first and then get on the table
		Vector3 desForRbt = des;
		des.y += (transform.position.y - _table.transform.position.y)/3f; 

		// another two points
		float disx = Random.Range (1, 3) * (Random.Range(0,1) * 2 - 1);
		float disy = Random.Range (1, 3)* (Random.Range(0,1) * 2 - 1);
		Vector3 p2 = (transform.position + des) / 2 + new Vector3 (disx, disy, 0);
		Vector3 p3 = (transform.position + des) / 2 - new Vector3 (disx, disy, 0);

		// generate bezier path
		_path = new bezier (transform.position, p2, p3, des);

		// generate the first destination
		moveDestination = _path.getPoint (0.0f);

		// reset the speed
		speed = 0.05f;

		return desForRbt;
	}

	// for movement control
	float t = 0.01f;
	Vector3 startPos;
	Quaternion startRot;
	float journey;
	float speed = 0.05f;
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

		startPos = transform.position;
		startRot = transform.rotation;
		speed = 0.1f;
//		t = 1f;
		_isMoving = true;
	}

	void moving ()
	{
		if (_isMoving) {
			//print ("moving");
			showWings ();
			//print ("moving:\t" + t);
			float curDis = Vector3.Distance (transform.position, moveDestination);
			print ("cur dis:\t" + curDis); 

			if (_hasRbtArrived) {
				if (curDis < 0.08f) {
					_isMoving = false;
					hideWings ();
					_isLinkRbt = true;
					print ("arrived");
				}
			}
			else if (curDis < 0.08f) {
				t = Mathf.Min(t+0.01f, 1.0f);

				moveRotation = transform.rotation * Quaternion.Euler (new Vector3 (Random.Range (0, 20), Random.Range (-0, 35), Random.Range (-0, 30)));
				moveDestination = _path.getPoint (t);
				if (t > 0.999f) {
					moveRotation = Quaternion.identity;
					//print ("moving rot:\t" + moveRotation);
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
		float curDis = Vector3.Distance (transform.position, startPos);
		if (journey > 0.01f) {
			transform.position = Vector3.Lerp (startPos, moveDestination, curDis / journey + speed);
			//transform.position = Vector3.Lerp (startPos, destination, Time.deltaTime * _damping);
			transform.rotation = Quaternion.Slerp (startRot, moveRotation, curDis / journey + speed);
		}
	}

}
