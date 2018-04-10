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
		attacher.events.OnWillAttach.AddListener(info => { phase.text = "Attaching"; phase.color = Color.yellow; });
		attacher.events.OnAttached.AddListener(info => { phase.text = "Attached"; phase.color = Color.white; });
		attacher.events.OnWillDetach.AddListener(info => { phase.text = "Detaching"; phase.color = Color.yellow; });
		attacher.events.OnDetached.AddListener(info => { phase.text = "Detached"; phase.color = Color.black; });
		attacher.events.OnConnected.AddListener(info => { status.text = "Connected"; status.color = Color.white; });
		attacher.events.OnDisconnected.AddListener(info => { 
                status.text = "Disconnected"; status.color = Color.black; });
	}
	
}
