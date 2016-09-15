//ForceGesture.cs
//Created by Aaron C Gaudette on 11.09.16

//sprint code

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Holojam.Tools.Pairable))]
public class ForceGesture : MonoBehaviour{
   public float range = 8;
   public float speed = 2;
   public float shakeAmount = 2;
   public float shakeForce = 1;
   public float recordTime = 1.5f;
   public Vector2 magnitudeRange = new Vector2(0.12f,0.45f);
   public float scale = 4;
   public float verticalOffset = 0.1f;

   public Transform table;

   [Space(8)]
   public float tableScale = 4;
   public Vector3 tablePosition = new Vector3(0,0.5f,0);
   public Vector3[] tableCorners = new Vector3[4];

   [Space(8)]
   public GestureTarget target;
   [Range(0,1)] public float weight = 0;
   Vector3 initialPoint;
   public float mark;
   public bool recording = false;
   public Vector3 startPosition, endPosition;

   Holojam.Tools.Pairable p;
   void Awake(){
      if(!Holojam.Utility.IsMasterPC())Destroy(this);
      p = GetComponent<Holojam.Tools.Pairable>();
   }

   RaycastHit hit;
   void Update(){
      tablePosition = table.transform.position;
      //
      tableCorners[0] = tablePosition-Vector3.right*tableScale+Vector3.forward*tableScale; //TL
      tableCorners[1] = tablePosition+Vector3.right*tableScale+Vector3.forward*tableScale; //TR
      tableCorners[2] = tablePosition+Vector3.right*tableScale-Vector3.forward*tableScale; //LR
      tableCorners[3] = tablePosition-Vector3.right*tableScale-Vector3.forward*tableScale; //LL
      for(int i=0;i<4;)
         Debug.DrawLine(tableCorners[i++],tableCorners[i%4],Color.red);
      //
      Debug.DrawRay(p.center,-transform.up*range,Color.yellow);

      if(!recording && Physics.Raycast(p.center,-transform.up,out hit,range)){
         //print("hit");
         GestureTarget gt = hit.transform.GetComponent<GestureTarget>();
         if(gt!=null && !gt.locked){
            //print("hit target");
            if(target!=null)ResetTarget();
            target = gt;
            gt.locked = true;
            initialPoint = target.transform.position;
         }
      } else if(!recording && target!=null)ResetTarget();

      if(target!=null || recording){
         weight+=speed*Time.deltaTime;
         if(weight<1){
            target.transform.position = Vector3.Lerp(target.transform.position,
               initialPoint+Random.insideUnitSphere*shakeAmount,
               shakeForce*weight*Time.deltaTime
            );
         } else if(!recording){
            target.transform.position = initialPoint;
            //print("selected");
            mark = Time.time;
            startPosition = p.center;
            recording = true;
            target.highlight = true;
         } else if(Time.time>mark+recordTime){
            endPosition = p.center;
            ProcessGesture();
            recording = false;
            print("finished recording");
         }
      }
   }

   void ProcessGesture(){
      Transform targetRef = target.transform;
      ResetTarget(); //clean

      Debug.DrawRay(startPosition,Vector3.up,Color.blue,4);
      Debug.DrawRay(endPosition,Vector3.up,Color.blue,4);

      Vector3 direction = endPosition-startPosition;
      direction = new Vector3(direction.x,0,direction.z);
      if(direction.magnitude<magnitudeRange.x){
         Debug.Log("No gesture detected.");
         return;
      }
      float strength = direction.magnitude/(magnitudeRange.y-magnitudeRange.x) + magnitudeRange.x; //broken
      strength = Mathf.Min(strength,1)*scale;
      //print(direction.magnitude); //

      direction = Vector3.Normalize(direction)*strength;
      //
      Debug.DrawRay(initialPoint,direction,Color.magenta,4);

      //clean
      Vector3 exitPoint = initialPoint+direction;

      float dz0 = -direction.z, dx0 = direction.x;
      float line0 = initialPoint.x*(dz0) + initialPoint.z*(dx0); //!
      Vector3 corner, segment;
      float line1, dz1, dx1, delta;

      Vector3 tmp, moveTo = Vector3.zero;
      bool found = false; //clean

      //clean up
      float minx, maxx, minz, maxz;
      if(initialPoint.x<exitPoint.x){
         minx = initialPoint.x;
         maxx = exitPoint.x;
      } else {
         minx = exitPoint.x;
         maxx = initialPoint.x;
      }
      if(initialPoint.z<exitPoint.z){
         minz = initialPoint.z;
         maxz = exitPoint.z;
      } else {
         minz = exitPoint.z;
         maxz = initialPoint.z;
      }

      for(int i=0;i<4;){
         corner = tableCorners[i++];
         segment = tableCorners[i%4]-corner;
         dz1 = segment.z; dx1 = segment.x;
         line1 = corner.x*(dz1) + corner.z*(dx1);

         Vector3 exitSegment = corner+segment;

         delta = dz0*dx1 - dz1*dx0;
         if(delta==0)print("parallel");
         else{
            tmp = new Vector3(
               (dx1*line0 - dx0*line1)/delta,0,
               (dz0*line1 - dz1*line0)/delta
            );
            Debug.DrawRay(tmp,Vector3.up,Color.magenta,4);

            if(Vector3.Dot((tmp-initialPoint).normalized,direction)<0){
               print("out of range");
               continue;
            }

            //clean up
            float minx1, maxx1, minz1, maxz1;
            if(corner.x<exitSegment.x){
               minx1 = corner.x;
               maxx1 = exitSegment.x;
            } else {
               minx1 = exitSegment.x;
               maxx1 = corner.x;
            }
            if(corner.z<exitSegment.z){
               minz1 = corner.z;
               maxz1 = exitSegment.z;
            } else {
               minz1 = exitSegment.z;
               maxz1 = corner.z;
            }

            //print("x: " + tmp.x + " vs " + minx + ", " + maxx);
            //print("z: " + tmp.z + " vs " + minz + ", " + maxz);
            //print("x1: " + tmp.x + " vs " + minx1 + ", " + maxx1);
            //print("z1: " + tmp.z + " vs " + minz1 + ", " + maxz1);

            if(Mathf.Clamp(tmp.x,minx,maxx)==tmp.x && Mathf.Clamp(tmp.z,minz,maxz)==tmp.z
               && Mathf.Clamp(tmp.x,minx1,maxx1)==tmp.x && Mathf.Clamp(tmp.z,minz1,maxz1)==tmp.z){

               //print("x: " + tmp.x + " vs " + minx + ", " + maxx);
               //print("z: " + tmp.z + " vs " + minz + ", " + maxz);
               //print("x1: " + tmp.x + " vs " + minx1 + ", " + maxx1);
               //print("z1: " + tmp.z + " vs " + minz1 + ", " + maxz1);

               print("bounded");
               moveTo = tmp;
            }
            else continue;
            moveTo.y = tablePosition.y; //offset
            print("found");
            found = true;
         }
      }
      if(!found){
         print("unclipped");
         moveTo = initialPoint+direction;
      }
      Debug.DrawRay(moveTo,Vector3.up,Color.green,4);

      //loading
      //lerp

      targetRef.position = moveTo+Vector3.up*verticalOffset;
   }

   void ResetTarget(){
      target.locked = false;
      target.highlight = false;
      target = null;
      weight = 0;
   }
}