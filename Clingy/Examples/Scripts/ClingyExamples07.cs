using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubC.Attachments;

public class ClingyExamples07 : MonoBehaviour {

    public Attacher attacher;
    public GameObject attachHeadButton, detachHeadButton, attachTailButton, detachTailButton, attachLinkButton, 
            detachLinkButton;
    public GameObject linkGameObject;
    AttachObject head, tail, link;

    void Start() {
        head = attacher.attachment.objects.Get(0, 0);
        detachHeadButton.SetActive(true);
        attachHeadButton.SetActive(false);
        tail = attacher.attachment.objects.Get(2, 0);
        detachTailButton.SetActive(true);
        attachTailButton.SetActive(false);
        link = new AttachObject(linkGameObject, 1);
        detachLinkButton.SetActive(false);
        attachLinkButton.SetActive(true);
    }

    public void AttachHead() {
        attacher.attachment.objects.Add(head);
        attachHeadButton.SetActive(false);
        detachHeadButton.SetActive(true);
    }

    public void DetachHead() {
        attacher.attachment.objects.Remove(head);
        attachHeadButton.SetActive(true);
        detachHeadButton.SetActive(false);
    }

    public void AttachTail() {
        attacher.attachment.objects.Add(tail);
        attachTailButton.SetActive(false);
        detachTailButton.SetActive(true);
    }

    public void DetachTail() {
        attacher.attachment.objects.Remove(tail);
        attachTailButton.SetActive(true);
        detachTailButton.SetActive(false);
    }

    public void AttachLink() {
        attacher.attachment.objects.Add(link);
        attachLinkButton.SetActive(false);
        detachLinkButton.SetActive(true);
    }

    public void DetachLink() {
        attacher.attachment.objects.Remove(link);
        attachLinkButton.SetActive(true);
        detachLinkButton.SetActive(false);
    }

}
