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

	// Use this for initialization
	void Start ()
	{
		_isSelected = false;
		_isMoving = false;
		_table = GameObject.Find ("Table");
		_damping = 4f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		moving ();
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
		generateTarget ();
		_isMoving = true;

		// call the robot to move
	}

	void generateTarget ()
	{
		// generate two points for bezier generation and
		float deltax = Random.Range (-3, 3) / 10.0f;
		float deltay = Random.Range (-3, 3) / 10.0f;
		float tableSize = 1f;	// TODO
		Vector3 des = new Vector3 (deltax * tableSize, 0, deltay * tableSize) + _table.transform.position;

		// another two points
		float disx = Random.Range (-2, 2);
		float disy = Random.Range (-2, 2);
		Vector3 p2 = (transform.position + des) / 2 + new Vector3 (disx, disy, 0);
		Vector3 p3 = (transform.position + des) / 2 - new Vector3 (disx, disy, 0);

		// generate bezier path
		_path = new bezier (transform.position, p2, p3, des);

	}

	// for movement control
	float t = 0.01f;
	Vector3 startPos;
	Quaternion startRot;
	float journey;
	float speed = 0.15f;
	Quaternion rot;
	void moving ()
	{
		if (_isMoving) {
			showWings ();
			print ("moving:\t" + t);

			float curDis = Vector3.Distance (transform.position, _path.getPoint (t));
			if (curDis < 0.01f) {
				t += 0.03f;
				if (t > 1.1f) {
					_isMoving = false;
					hideWings ();
					return;
				}
				startPos = transform.position;
				rot = transform.rotation * Quaternion.Euler (new Vector3 (Random.Range (0,20), Random.Range (-0,35), Random.Range (-0,30)));
				startRot = transform.rotation;
				if (t > 1f) {
					rot = Quaternion.identity;
					print ("moving rot:\t" + rot);
				}
			}


			movingTo (_path.getPoint (t), rot);
		} 
			
	}

	// when robots arrived, speed up the speed. Or change the path to a straight line
	void movingTo (Vector3 destination, Quaternion rotation)
	{
		print ("moving to:\t" + destination + "\t" + rotation);
		journey = Vector3.Distance (startPos, destination);
		float curDis = Vector3.Distance (transform.position, destination);
		if (journey != 0) {
			transform.position = Vector3.Lerp (startPos, destination, 1 - curDis / journey * speed);
			//transform.position = Vector3.Lerp (startPos, destination, Time.deltaTime * _damping);
			transform.rotation = Quaternion.Slerp (startRot, rotation, 1 - curDis / journey * speed);
		}
	}

}
