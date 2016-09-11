using UnityEngine;
using System.Collections;

public class chessCtrl : MonoBehaviour {

	private float startCollide;
	private float endCollide;

	Transform circle;
	Transform cross;
	public bool isHighLight;

	// Use this for initialization
	void Start () {
		endCollide = startCollide = Time.time;
		// disable other two models
		Transform[] ts = transform.GetComponentsInChildren<Transform>();
		foreach (Transform t in ts) {
			if (t.gameObject.name == "circle") {
				circle = t;
			}
			if (t.gameObject.name == "cross") {
				cross = t;
			}
		}
		circle.gameObject.SetActive (false);
		cross.gameObject.SetActive (false);
		isHighLight = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// highlight
	public void highLight(){
		transform.position = new Vector3 (transform.position.x, transform.parent.transform.position.y + 0.02f, transform.position.z);
		isHighLight = true;
	}

	public void reset(){
		transform.position = new Vector3 (transform.position.x, transform.parent.transform.position.y, transform.position.z);
		startCollide = 0;
		endCollide = 0;
		isHighLight = false;
	}

	public void setStartCollide(){
		startCollide = Time.time;
	}

	public void setEndCollide(){
		endCollide = Time.time;
	}

	public void hover(){
		// set start if no
		// set end if start
	}

	public bool isSelect(){
		if (endCollide - startCollide > 1.5f) {
			//print (name + "\tend:\t" + endCollide + "\tstart:\t" + startCollide);
			return true;
		}
		else
			return false;
	}

	// select the cube
	public bool select(string name){
		// check if already select
//		if ((circle.gameObject.activeSelf && name.Equals ("circle"))
//		   || (cross.gameObject.activeSelf && name.Equals ("cross")))
		if ((circle.gameObject.activeSelf)
			|| (cross.gameObject.activeSelf))
			return false;
		
		Transform[] ts = transform.GetComponentsInChildren<Transform>();
		foreach (Transform t in ts) {
			//print (t.gameObject.name);
			if (t.gameObject.name == "default") {
				t.gameObject.SetActive (false);
			}
		}
		if (name == "circle") {
			circle.gameObject.SetActive (true);
		} else if (name == "cross") {
			cross.gameObject.SetActive (true);
		} 
		return true;
	}
}
