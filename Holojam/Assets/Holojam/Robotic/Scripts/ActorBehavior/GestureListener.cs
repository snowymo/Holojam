//GestureListener.cs
//Created by Aaron C Gaudette on 11.09.16

//modified on Zhenyi's computer

using UnityEngine;
using System.Collections;

//sprint code

[RequireComponent (typeof(Holojam.Tools.Trackable))]
public class GestureListener : MonoBehaviour
{
	public float range = 9, speed = 0.45f;
	public float shakeAmount = 0.05f, shakeForce = 7;
	public float recordTime = 0.9f;
	public float cooldown = 1;
	public float lerpTime = 0.8f;

	[Space (8)]
	public Vector2 magnitudeRange = new Vector2 (0.1f, 0.45f);
	public float resultScale = 1;

	[Space (8)]
	public Transform table;
	public float tableSize = 0.45f;

	[Space (8)]
	public bool robotSynced = true;
	//
	public GestureTarget target;
	public float weight, mark;
	public bool recording = false;
	Vector3 initialPoint, startPosition, endPosition;

	Holojam.Tools.Trackable p;

	public GameObject robotCtrl;

	void Awake ()
	{
//		if (!Holojam.Utility.IsMasterPC ())
//			Destroy (this);
		p = GetComponent<Holojam.Tools.Trackable> ();
	}

	RaycastHit hit;

	Vector3 center(){
		return p.transform.TransformPoint (Vector3.zero);
	}

	void Update ()
	{
		if (!recording && Physics.Raycast (transform.position, -transform.up, out hit)) {
			print ("hit " + hit.collider);
			GestureTarget gt = hit.transform.GetComponent<GestureTarget> ();
			if (gt != null && !gt.locked && !gt.controlled) {
				if (target != null)
					ResetTarget (target);
				target = gt;
				gt.controllable = false;
				gt.locked = true;
				initialPoint = target.transform.position;
			}
		} else if (!recording && target != null) {
			target.controllable = true;
			ResetTarget (target);
		}

		if (target != null || recording) {
			weight += speed * Time.deltaTime;
			if (weight < 1) {
				target.transform.position = Vector3.Lerp (target.transform.position,
					initialPoint + Random.insideUnitSphere * shakeAmount,
					shakeForce * weight * Time.deltaTime
				);
			} else if (!recording) {
				target.transform.position = initialPoint;
				mark = Time.time;
				startPosition = center();
				recording = true;
				target.highlight = true;
			} else if (Time.time > mark + recordTime) {
				endPosition = center();
				ProcessGesture ();
				recording = false;
			}
		}

		//Debug
		Debug.DrawRay (transform.position, -transform.up * range, Color.yellow);
	}

	void ProcessGesture ()
	{
		Transform targetRef = target.transform;
		ResetTarget (target, true);

		//Debug
		Debug.DrawRay (startPosition, Vector3.up, Color.blue, 4);
		Debug.DrawRay (endPosition, Vector3.up, Color.blue, 4);

		Vector3 direction = endPosition - startPosition;
		direction = new Vector3 (direction.x, 0, direction.z);
		if (direction.magnitude < magnitudeRange.x) {
			Debug.Log ("No gesture detected.");
			targetRef.GetComponent<GestureTarget> ().controllable = true; //uneeded
			return;
		}
		float strength = direction.magnitude / (magnitudeRange.y - magnitudeRange.x) + magnitudeRange.x; //broken
		strength = Mathf.Min (strength, 1) * resultScale;
		//print(direction.magnitude);

		direction = Vector3.Normalize (direction) * strength;

		//Debug
		Debug.DrawRay (initialPoint, direction, Color.magenta, 4);

		Vector3 exitPoint = initialPoint + direction;
		exitPoint = table.InverseTransformPoint (exitPoint);
		float x = exitPoint.x > tableSize ? tableSize : exitPoint.x < -tableSize ? -tableSize : exitPoint.x;
		float z = exitPoint.z > tableSize ? tableSize : exitPoint.z < -tableSize ? -tableSize : exitPoint.z;

		exitPoint = new Vector3 (x, exitPoint.y, z);
		exitPoint = table.TransformPoint (exitPoint);

		robotSynced = false;
		//tell robot to do something with exitPoint <-- position
		//make sure you set that to true when you're done
		robotCtrl.GetComponent<TelekinesisBeerCtrl>().setDestination(exitPoint);

		//targetRef.position = exitPoint;
		StartCoroutine (LerpPosition (targetRef, exitPoint));
	}

	IEnumerator LerpPosition (Transform reference, Vector3 exit)
	{
		Vector3 start = reference.position;
		float startTime = Time.time;
		while (reference.position != exit) {
			reference.position = Vector3.Lerp (start, exit, (Time.time - startTime) / lerpTime);
			yield return null;
		}
	}

	void ResetTarget (GestureTarget gt, bool cool = false)
	{
		StartCoroutine (Reset (gt, cool));
	}

	IEnumerator Reset (GestureTarget gt, bool cool = false)
	{
		target = null;
		gt.highlight = false;
		weight = 0;
		if (cool) {
			print ("cool started");
			yield return new WaitForSeconds (cooldown);
			print ("cool ended");
			//robot
			gt.loading = true;
			while (!robotSynced)
				yield return null;
			gt.controllable = true;
		}
		gt.loading = false;
		gt.locked = false;
		gt = null;
	}
}