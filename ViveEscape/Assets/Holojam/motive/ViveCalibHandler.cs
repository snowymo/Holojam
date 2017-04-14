using System;
using System.Collections;
using System.Collections.Generic;
using FRL.IO;
using UnityEngine;

public class ViveCalibHandler : MonoBehaviour, IGlobalTriggerPressDownHandler, IGlobalTriggerPressHandler, IGlobalTriggerPressUpHandler, IGlobalGripPressDownHandler, IGlobalGripPressUpHandler
{

  //public bool bPressDown;

  [SerializeField]
  private Vector3[] calibPoints, controlPoints;
  private int calibCount;


  public Vector3 transOffset,manualOffset;
  public float angleOffset;

  /// <summary>
  /// A transform representing the tracking bounds, e.g. the CameraRig prefab.
  /// </summary>
  public Transform centroid;

  public Transform referController;

  public motiveSync optiObj;

  private Vector3 cachedPosition = Vector3.zero;
  private Quaternion cachedRotation = Quaternion.identity;

  public void OnGlobalTriggerPress(BaseEventData eventData)
  {
    // do the calibration
    //bPressDown = true;
    //print("right trigger down");
  }

  public void OnGlobalTriggerPressDown(BaseEventData eventData)
  {
    // do the calibration
    //bPressDown = true;
    //print("right trigger down");
  }

  public void OnGlobalTriggerPressUp(BaseEventData eventData)
  {
    //bPressDown = false;
    //print("right trigger up");
  }


  // Use this for initialization
  void Start()
  {
    //bPressDown = false;
    if (centroid)
      cachedPosition = centroid.position;
    calibPoints = new Vector3[2];
    controlPoints = new Vector3[2];
    calibCount = 0;
    manualOffset = new Vector3(0, -0.28f, 0.1f);
  }

  void addCalibPoint()
  {
    if (calibCount >= 2)
      calibCount = 0;
    if (optiObj.Tracked)
    {
      calibPoints[calibCount] = optiObj.gameObject.transform.position;
      controlPoints[calibCount] = referController.position;
      ++calibCount;
    }

  }

  // Update is called once per frame
  void Update()
  {
    //     if (bPressDown) {
    //       Calibrate(optiObj.position - referController.position);
    //     }
    if (calibCount == 2)
    {
      // translation offset
      transOffset = calibPoints[0] - controlPoints[0] + manualOffset;
      print("offset:" + transOffset.x + "," + transOffset.y + "," + transOffset.z );

      // check scale first for future usage
      float optiDis = (calibPoints[1] - calibPoints[0]).magnitude;
      float controlDis = (controlPoints[1] - controlPoints[0]).magnitude;
      float scale = controlDis / optiDis;
      // it changed and around 1.00
      print("motive dis:" + optiDis + "\tcontrol dis:" + controlDis + "\tscale:" + scale);
      // rotation
      Vector3 axis1 = calibPoints[1] - calibPoints[0], axis2 = controlPoints[1] - controlPoints[0];
      axis1.y = 0;
      axis2.y = 0;
      angleOffset = Vector3.Angle(axis1, axis2);
      //Calibrate(angleOffset);

      print("rotation:" + angleOffset);
      Calibrate(transOffset);
      // for later motive position, we would do (p+t)*scale then turn angle through y axis
      // ready for next calibration
      calibCount = 0;
    }
  }

  /// <summary>
  /// Offset the centroid by its difference to the absolute center.
  /// </summary>
  void Calibrate(Vector3 center)
  {
    centroid.position = cachedPosition + new Vector3(center.x, center.y + 0.25f, center.z);
    cachedPosition = centroid.position;
  }

  /// <summary>
  /// Offset the centroid by its difference to the absolute center.
  /// </summary>
  void Calibrate(float angle)
  {
    centroid.rotation = cachedRotation * Quaternion.Euler(0, -angle, 0);
    cachedRotation = centroid.rotation;
  }

  public void OnGlobalGripPressDown(BaseEventData eventData)
  {
    //bPressDown = true;
    addCalibPoint();
    print("trigger down");
  }

  public void OnGlobalGripPressUp(BaseEventData eventData)
  {
    //bPressDown = false;
    print("trigger up");
  }
}
