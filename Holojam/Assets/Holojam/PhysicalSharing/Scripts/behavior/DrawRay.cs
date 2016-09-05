using UnityEngine;
using System.Collections;

public class DrawRay : MonoBehaviour
{

	LineRenderer lineRenderer;
	float counter;
	float dist;
	wandCtrl wCtrl;

	public Transform origin;
	//public Transform destination;
	public float startWidth;
	public float endWidth;
	public float lineDrawSpeed = 6f;
	float distance;
	bool startDrawdot;

	// Use this for initialization
	void Start ()
	{
		lineRenderer = GetComponent<LineRenderer> ();

		wCtrl = GetComponentInParent<wandCtrl> ();

		lineRenderer.SetWidth (startWidth, endWidth);

		startDrawdot = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!startDrawdot && checkHolding ())
			startDrawdot = true;

		if(startDrawdot)
			drawDotRay ();

	}

	bool checkHolding ()
	{
		if (Vector3.Angle (origin.rotation * Vector3.forward, Vector3.up) < 5f)
			return true;
		else
			return false;
	}

	void drawDotRay ()
	{
		// draw when the wand is holding TODO
		lineRenderer.SetPosition (0, origin.transform.position);
		// if hit then just draw until that obj
		if (wCtrl.isCollide) {
			lineRenderer.SetPosition (1, wCtrl.objChosen.transform.position);
			distance = Vector3.Distance (origin.transform.position, wCtrl.objChosen.transform.position);
		}
		// if not hit, then just draw to infinite
		else {
			lineRenderer.SetPosition (1, transform.position +
			transform.transform.rotation * Vector3.forward * 10f);
			distance = 10f;
		}

		lineRenderer.materials [0].mainTextureScale = new Vector3 (distance, 1, 1);
	}
}
