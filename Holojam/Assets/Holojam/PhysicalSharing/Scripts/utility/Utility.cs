using UnityEngine;
using System.Collections;

public class Utility {

	static Utility m_instance;

	//TODO 0.05 for normal using
	public float disError = 0.05f;

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
		if ((thisXOZ-carXOZ).magnitude < disError 
		//	&& ((pos-refPos).y < yError)
		)
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
		if ((temp1-temp2).magnitude < disError 
			//&&((tfm.position.y-refTfm.position.y) < yError)
		
			&& (Mathf.Abs(Mathf.Abs(matching)-1.0f) < rotateError))
			return true;
		else
			return false;
	}

	public void drawRays (Transform localTrans, Transform remoteTrans)
	{
		Quaternion facing = Quaternion.identity;
		facing.SetFromToRotation (localTrans.rotation * Vector3.forward, remoteTrans.position - localTrans.position);
		//Vector3 vFacing = facing * Vector3.forward;

		Vector3 vCur = localTrans.rotation * Vector3.forward;

		// test if these two vectors are correct
		Debug.DrawRay (localTrans.position, vCur, Color.green);

		Debug.DrawRay (localTrans.position, remoteTrans.position - localTrans.position, Color.magenta);

		//Debug.DrawRay(this.transform.position,vFacing,Color.red);

		Debug.DrawRay (localTrans.position, facing * new Vector3 (0, 0, -1), Color.cyan);
	}

	public bool checkRtnMsg(m3piComm ctrl){
		// check if there is return msg already
		if (!ctrl.m_bRtn) {
			float executeTime = Time.time - ctrl.m_runTime;
			Debug.Log ("exe:\t" + executeTime + "\test:\t" + ctrl.m_cmdTime);
			// check if it is already too long then return and sync up them again
			if(executeTime < (ctrl.m_cmdTime + 0.8f))
				return false;
			Debug.Log ("wait too long:\t" + executeTime);
			ctrl.m_exStop = true;
			return true;
		}

		if(ctrl.m_returnMsg.Length > 0)
			Debug.Log (ctrl.m_returnMsg);
		ctrl.m_returnMsg = "";
		if (ctrl.receiveThread != null) {
			Debug.Log ("abort in checkRtnMsg and return true" + ctrl.receiveThread.ThreadState);
			ctrl.receiveThread.Abort ();
		}
		return true;
	}

//
//	void Start(){
//	}
//
//	void Update(){
//		
//	}
}
