//NearestSelector.cs
//Created by Aaron C Gaudette on 16.09.16

//sprint code

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Holojam.Tools.Trackable))]
public class NearestSelector : MonoBehaviour{
   public float range = 1;
   public TextMesh loadingBuffer;
   public float loadingOffset = 0.1f;
   //public Transform robot;
	public cityRbtCtrl robotCtrl;


   //Assign these from outside
   [Space(8)]
   public List<Transform> contextTargets = new List<Transform>();
   public bool canSelect = true;
   public bool robotSynced = true;

   //Debug
   [Space(8)]
   public GestureTarget target;

	TrackableComponent pairable;
   void Awake(){
		pairable = GetComponent<TrackableComponent>();
   }

	Vector3 center(){
		return pairable.transform.TransformPoint (Vector3.zero);
	}

   void Update(){
      loadingBuffer.text = "";

      if(contextTargets.Count==0 || !canSelect){
         ResetTarget();
         return;
      }
      if(target!=null && target.controlled)
         return;

      //Find closest target
      float minDist = -1;
      Transform swap = null;
      foreach(Transform t in contextTargets){
			float d = Vector3.Distance(t.position,center());
         if(d<range){
            if(d<minDist || minDist==-1){
               swap = t;
               minDist = d;
            }
         }
      }
      if(minDist>-1){
         ResetTarget();
         target = swap.GetComponent<GestureTarget>();
         target.locked = true;
         //Debug
			Debug.DrawLine(target.transform.position,center(),Color.yellow);
      } else ResetTarget();

      if(target==null)return;

      // here: move the robot to target.position
		robotCtrl.setDestination(target.transform.position);
		robotSynced = robotCtrl.hasArrived();

      //Process target
		target.robot = robotCtrl.getExecuteRbt();
      target.controllable = robotSynced;
      target.loading = !robotSynced;
      //
      if(target.loading){
         loadingBuffer.transform.position = target.transform.position + Vector3.up*loadingOffset;
         //copy
         float div = Time.time % 3;
         loadingBuffer.text = div < 1 ? ".  " : div >= 2 ? "..." : ".. ";
      }
   }

   void ResetTarget(){
      if(target==null)return;

      target.locked = false;
      target.loading = false;
      target.controllable = false;
      target = null;
   }
}