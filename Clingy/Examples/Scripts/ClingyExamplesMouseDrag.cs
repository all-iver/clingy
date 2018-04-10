using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubC.Attachments;

public class ClingyExamplesMouseDrag : MonoBehaviour {

    ClingyMouse mouse;
    Attachment attachment;
    public AttachStrategy dragStrategy;

    void Start() {
        mouse = GetComponent<ClingyMouse>();
        mouse.events.OnMouse0Down.AddListener(info => {
            Collider2D coll = Physics2D.OverlapPoint(transform.position);
            if (!coll)
                return;
            attachment = Clingy.AttachOneToOne(dragStrategy, gameObject, coll.gameObject);
        });
        mouse.events.OnMouse0Up.AddListener(info => {
            if (attachment != null) {
                attachment.Detach();
                attachment = null;
            }
        });
    }

}
