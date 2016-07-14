using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandAnimation : MonoBehaviour
{

	public int isTimeToShow;

	public bool isRandom;

	public GameObject obj;

	public float speed;

	public float R;

	List<GameObject> _objs;

	List<Vector3> destination;

	List<Vector3> startPos;

	List<float> journey;

	public Vector3 finalPos;

	float curDis;

	int randNum;

	// Use this for initialization
	void Start ()
	{
		isRandom = false;
		_objs = new List<GameObject> ();
		destination = new List<Vector3> ();
		startPos = new List<Vector3> ();
		journey = new List<float> ();
	}

	void initialRands ()
	{
		// create an game object as OBJ is.
		if (_objs.Count == 0){
			randNum = Random.Range (10, 20);
			for (int i = 0; i < randNum; i++) {
				_objs.Add ((GameObject)GameObject.Instantiate (obj, obj.transform.position, obj.transform.rotation));
				_objs[i].SetActive (true);
				startPos.Add (_objs[i].transform.position);
				Vector3 dest = generateDestination (_objs[i].transform.position);
				destination.Add (dest);
				journey.Add (Vector3.Distance (startPos[i], dest));
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// when it is time to show up
		switch (isTimeToShow) {
		case 1:
				// show and assign position
			initialRands ();

			isTimeToShow = 2;
			break;

		case 2:
			for (int i = 0; i < randNum; i++) {
				curDis = Vector3.Distance (_objs[i].transform.position, destination[i]);
				if (curDis < 0.01) {
					Vector3 dest = generateDestination (_objs[i].transform.position);
					startPos[i] = _objs[i].transform.position;
					destination [i] = dest;
					journey[i] = Vector3.Distance (startPos[i], destination[i]);
					curDis = Vector3.Distance (_objs[i].transform.position, destination[i]);
				}
				if (journey[i] != 0)
					_objs[i].transform.position = Vector3.Lerp (startPos[i], destination[i], 1 - curDis / journey[i] + speed);
			}

			break;
		case 3:
			for (int i = 0; i < randNum; i++) {
				destination[i] = finalPos;
				startPos[i] = _objs[i].transform.position;
				journey[i] = Vector3.Distance (startPos[i], destination[i]);
				curDis = Vector3.Distance (_objs[i].transform.position, destination[i]);
				if (journey[i] != 0)
					_objs[i].transform.position = Vector3.Lerp (startPos[i], destination[i], 1 - curDis / journey[i] + speed * 2);
			}
			isTimeToShow = 4;
			break;

		case 4:
			bool alldone = true;
			for (int i = 0; i < randNum; i++) {
				curDis = Vector3.Distance (_objs [i].transform.position, destination[i]);

				if (curDis >= 0.01) {
					_objs [i].transform.position = Vector3.Lerp (startPos[i], destination[i], 1 - curDis / journey[i] + speed * 2);
					alldone = false;
				}
			}
				
			if (alldone) {
				_objs.Clear ();
				isTimeToShow = 0;
			}

			break;

		default:
			break;
		}
	}

	public void disappear (Vector3 fpos)
	{
		if (isTimeToShow == 2)
			isTimeToShow = 3;
//		else if(isTimeToShow != 4)
//			isTimeToShow = 0;
		finalPos = fpos;
	}

	public void appear (bool isrdtomv)
	{
		if (isrdtomv && (isTimeToShow != 2)) {
			isTimeToShow = 1;
			Debug.LogWarning ("appear");
		}

	}

	Vector3 generateDestination (Vector3 pos)
	{
		float dis = Random.Range (0, R);
		float angle_x = Random.Range (-90, 90);
		float angle_y = Random.Range (-90, 90);
		Vector3 translation = new Vector3 (0, 0, dis);
		Quaternion rotation = Quaternion.Euler (angle_x, angle_y, 0);
		Matrix4x4 m = Matrix4x4.TRS (translation, rotation, new Vector3 (1, 1, 1));
		Vector3 dest = m.MultiplyPoint3x4 (pos);
		return dest;
		//print ("dest:\t" + destination);
	}

//	void moving ()
//	{
//		destination = finalPos;
//		startPos = _obj.transform.position;
//		journey = Vector3.Distance (startPos, destination);
//	}
}
