using UnityEngine;
using System.Collections;

public class myCupCtrl : MonoBehaviour
{

	public GameObject trackedObj;

	public Vector3 remoteOffset;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = trackedObj.transform.position + remoteOffset;


	}
}
