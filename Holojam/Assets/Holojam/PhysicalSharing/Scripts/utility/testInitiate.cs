using UnityEngine;
using System.Collections;

public class testInitiate : MonoBehaviour {

	public GameObject refObj;

	GameObject _obj;

	// Use this for initialization
	void Start () {
		if (_obj == null)
			_obj = GameObject.Instantiate (refObj);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
