using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SubC.Attachments;

public class ClingyExamples09 : MonoBehaviour {

    public Text phase, status;
    public Attacher attacher;

	// Use this for initialization
	void Start () {
		attacher.events.OnWillAttach.AddListener(info => { phase.text = "Attaching"; });
		attacher.events.OnAttached.AddListener(info => { phase.text = "Attached"; });
		attacher.events.OnWillDetach.AddListener(info => { phase.text = "Detaching"; });
		attacher.events.OnDetached.AddListener(info => { phase.text = "Detached"; });
		attacher.events.OnConnected.AddListener(info => { status.text = "Connected"; });
		attacher.events.OnDisconnected.AddListener(info => { status.text = "Disconnected"; });
	}
	
}
