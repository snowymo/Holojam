using System.Collections;
using System.Collections.Generic;
using Holojam.Tools;
using UnityEngine;

public class Results : SynchronizableTrackable
{

    string info;

    public ViewCtrl whoamI;
    public BoardCtrl board;

    [SerializeField]
    string label = "RES";
    [SerializeField]
    string scope = "";

    [SerializeField]
    bool host = true;
    [SerializeField]
    bool autoHost = false;

    // As an example, allow all the Synchronizable properties to be publicly settable
    // In practice, you probably want to control some or all of these manually in code.

    public void SetLabel(string label) { this.label = label; }
    public void SetScope(string scope) { this.scope = scope; }

    public void SetHost(bool host) { this.host = host; }
    public void SetAutoHost(bool autoHost) { this.autoHost = autoHost; }

    // Point the property overrides to the public inspector fields

    public override string Label { get { return label; } }
    public override string Scope { get { return scope; } }

    public override bool Host { get { return host; } }
    public override bool AutoHost { get { return autoHost; } }

    // Add the scale vector to Trackable, which by default only contains position/rotation
    public override void ResetData()
    {
        data = new Holojam.Network.Flake(0, 0, 0, 0, 0, true);
        host = false;
    }

    // Override Sync() to include the scale vector
    protected override void Sync()
    {
        //base.Sync();
        if (isFinished())
        {
            host = true;
        }
        transform.position = Camera.main.transform.position;// + Camera.main.transform.forward * 2;
        transform.rotation = Camera.main.transform.rotation;

        if (Sending)
        {
            data.text = "YOU LOSE";
        }
        else
        {
            info = data.text;
        }
        
        if(info != "")
        {
            GetComponent<TextMesh>().text = info;
            GetComponent<TextMesh>().offsetZ = 3.5f;
        }
        
	}

    bool isFinished()
    {
        string name;
        if(whoamI.viewRoom == ViewCtrl.VIEWROOM.rooma)// cross
        {
            name = "cross";
       
        }else
        {
            name = "circle";
          
        }
        for(int i = 0; i < 9; i+=3)
        {
            if (board.gameObject.transform.Find("chess0" + i.ToString()).Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess0" + (i+1).ToString()).Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess0" + (i+2).ToString()).Find(name).gameObject.activeSelf)
            {
                info = "YOU WIN";
                return true;
            }
        }
        for (int i = 0; i < 3; i += 1)
        {
            if (board.gameObject.transform.Find("chess0" + i.ToString()).Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess0" + (i + 3).ToString()).Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess0" + (i + 6).ToString()).Find(name).gameObject.activeSelf)
            {
                info = "YOU WIN";
                return true;
            }
        }

        if (board.gameObject.transform.Find("chess00").Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess04").Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess08").Find(name).gameObject.activeSelf)
        {
            info = "YOU WIN";
            return true;
        }
        if (board.gameObject.transform.Find("chess02").Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess04").Find(name).gameObject.activeSelf
            && board.gameObject.transform.Find("chess06").Find(name).gameObject.activeSelf)
        {
            info = "YOU WIN";
            return true;
        }
        return false;
    }

}
