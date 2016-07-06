﻿//ActorManager.cs
//Created by Aaron C Gaudette on 22.06.16

using UnityEngine;
using Holojam.Server;

namespace Holojam{
	[ExecuteInEditMode]
	public class ActorManager : MonoBehaviour{
		public TrackedHeadset viewer; //VR camera for target device
		public LiveObjectTag buildTag = LiveObjectTag.HEADSET1; //Target device
		public bool runtimeIndexing = false;
		
		[HideInInspector] public Actor[] actors = new Actor[4]; //Actor array reference
		int[] indexCache;
		LiveObjectTag cachedBuildTag;
		
		//Get the current build actor (re-index if necessary)
		[HideInInspector] public Actor ba;
		public Actor buildActor{get{
			if(ba==null && (!Application.isPlaying || runtimeIndexing))Index(true);
			return ba;
		}}
		
		void Start(){
			Index(true); //Force index because the TrackedObject class is not currently serializable
			//foreach(Actor a in actors)a.ApplyMotif();
			//Instantiate viewer
			if(Application.isPlaying){
				GameObject v = Instantiate(viewer.gameObject,Vector3.zero,Quaternion.identity) as GameObject;
				v.name="Viewer";
				v.GetComponent<TrackedHeadset>().liveObjectTag=buildTag;
			}
		}
		
		public void Update(){
			//Force index in case prefabs are updated (will increase logging!)
			if(!Application.isPlaying || runtimeIndexing)
				Index(Application.isEditor && !Application.isPlaying);
		}
		
		enum Result{INDEXED,PASSED,ERROR,NOVIEW};
		Result Index(bool force = false){
			if(actors.Length!=transform.childCount)actors=new Actor[transform.childCount];
			int[] indices = new int[transform.childCount];
			
			bool equal = indexCache!=null && indexCache.Length==indices.Length;
			//Build actor array and cache
			for(int i=0;i<transform.childCount;++i){
				if(actors[i]==null)actors[i]=transform.GetChild(i).GetComponent<Actor>();
				indices[i]=actors[i].index;
				equal=equal && indices[i]==indexCache[i];
			}
			//If tags differ from last check, perform index
			if(equal && buildTag==cachedBuildTag && !force)return Result.PASSED;
			indexCache=indices;
			cachedBuildTag=buildTag;
			
			if(actors.Length==0){
				if(Application.isPlaying)Debug.LogWarning("ActorManager: No actors in hierarchy!");
				return Result.ERROR;
			}
			if(viewer==null){
				Debug.LogWarning("ActorManager: Viewer prefab reference is null");
				return Result.ERROR;
			}
			
			//Index each actor
			bool setBuild = false;
			foreach(Actor a in actors){
				a.transform.position=Vector3.zero;
				a.transform.rotation=Quaternion.identity;
				
				//Is this the build actor?
				bool isBuild = a.index==(int)buildTag;
				if(isBuild && setBuild){
					Debug.LogWarning("ActorManager: Duplicate build actor!");
					isBuild=false;
				} else if(isBuild)ba=a; //Assign reference
				a.gameObject.name=a.name+(isBuild?" (Build)":"");
				
				//Activate mask
				if(a.mask!=null)a.mask.SetActive(!isBuild);
				else Debug.Log("ActorManager: No mask found for Actor "+(a.index+1)); //No warning
				
				setBuild=setBuild || isBuild;
				
				//Color actors (geometry)
				a.ApplyMotif();
			}
			if(!setBuild){
				Debug.LogWarning("ActorManager: No actor found with matching build tag!");
				return Result.NOVIEW;
			}
			
			return Result.INDEXED;
		}
	}
}