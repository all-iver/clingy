namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

	public abstract class AttachStrategy : ScriptableObject {

        [SerializeField]
        #pragma warning disable 0414
        int _selectedCategory = 0; // for internal/editor use only

        public abstract AttachCategoryDefinition[] GetCategories();

        string[] _categoryLabels;
        public string[] GetCategoryLabels() {
            if (_categoryLabels == null || _categoryLabels.Length == 0) {
                _categoryLabels = new string[GetCategories().Length];
                for (int i = 0; i < GetCategories().Length; i++)
                    _categoryLabels[i] = GetCategories()[i].label;
            }
            return _categoryLabels;
        }

        string[] _categoryObjectLabels;
        public string[] GetCategoryObjectLabels() {
            if (_categoryObjectLabels == null || _categoryObjectLabels.Length == 0) {
                _categoryObjectLabels = new string[GetCategories().Length];
                for (int i = 0; i < GetCategories().Length; i++)
                    _categoryObjectLabels[i] = GetCategories()[i].objectLabel;
            }
            return _categoryObjectLabels;
        }

        public List<Transitioner> transitioners = new List<Transitioner>();

        // public bool reattachWhenParamsUpdated;

        protected Coroutine StartCoroutine(IEnumerator coroutine) {
            return ClingyComponent.instance.StartCoroutine(coroutine);
        }

        protected void StopCoroutine(IEnumerator coroutine) {
            ClingyComponent.instance.StopCoroutine(coroutine);
        }

        protected void StopCoroutine(Coroutine coroutine) {
            ClingyComponent.instance.StopCoroutine(coroutine);
        }

        // reference isn't always needed.  transitioners should always provide a reference; strategies may be able to 
        // get away without one and just pass in the Attachment instead.
        public abstract AttachObject ResolveProvider(int provider, AttachObject reference);

        // reference isn't always needed.  transitioners should always provide a reference; strategies may be able to 
        // get away without one and just pass in the Attachment instead.
        public abstract AttachObject ResolveProvider(int provider, Attachment attachment);

        public abstract int[] GetProvidersForTransitioner(int category);

        public abstract string GetLabelForProvider(int provider);

        public Transitioner GetTransitionerForCategory(int category) {
            if (transitioners.Count > category)
                return transitioners[category];
            return null;
        }

        // for use in the Unity editor
        protected virtual void Reset() {
            _categoryLabels = null;
            _categoryObjectLabels = null;
        }

        public virtual AttachStrategyState CreateEmptyState() {
            return new AttachStrategyState();
        }

        public virtual void DestroyState(AttachStrategyState state) {
        }

        public virtual void UpdateForEditorChanges(Attachment attachment) {
        }

        public abstract bool ConnectObject(AttachObject obj);

        public virtual bool DisconnectObject(AttachObject obj) {
            DisconnectObjectImmediate(obj);
            return true;
        }

        public abstract void DisconnectObjectImmediate(AttachObject obj);

        public virtual void OnParamsUpdated(Attachment attachment) {
        }

        public virtual void DoFixedUpdate(Attachment attachment) {
        }

        public virtual void DoUpdate(Attachment attachment) {
        }

        public virtual void DoLateUpdate(Attachment attachment) {
        }

	}

}