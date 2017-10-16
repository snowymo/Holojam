using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using System;
using Holojam.Tools;

public class HolojamConfig : Synchronizable
{

    private Dictionary<string, Vector3> loadedConfig = new Dictionary<string, Vector3>() {
    {"HMDPos", Vector3.zero },
    {"HMDRot", Vector3.zero },
  };

    public string configFilePath = "./HolojamConfig.xml";

    public override bool Host
    {
        get { return AutoHost; }
    }

    public override bool AutoHost
    {
        get { return BuildManager.IsMasterClient(); }
    }

    public override string Label
    {
        get { return "HConfig"; }
    }

    public bool TryGetValue(string name, out Vector3 value)
    {
        value = Vector3.zero;
        if (!loadedConfig.ContainsKey(name)) return false;
        value = loadedConfig[name];
        return true;
    }

    protected void Start()
    {
        if (Sending)
        {
            LoadConfiguration();
        }
    }

    public void CallLoadConfiguration()
    {
        LoadConfiguration();
    }

    // Returns a queue of messages indicating the result of the load attempt
    void LoadConfiguration()
    {
        if (Application.isEditor)
        {
            Debug.Log("Using sample config");
            configFilePath = "./Assets/PhyShare/Scenes/HolojamConfig.xml";
        }
        Debug.Log(configFilePath);
        XmlDocument configFile = new XmlDocument();
        FileInfo info = new FileInfo(configFilePath);

        // Load file
        try
        {
            configFile.Load(@configFilePath);
            Debug.Log(
              "Configuration: Load successful at " + info.FullName
            );
        }
        catch (FileNotFoundException)
        {
            Debug.Log("File at " + info.FullName + " not found!");
            return;
        }
        catch (DirectoryNotFoundException)
        {
            Debug.Log("Directory " + info.FullName + " is invalid!");
            return;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error during load attempt: " + ex.Message);
            return;
        }

        // Read nodes
        foreach (XmlNode node in configFile.DocumentElement.ChildNodes)
        {
            if (node.NodeType == XmlNodeType.Comment)
                continue;
            string key = node.Name;
            string value = GetText(node);

            string[] arr = value.Split(',');

            try
            {
                Vector3 vValue = new Vector3(
                  float.Parse(arr[0]),
                  float.Parse(arr[1]),
                  float.Parse(arr[2]));
                loadedConfig[key] = vValue;
            }
            catch (Exception e)
            {
                Debug.Log("Failed to parse " + node.Name);
            }
        }
    }

    string GetText(XmlNode node)
    {
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.NodeType == XmlNodeType.Text)
                return child.Value;
        }
        return null;
    }

    public override void ResetData()
    {
        data = new Holojam.Network.Flake(loadedConfig.Keys.Count);
    }

    protected override void Sync()
    {
        List<string> keys = new List<string>(loadedConfig.Keys);
        if (Sending)
        {
            for (int i = 0; i < loadedConfig.Keys.Count; i++)
            {
                data.vector3s[i] = loadedConfig[keys[i]];
            }
        }
        else if (Tracked)
        {
            for (int i = 0; i < loadedConfig.Keys.Count; i++)
            {
                loadedConfig[keys[i]] = data.vector3s[i];
            }
        }
    }
}