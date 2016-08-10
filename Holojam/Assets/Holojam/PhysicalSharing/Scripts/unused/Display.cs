using UnityEngine;
using System.Collections;

public class Display : MonoBehaviour {

	public GameObject refCar;

	public GameObject frame;

	// Use this for initialization
	void Start () {
		refCar.gameObject.SetActive(false);
		frame.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
//		if (trackedCar.GetComponent<TrackedCar> ().isReadyToMove) {
//			// show the local car
//			refCar.gameObject.SetActive(true);
//		}
	}
}
