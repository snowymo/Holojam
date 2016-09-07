using UnityEngine;
using System.Collections;
using Holojam.Network;
using Holojam.IO;

public class wandCtrl : MonoBehaviour {

	public GameObject _hand;

	public magicObjCtrl objChosen;

	public GameObject _objs;

	public bool isCollide;

	// Use this for initialization
	void Start () {
		isCollide = false;
	}
	
	// Update is called once per frame
	void Update () {

		// check if selected, and highlight it
		isCollide = isCollision ();
		if (isCollide) {
			objChosen.highlight ();
		} else {
			uncheckAll ();
		}

		// draw the ray of wand, TODO maybe activate when button B is clicked
		drawRays ();

		// for laptop
		if (Input.GetKeyDown (KeyCode.Space)) {
			print ("click space");
			if (isCollide) {
				unlinkAll ();
				objChosen.moveWanimation ();
			} 
		}

		// for phones
		int bits = GetComponent<HolojamView>().Bits;

		if((bits & ButtonConstants.A)>0){
			OnButtonA();
		} 
	
	}

	public bool isCollision(){
		Vector3 fwd = //transform.position - _hand.transform.position;
			transform.transform.rotation * Vector3.forward;
		RaycastHit hitObj;
		if (Physics.Raycast (transform.position, fwd, out hitObj)) {
			objChosen = hitObj.collider.GetComponentInParent<magicObjCtrl> ();
			if(objChosen != null)
				return true;
		}
		return false;
	}

	public void OnButtonA(){
		//print ("click button A");
		if (isCollide) {
			unlinkAll ();
			objChosen.moveWanimation ();
		}
	}

	protected void drawRays(){
		Vector3 fwd = transform.position - _hand.transform.position;
		Debug.DrawRay (transform.position, fwd, Color.white);
		Debug.DrawRay (transform.position, transform.transform.rotation * Vector3.forward, Color.magenta);
	}

	protected void uncheckAll(){
		magicObjCtrl[] mos = _objs.GetComponentsInChildren <magicObjCtrl> ();
		foreach (magicObjCtrl mo in mos){
			mo.hideWings ();
		}
	}

	protected void unlinkAll(){
		magicObjCtrl[] mos = _objs.GetComponentsInChildren <magicObjCtrl> ();
		foreach (magicObjCtrl mo in mos){
			mo.SetLink (false);
		}
	}
}
