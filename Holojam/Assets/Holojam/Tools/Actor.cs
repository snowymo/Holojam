﻿//Actor.cs
//Created by Aaron C Gaudette on 23.06.16
//Umbrella class for accessing player (headset user) data in a generic manner

using UnityEngine;
using Holojam.Network;

namespace Holojam{
	public class Actor : Trackable{
		public string handle = "Actor";
		public Color motif = Color.white; //Useful color identifier, optional for rendering
		void Reset(){trackingTag=Motive.Tag.HEADSET1;}
		public GameObject mask; //This object is disabled for build actors by the manager
		
		public int index{get{return (int)trackingTag;}}
		public bool managed{get{
			return transform.parent!=null && transform.parent.GetComponent<ActorManager>()!=null;
		}}
		public ActorManager manager{get{return managed?transform.parent.GetComponent<ActorManager>():null;}}
		
		//Override these in derived classes for custom unique implementation
		
		protected override void Update(){
			UpdateTracking(); //Call this or the actor won't be tracked
		}
		//Update tracking data (position, rotation) and manage the untracked state here
		protected override void UpdateTracking(){
			if(!Application.isPlaying)return; //Safety check
			
			if(view.IsTracked){
				transform.position=view.RawPosition;
				transform.rotation=view.RawRotation;
			}
		}
		//These accessors should always reference assigned data (e.g. transform.position), not source (raw) data
		public virtual Vector3 eyes{
			get{return transform.position;}
		}
		public virtual Quaternion orientation{
			//Be careful not to map rotation to anything other than the user's actual head movement
			//unless you absolutely know what you're doing. The Viewer (headset) uses a custom
			//tracking algorithm and relies on this accessor to provide absolute truth.
			get{return transform.rotation;}
		}
		
		//Useful derived accessors
		public Vector3 look{get{return orientation*Vector3.forward;}}
		public Vector3 up{get{return orientation*Vector3.up;}}
		public Vector3 left{get{return orientation*Vector3.left;}}
		
		//Useful (goggles) visualization for edge of GearVR headset
		void OnDrawGizmos(){
			Gizmos.color=motif;
			Vector3 offset = eyes+look*0.015f;
			Drawer.Circle(offset+left*0.035f,look,up,0.03f);
			Drawer.Circle(offset-left*0.035f,look,up,0.03f);
			//Reference forward vector
			Gizmos.DrawRay(offset,look);
		}
		void OnDrawGizmosSelected(){}
	}
}