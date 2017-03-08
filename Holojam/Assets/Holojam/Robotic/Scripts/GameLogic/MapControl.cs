//MapControl.cs
//Created by Aaron C Gaudette on 16.09.16

using UnityEngine;
using System.Collections.Generic;

public class MapControl : MonoBehaviour{
   public Transform table;
   public float tableSize = 0.2f;

   public Transform buildingRoot;
   public NearestSelector[] hands;

   public Transform[] buildings;
   List<Transform> activeCache = new List<Transform>();
   Vector3 testPosition;

   void Awake(){
      buildings = new Transform[buildingRoot.childCount];
      int i = 0;
      foreach(Transform child in buildingRoot){
         buildings[i]=child;
         buildings[i++].GetComponent<Holojam.Tools.Synchronizable>().Label = "Building-"+(i-1);
      }
   }

   void Update(){
      activeCache.Clear();

      foreach(Transform b in buildings){
         testPosition = table.InverseTransformPoint(b.position);

         bool oob = testPosition.x > tableSize || testPosition.x < -tableSize
            || testPosition.z > tableSize || testPosition.z < -tableSize;

         oob = oob && b.GetComponent<GestureTarget>().controlled == false;

         b.GetChild(0).GetComponent<Renderer>().enabled = !oob;
         if(!oob)activeCache.Add(b);
      }

      //
      foreach(NearestSelector ns in hands){
         ns.contextTargets = activeCache;
      }
   }
}