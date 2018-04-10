using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubC.Attachments;

public class ClingyExamples04 : MonoBehaviour {

    public Attacher attacher;
    public GameObject anchorPosBtn, anchorNegBtn;
    bool toggle;

	// Use this for initialization
	void Start () {
        OnPosClicked();
	}

    public void OnPosClicked() {
        anchorPosBtn.SetActive(false);
        anchorNegBtn.SetActive(true);
        attacher.attachment.objects.Get(0, 0).defaultParams.SetParam(new Param(new Vector2(-0.5f, 0.5f), "anchor"));
    }

    public void OnNegClicked() {
        anchorPosBtn.SetActive(true);
        anchorNegBtn.SetActive(false);
        attacher.attachment.objects.Get(0, 0).defaultParams.SetParam(new Param(new Vector2(-0.5f, -0.5f), "anchor"));
    }

}
