namespace SubC.Attachments {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Transitioner : ScriptableObject {

        public class TransitionerState {
        }

        public static string GetEditorClass() {
            return "SubC.Attachments.ClingyEditor.TransitionerEditor";
        }

        // for the unity editor
        public virtual void Reset(AttachStrategy strategy) {
        }

        public virtual TransitionerState CreateEmptyState() {
            return new TransitionerState();
        }

        protected Coroutine StartCoroutine(IEnumerator coroutine) {
            return ClingyComponent.instance.StartCoroutine(coroutine);
        }

        protected void StopCoroutine(IEnumerator coroutine) {
            ClingyComponent.instance.StopCoroutine(coroutine);
        }

        protected void StopCoroutine(Coroutine coroutine) {
            ClingyComponent.instance.StopCoroutine(coroutine);
        }

        public virtual bool Join(AttachObject obj) {
            return true;
        }

        public virtual bool Leave(AttachObject obj) {
            LeaveImmediate(obj);
            return true;
        }

        public virtual void LeaveImmediate(AttachObject obj) {
        }

        public virtual void OnConnected(AttachObject obj) {
        }

        public virtual void OnDisconnected(AttachObject obj) {
        }

        public virtual void OnParamsUpdated(AttachObject obj) {
        }

        public virtual void DoFixedUpdate(AttachObject obj) {
        }

        public virtual void DoUpdate(AttachObject obj) {
        }

        public virtual void DoLateUpdate(AttachObject obj) {
        }

    }

}