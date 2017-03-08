//Viewer.cs
//Created by Aaron C Gaudette on 07.07.16

using UnityEngine;

namespace Holojam.Tools{
   [ExecuteInEditMode]
   public class Viewer : MonoBehaviour{
      public enum TrackingType{DIRECT,ACTOR};
      public TrackingType trackingType = TrackingType.ACTOR;

      public Converter converter;

      //Get tracking data from actor (recommended coupling), or from the view?
      public Actor actor = null;
      [HideInInspector] public Network.View view = null;
      public int index = 0;
      public bool localSpace = false;


		//OPTITRACK
		const float correctionThreshold = 0.98f;
		//Lower values allow greater deviation without correction
		Quaternion correction = Quaternion.identity;

		const float differenceThreshold = 0.9995f;
		//Lower values allow correction at greater angular speeds
		float difference = 1;

		const float timestep = 0.01f;
		float lastTime = 0;
		Quaternion lastRotation = Quaternion.identity;
		//OPTITRACK



      //Update late to catch local space updates
      void LateUpdate(){
         if(converter==null){
            Debug.LogWarning("Viewer: Converter is null!");
            return;
         }

         if(BuildManager.DEVICE==BuildManager.Device.VIVE)
            return;

         //Flush extra components if necessary
         Network.View[] views = GetComponents<Network.View>();
         if((view==null && views.Length>0) || (view!=null && (views.Length>1 || views.Length==0))){
            foreach(Network.View v in views)DestroyImmediate(v);
            view=null; //In case the view has been set to a prefab value
         }

         //Automatically add a View component if not using a reference actor
         if(actor==view){
            view = gameObject.AddComponent<Network.View>() as Network.View;
            view.triples = new Vector3[1];
            view.quads = new Quaternion[1];
         }
         else if(actor!=null && view!=null)DestroyImmediate(view);

         if(view!=null){
            view.label = Network.Canon.IndexToLabel(index);
            view.scope = Network.Client.SEND_SCOPE;
         }
         if(!Application.isPlaying)return;

         Vector3 sourcePosition = GetPosition();
         Quaternion sourceRotation = GetRotation();
         bool sourceTracked = GetTracked();
			//OPTITRACK
			//Don't use Camera.main (reference to Oculus' instantiated camera at runtime)
			//in the editor or standalone, reference the child camera instead
			if (BuildManager.DEVICE == BuildManager.Device.GEARVR) {
				Vector3 cameraPosition = BuildManager.IsMasterClient () ?
					GetComponentInChildren<Camera> ().transform.position : Camera.main.transform.position;
				//Ne gate Oculus' automatic head offset (variable reliant on orientation) independent of recenters
				transform.position += sourcePosition - cameraPosition;
			}
			//OPTITRACK
			if (sourceTracked) {
				//TrackingType.DIRECT:
				//Direct raw conversion from Converter (no additional transformation)

				//TrackingType.ACTOR:
				//Loops once through the network (Converter -> Server -> Actor -> Viewer)

				//OPTITRACK
				if (BuildManager.DEVICE == BuildManager.Device.GEARVR) {
					Quaternion imu = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye);
					Quaternion optical = sourceRotation*Quaternion.Inverse(imu);

					//Calculate rotation difference since last timestep
					if(Time.time>lastTime+timestep){
						difference=Quaternion.Dot(imu,lastRotation);
						lastRotation=imu; lastTime=Time.time;
					}

					//Ignore local space rotation in the IMU calculations
					Quaternion localRotation = transform.rotation;
					if(actor!=null && actor.localSpace && actor.transform.parent!=null)
						localRotation=Quaternion.Inverse(actor.transform.parent.rotation)*transform.rotation;
					else if(actor==null && localSpace && transform.parent!=null)
						localRotation=Quaternion.Inverse(transform.parent.rotation)*transform.rotation;

					//Recalculate IMU correction if stale (generally on init/recenter)
					if(Quaternion.Dot(localRotation*imu,sourceRotation)<=correctionThreshold
						&& difference>=differenceThreshold) //But not if the headset is moving quickly
						correction=optical;

					if (BuildManager.IsMasterClient ())
						transform.rotation = optical;
					else
						transform.rotation=correction;

				} else {

					transform.position = sourcePosition;
					if (BuildManager.IsMasterClient ())
						transform.rotation = sourceRotation;
					else {
						//Negate IMU
						Quaternion raw = Quaternion.identity;
						switch (BuildManager.DEVICE) {
						case BuildManager.Device.CARDBOARD:
							raw = transform.GetChild (0).localRotation;
							break;
						case BuildManager.Device.DAYDREAM:
							raw = UnityEngine.VR.InputTracking.GetLocalRotation (
								UnityEngine.VR.VRNode.CenterEye
							);
							break;
						}
						sourceRotation *= Quaternion.Inverse (raw);
						transform.rotation = sourceRotation;
					}
				}
			} else if (BuildManager.IsMasterClient () || BuildManager.DEVICE == BuildManager.Device.GEARVR) //Fall back to IMU
				transform.rotation = sourceRotation;

			//Apply local rotation if necessary
			if (actor != null && actor.localSpace && actor.transform.parent != null)
				transform.rotation = actor.transform.parent.rotation * transform.rotation;
			else if (actor == null && localSpace && transform.parent != null)
				transform.rotation = transform.parent.rotation * transform.rotation;
		}

		//Get tracking data from desired source
		Vector3 GetPosition ()
		{
			if (BuildManager.DEVICE==BuildManager.Device.GEARVR)
				return actor!=null? actor.center:
					localSpace && transform.parent!=null?
					transform.parent.TransformPoint(view.triples[0]) : view.triples[0];
			if (trackingType == TrackingType.DIRECT)
				return converter.outputPosition;
			else {
				return actor != null ? actor.center :
					localSpace && transform.parent != null ?
					transform.parent.TransformPoint (view.triples [0]) : view.triples [0];
			}
		}

		Quaternion GetRotation ()
		{
			if (BuildManager.DEVICE==BuildManager.Device.GEARVR)
				return actor!=null?actor.rawOrientation:view.quads[0];
			if (trackingType == TrackingType.DIRECT)
				return converter.outputRotation;
			else
				return actor != null ? actor.rawOrientation : view.quads [0];
		}

		bool GetTracked ()
		{
			if (BuildManager.DEVICE==BuildManager.Device.GEARVR)
				return actor!=null?actor.view.tracked:view.tracked;
			if (trackingType == TrackingType.DIRECT)
				return converter.hasInput;
			else
				return actor != null ? actor.view.tracked : view.tracked;
		}
	}
}
