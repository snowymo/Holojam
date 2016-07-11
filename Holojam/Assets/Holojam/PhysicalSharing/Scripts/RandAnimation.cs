using UnityEngine;
using System.Collections;

public class RandAnimation : MonoBehaviour
{

	public int isTimeToShow;

	public bool isRandom;

	public GameObject obj;

	public float speed;

	public float R;

	GameObject _obj;

	Vector3 destination;

	Vector3 startPos;

	float journey;

	public Vector3 finalPos;

	float curDis;

	// Use this for initialization
	void Start ()
	{
		_obj = null;	
		isRandom = false;
	}

	void createObj ()
	{
		// create an game object as OBJ is.
		if (_obj == null)
			_obj = GameObject.Instantiate (obj);
	}
	
	// Update is called once per frame
	void Update ()
	{
		// when it is time to show up
		switch (isTimeToShow) {
			case 1:
				// show and assign position
				createObj ();

				startPos = transform.position;
				generateDestination ();
				journey = Vector3.Distance (startPos, destination);
				if (journey != 0)
					transform.position = Vector3.Lerp (startPos, destination, 1 - curDis / journey + speed);
				isTimeToShow = 2;
				break;

			case 2:

				curDis = Vector3.Distance (this.transform.position, destination);
				if (curDis < 0.01) {
					generateDestination ();
					startPos = transform.position;
					journey = Vector3.Distance (startPos, destination);
					curDis = Vector3.Distance (this.transform.position, destination);
				}
				if (journey != 0)
					transform.position = Vector3.Lerp (startPos, destination, 1 - curDis / journey + speed);
				break;
			case 3:
				moving ();
				curDis = Vector3.Distance (this.transform.position, destination);
				if (curDis < 0.01) {
					_obj.SetActive (false);
					_obj = null;
					isTimeToShow = 0;
				}
			
				if (journey != 0)
					transform.position = Vector3.Lerp (startPos, destination, 1 - curDis / journey + speed * 2);
				break;

			default:
				break;
		}
	}

	public void disappear (Vector3 fpos)
	{
		if (isTimeToShow == 2)
			isTimeToShow = 3;
		else
			isTimeToShow = 0;
		finalPos = fpos;
	}

	public void appear(){
		if (isTimeToShow != 2)
			isTimeToShow = 1;

	}

	void generateDestination ()
	{
		float dis = Random.Range (0, R);
		float angle_x = Random.Range (-90, 90);
		float angle_y = Random.Range (-90, 90);
		Vector3 translation = new Vector3 (0, 0, dis);
		Quaternion rotation = Quaternion.Euler (angle_x, angle_y, 0);
		Matrix4x4 m = Matrix4x4.TRS (translation, rotation, new Vector3 (1, 1, 1));
		destination = m.MultiplyPoint3x4 (this.transform.position);
		print ("dest:\t" + destination);
	}

	void moving ()
	{
		destination = finalPos;
		startPos = transform.position;
		journey = Vector3.Distance (startPos, destination);
	}
}
