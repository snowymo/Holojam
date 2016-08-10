using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ctrlC : MonoBehaviour
{

	public GameObject carA1;
	public GameObject carA2;
	public GameObject carB1;
	public GameObject carB2;

	private Vector2 vLocal;
	private Vector2 vRemote;

	private bool isLastRound,isLastStraight;

	private m3piComm m3piCtrl;

	// Use this for initialization
	void Start ()
	{
		m3piCtrl = new m3piComm ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// update them with offset
		//updateOffset ();
		// synchoronize A1 with B1 and sync A2 with B2 all the time
		//sync (carB1, carA1);
		sync (carB2, carA2);

	}

	void updateOffset ()
	{
		carA2.transform.position += Offset.getInst ().getOffset ();
		carB1.transform.position -= Offset.getInst ().getOffset ();
	}

	bool isClose (Vector3 pos1, Vector3 pos2)
	{
		return Utility.getInst ().checkMatchV2 (pos1, pos2);
	}

	void drawRays (Transform localTrans, Transform remoteTrans)
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

	bool turnAround(GameObject local, GameObject remote, ref Vector3 lastPosition, ref Quaternion lastRotation){
		// check if turn around last time and the distance is positive
		if(isLastRound
			&& (Quaternion.Angle(transform.rotation,lastRotation) < 1))
			return false;
		else{
			Quaternion facing = Quaternion.identity;
			facing.SetFromToRotation (local.transform.rotation * Vector3.forward, remote.transform.position - local.transform.position);
			Vector3 vFacing = remote.transform.position-local.transform.position;
			Vector3 vCur = local.transform.rotation * Vector3.forward;
			float angle = Vector3.Angle(vCur, vFacing);
			Vector3 vUp = Vector3.Cross (vCur, vFacing);

			print ("turnRound:\tvCur:\t" + vCur.ToString ("F2") + "\tvFacing:\t" + vFacing.ToString ("F2"));
			print ("turnRound:\tangle:\t" + angle);

			if(angle > 90.0f)
				angle = angle - 180.0f;
			if (angle % 180.0f > 6.0f) {
				//print("turnRound:\tupVector:\t" + vUp.ToString("F2"));
				if (vUp.y > 0.005)
					m3piCtrl.right ();
				else if (vUp.y < -0.005)
					m3piCtrl.left ();
				else
					return true;
				isLastRound = true;
				return false;
			} else {
				isLastRound = false;
				return true;
			}
		}
	}

	bool goStraight(GameObject local, GameObject remote, ref Vector3 lastPosition ){
		// check if turn around last time and the distance is positive
		if (isLastStraight && (Vector3.Distance (transform.position, lastPosition) < 0.0001f)) {
			isLastStraight = false;
			return false;
		}
		Vector3 dis = remote.transform.position - local.transform.position;
		print ("goStraight\tdis:\t" + dis.ToString("F3") + "\tref:\t" + 
			remote.transform.position.ToString("F3") + "\tcur:\t" + local.transform.position.ToString("F3"));
		if (dis.magnitude > Utility.getInst ().disError) {
			Vector3 vCur = local.transform.rotation * Vector3.forward;
			Vector3 vUp = Vector3.Cross (dis, vCur);
			print ("goStraight\tvCur:\t" + vCur.ToString ("F2") + "\tvUp:\t" + vUp.ToString ("F2"));
			if ((vCur.x * dis.x >= 0) || (vCur.z * dis.z >= 0))
				m3piCtrl.forward ();
			else
				m3piCtrl.backward ();
			isLastStraight = true;
			return false;
		} else {
			isLastStraight = false;
			return true;
		}
	}

	void sync (GameObject local, GameObject remote)
	{
		vLocal = transform.rotation * Vector3.forward;
		// TODO: check if tracked
		drawRays (local.transform, remote.transform);
		// initialize
		int step = 1;
		isLastRound = false;
		isLastStraight = false;
		Vector3 lastPos = new Vector3();
		Quaternion lastRot = Quaternion.identity;

		while (step != 0) {
			switch (step) {
			case 0:
				break;
			case 1:
			case 3:
				// check distance first
				if (isClose (local.transform.position, remote.transform.position)) {
					step = 0;
				} else {
					++step;
				}
				break;
			case 2:
				// moved car with turning
				if (turnAround (local, remote,ref lastPos,ref lastRot))
					step = 3;
				break;
			case 4:
				// moved car with going straight
				if (goStraight (local, remote,ref lastPos)) {
					step = 0;
				}
				break;
			default:
				break;
			}
			lastPos = local.transform.position;
			lastRot = local.transform.rotation;
		}
	}
}
