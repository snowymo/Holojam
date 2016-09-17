//GestureTarget.cs
//Created by Aaron C Gaudette on 11.09.16

//modified on Zhenyi's computer

using UnityEngine;

public class GestureTarget : Holojam.Tools.Synchronizable
{
	public bool locked = false;
	public bool highlight = false;
	public bool loading = false;
	public bool controllable = false, controlled = false;

	public float tableOffset = 0.001f;

	public Transform robot, table; //

	public GameObject glow;
	public TextMesh buffering;

	protected override void Sync ()
	{
		if (sending) {
			if (controllable) {
				float h = robot.position.y - table.position.y;
				//print (h); //
				controlled = h > tableOffset;
			} else
				controlled = false;
			if (controllable){
				transform.position = robot.position;
				transform.rotation = robot.rotation;
			}

			synchronizedVector3 = transform.position;
			synchronizedQuaternion = transform.rotation;
			synchronizedString = highlight ? "True" : "False";
			synchronizedInt = loading ? 1 : 0;
		} else {
			transform.position = synchronizedVector3;
			transform.rotation = synchronizedQuaternion;
			highlight = synchronizedString == "True";
			loading = synchronizedInt == 1;
			locked = false;
		}
		if(glow!=null)glow.SetActive (highlight);

		if(buffering==null)return;

		buffering.gameObject.SetActive (loading);
		if (loading) {
			float div = Time.time % 3;
			buffering.text = div < 1 ? ".  " : div >= 2 ? "..." : ".. ";
		}
	}
}