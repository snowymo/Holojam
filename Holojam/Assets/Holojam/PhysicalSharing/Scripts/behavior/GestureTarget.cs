//GestureTarget.cs
//Created by Aaron C Gaudette on 11.09.16

using UnityEngine;

public class GestureTarget : Holojam.Tools.Synchronizable{
   public bool locked = false;
   public bool highlight = false;

   public GameObject glow;

   protected override void Sync(){
      if(sending){
         synchronizedVector3 = transform.position;
         synchronizedQuaternion = transform.rotation;
         synchronizedString = highlight?"True":"False";
      }
      else{
         transform.position = synchronizedVector3;
         transform.rotation = synchronizedQuaternion;
         highlight = synchronizedString=="True";
         locked = false;
      }
      glow.SetActive(highlight);
   }
}