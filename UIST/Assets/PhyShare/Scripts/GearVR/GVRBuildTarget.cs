using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Tools;

[ExecuteInEditMode]
public class GVRBuildTarget : MonoBehaviour
{

    Dictionary<int, string> targets = new Dictionary<int, string>() {
    {0 , "NONE" },
    {1 , "P1HMD" },
    {2 , "P2HMD" },
  };

    public Transform P1HMD;
    public Transform P2HMD;

    private GVRViveHeadset headset;

    void Start()
    {
        headset = this.GetComponent<GVRViveHeadset>();
    }

    // Update is called once per frame
    void Update()
    {
        headset.label = targets[BuildManager.BUILD_INDEX];
        if (headset.label.Equals("P1HMD"))
        {
            headset.targetTransform = P1HMD;
        }
        else if (headset.label.Equals("P2HMD"))
        {
            headset.targetTransform = P2HMD;
        }
        else
        {
            headset.targetTransform = null;
        }
    }
}