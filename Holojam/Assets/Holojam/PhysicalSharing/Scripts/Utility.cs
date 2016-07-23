using UnityEngine;
using System.Collections;

public class Utility {

	static Utility m_instance;

	public float disError = 0.035f;

	public float rotateError = 0.02f;

	public float yError = 0.01f;

	public static Utility getInst (){
		if(m_instance == null)
			m_instance = new Utility();
		return m_instance;
	}

	public Utility(){
	}

	// do not care about the rotation
	public bool checkMatchV2(Vector3 pos, Vector3 refPos){
		Vector3 thisXOZ = new Vector3(pos.x,0,pos.z);
		Vector3 carXOZ = new Vector3(refPos.x,0,refPos.z);
		//Debug.Log("checkMatchV2:\tdis:" + (thisXOZ-carXOZ).magnitude);

		//Debug.Log("checkMatchV2:\ty:" + (pos.y-refPos.y));
		// 
		if ((thisXOZ-carXOZ).magnitude < disError && ((pos-refPos).y < yError))
			return true;
		else
			return false;
	}

	public bool checkMatchStart(Transform tfm, Transform refTfm){
		// check if tracked ball is very close to start ball

		//print((trackedBall.transform.position));

		// temp version
		Vector3 temp1 = new Vector3(tfm.position.x,0,tfm.position.z);
		Vector3 temp2 = new Vector3(refTfm.position.x,0,refTfm.position.z);
		Debug.Log((temp1-temp2).magnitude);
		//		print ("euler angle:\t" + Quaternion.Angle (transform.rotation, trackedBall.transform.rotation));//8
		float matching = Quaternion.Dot(tfm.rotation, refTfm.rotation);
		Debug.Log ("checkMatchStart:\tQuaternion dot:\t" + (Mathf.Abs(Mathf.Abs(matching)-1.0f)));//
		Debug.Log("checkMatchStart:\t" + (tfm.position.y-refTfm.position.y));
		// 
		if ((temp1-temp2).magnitude < disError && ((tfm.position.y-refTfm.position.y) < yError)
			&& (Mathf.Abs(Mathf.Abs(matching)-1.0f) < rotateError))
			return true;
		else
			return false;
	}
//
//	void Start(){
//	}
//
//	void Update(){
//		
//	}
}
