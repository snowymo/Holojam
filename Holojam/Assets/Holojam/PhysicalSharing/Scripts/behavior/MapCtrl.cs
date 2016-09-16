// created by Zhenyi He 9/16/16
// control the navigation of map

using UnityEngine;
using System.Collections;

public class MapCtrl : MonoBehaviour {

	int zoomstage;

	Vector2 mappos;

	Vector3 defaultPos = new Vector3(0.45f,0,0.35f);
//	Vector3 defaultScale = new Vector3(0.004f,0.004f,0.004f);
	Vector3 defaultScale = new Vector3(1f,1f,1f);

	Transform parent;

	// Use this for initialization
	void Start () {
		zoomstage = 0;	// 0 means control mode, 1 means high and navigation mode
		mappos = new Vector2(); // 0,0 means left and down 1,0 means right and down, 0,1 means left and up
		parent = transform.parent;
	}

	
	// Update is called once per frame
	void Update () {
		// keyboard test
		if (Input.GetKey (KeyCode.UpArrow))
			moveforward ();
		else if (Input.GetKey (KeyCode.DownArrow))
			movebackward ();
		else if (Input.GetKey (KeyCode.LeftArrow))
			moveleft ();
		else if (Input.GetKey (KeyCode.RightArrow))
			moveright ();
		else if (Input.GetKey (KeyCode.U))
			zoomout ();
		else if (Input.GetKey (KeyCode.D))
			zoomin ();

		// smoothly lerp to the position based on zoom stage and mappos
		Vector3 newmappos = transform.localPosition;
		Vector3 newmapscale = parent.localScale;
		//newmappos.y = (zoomstage == 0) ? 0f : -100f;
		newmapscale = (zoomstage == 0) ? defaultScale : (defaultScale / 3f);
		newmappos.x = (mappos.x == 0) ? defaultPos.x : defaultPos.x - 0.7f;
		newmappos.z = (mappos.y == 0) ? defaultPos.z : defaultPos.z - 0.75f;
		transform.localPosition = Vector3.Lerp (transform.localPosition, newmappos, Time.deltaTime * 10f);
		parent.localScale = Vector3.Lerp (parent.localScale, newmapscale, Time.deltaTime * 10f);
	}

	void zoomin(){
		zoomstage = 0;
	}

	void zoomout(){
		zoomstage = 1;
	}

	void moveleft(){
		if (zoomstage == 1) {
			mappos.x = 0;
		}
	}

	void moveright(){
		if (zoomstage == 1) {
			mappos.x = 1;
		}
	}

	void moveforward(){
		if (zoomstage == 1) {
			mappos.y = 1;
		}
	}

	void movebackward(){
		if (zoomstage == 1) {
			mappos.y = 0;
		}
	}
}
