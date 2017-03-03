//GestureTarget.cs
//Created by Aaron C Gaudette on 11.09.16

//modified on Zhenyi's computer

using UnityEngine;

public class GestureTarget : Holojam.Tools.Synchronizable
{
	public override int tripleCount{get{return 1;}}
	public override int quadCount{get{return 1;}}
	public override bool hasText{get{return true;}}
	public override int intCount{get{return 1;}}

	public override string labelField { get { return label; } }
	public override string scopeField { get { return scope; } }

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

			view.triples[0] = transform.position;
			view.quads[0] = transform.rotation;
			view.text = highlight ? "True" : "False";
			view.ints[0] = loading ? 1 : 0;
		} else {
			transform.position = view.triples[0];
			transform.rotation = view.quads[0];
			highlight = view.text == "True";
			loading = view.ints[0] == 1;
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