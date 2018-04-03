namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
	using System.Text.RegularExpressions;

	[AddComponentMenu("Clingy/Attach Point", 2)]
	public class AttachPoint : MonoBehaviour {

		public GameObject attachable;
		public string key = "";
		public SpriteRenderer spriteRenderer;
        public SpriteParams spriteParams;
        [HideInInspector]
		public ParamList baseParams = new ParamList();

        // for editor use only
        [SerializeField]
        [HideInInspector]
        #pragma warning disable 0414
        bool expandParamsInInspector = true;

        protected List<AttachObject> attachObjects = new List<AttachObject>();

        // fixme - this terminology might be wrong, should be isActive maybe
        public bool isAttached { get { return attachObjects.Count > 0; } }

		public static bool MatchesKey(string key, string pattern) {
            if (string.IsNullOrEmpty(pattern))
                return true;
            if (key == null)
                key = "";
			Match match = Regex.Match(key, pattern, RegexOptions.IgnoreCase);
			return match.Success && match.Value.Length == key.Length;
		}

		// returns true if GameObject go either has a matching available AttachPoint, 
		// or if there are no AttachPoints and the key doesn't require one.
		public static bool HasMatching(GameObject go, string keyPattern) {
			AttachPoint ap;
			return FindMatching(go, keyPattern, out ap);
		}

		public static bool FindMatching(GameObject go, string keyPattern, out AttachPoint result) {
			foreach (AttachPoint ap in go.GetComponents<AttachPoint>()) {
				if (!ap.MatchesKey(keyPattern))
					continue;
				result = ap;
				return true;
			}
			result = null;
			if (!MatchesKey("", keyPattern))
				return false; // an AttachPoint with a specific key is required
			return true; // no AttachPoint required
		}

        [System.Serializable]
        public class AttachPointEventTrigger : AttachEventTrigger {
            protected static AttachEventType[] _supportedEventTypes = new AttachEventType[] { 
                AttachEventType.OnObjectWillJoin, AttachEventType.OnObjectWillConnect,
                AttachEventType.OnObjectConnected, AttachEventType.OnObjectWillDisconnect, 
                AttachEventType.OnObjectWillLeave, AttachEventType.OnObjectLeft
            };
            public override AttachEventType[] supportedEventTypes { get { return _supportedEventTypes; } }
            public AttachEvent OnObjectWillJoin { get { return GetOrCreateEvent(AttachEventType.OnObjectWillJoin); } }
            public AttachEvent OnObjectWillConnect { get { 
                    return GetOrCreateEvent(AttachEventType.OnObjectWillConnect); } }
            public AttachEvent OnObjectConnected { get { 
                    return GetOrCreateEvent(AttachEventType.OnObjectConnected); } }
            public AttachEvent OnObjectWillDisconnect { get { 
                    return GetOrCreateEvent(AttachEventType.OnObjectWillDisconnect); } }
            public AttachEvent OnObjectWillLeave { get { 
                    return GetOrCreateEvent(AttachEventType.OnObjectWillLeave); } }
            public AttachEvent OnObjectLeft { get { return GetOrCreateEvent(AttachEventType.OnObjectLeft); } }
        }
        [HideInInspector]
        public AttachPointEventTrigger events = new AttachPointEventTrigger();

		public override string ToString() {
			if (this == null)
				return base.ToString(); 
			return string.Format("AttachPoint({0}, key: {1})", name, key);
		}

        public AttachObject GetAttachObject(Attachment attachment) {
            foreach (AttachObject obj in attachObjects)
                if (obj.attachment == attachment)
                    return obj;
            throw new System.InvalidOperationException("Attachment is not registered with this AttachPoint");
        }

        void HandleEvent(AttachEventInfo info) {
            AttachEventInfo newInfo = new AttachEventInfo(info.eventType, info.attachObject, sender: this);
            if (!attachObjects.Contains(info.attachObject))
                return;
            AttachEvent evt = events.GetOrCreateEvent(newInfo.eventType);
            if (evt != null)
                evt.Invoke(newInfo);
        }

		// called by Attachment
		public void RegisterAttachObject(AttachObject obj) {
            Debug.Assert(!attachObjects.Contains(obj));
            attachObjects.Add(obj);
            obj.attachment.events.OnObjectWillJoin.AddListener(HandleEvent);
            obj.attachment.events.OnObjectWillConnect.AddListener(HandleEvent);
            obj.attachment.events.OnObjectConnected.AddListener(HandleEvent);
            obj.attachment.events.OnObjectWillDisconnect.AddListener(HandleEvent);
            obj.attachment.events.OnObjectWillLeave.AddListener(HandleEvent);
            obj.attachment.events.OnObjectLeft.AddListener(HandleEvent);
		}

		// called by Attachment
		public void DeregisterAttachObject(AttachObject obj) {
            Debug.Assert(attachObjects.Contains(obj));
            attachObjects.Remove(obj);
            obj.attachment.events.OnObjectWillJoin.RemoveListener(HandleEvent);
            obj.attachment.events.OnObjectWillConnect.RemoveListener(HandleEvent);
            obj.attachment.events.OnObjectConnected.RemoveListener(HandleEvent);
            obj.attachment.events.OnObjectWillDisconnect.RemoveListener(HandleEvent);
            obj.attachment.events.OnObjectWillLeave.RemoveListener(HandleEvent);
            obj.attachment.events.OnObjectLeft.RemoveListener(HandleEvent);
		}

		public bool MatchesKey(string pattern) {
			return MatchesKey(key, pattern);
		}

        public virtual void ApplyParamsForSelf(AttachObject obj) {
            if (spriteParams)
                spriteParams.MergeParamsForCurrentSprite(obj.resolvedParams, GetSpriteRenderer());
            obj.resolvedParams.MergeFrom(baseParams);
        }

        public virtual void ApplyParamsForOther(AttachObject other, AttachObject self) {
        }

        public virtual Param GetParam(ParamType type, string name) {
            if (spriteParams && spriteParams.HasParam(GetSpriteRenderer(), type, name))
                return spriteParams.GetParam(GetSpriteRenderer(), type, name);
            return baseParams.GetParam(type, name);
        }

        public virtual Param GetParam(Param defaultParam) {
            if (spriteParams && spriteParams.HasParam(GetSpriteRenderer(), defaultParam.type, defaultParam.name))
                return spriteParams.GetParam(GetSpriteRenderer(), defaultParam);
            return baseParams.GetParam(defaultParam);
        }

		public virtual GameObject GetAttachable() {
			return attachable ? attachable : gameObject;
		}

        void Reset() {
            if (attachable == null)
                attachable = gameObject;
        }

        void OnValidate() {
            if (attachable == null)
                attachable = gameObject;
        }

        public virtual SpriteRenderer GetSpriteRenderer() {
            if (spriteRenderer)
                return spriteRenderer;
            return GetAttachable().GetComponentInChildren<SpriteRenderer>();
        }

        public void DetachAll() {
            for (int i = attachObjects.Count - 1; i >= 0; i--)
                attachObjects[i].attachment.Detach();
        }

        public AttachObject GetOther(Attachment attachment = null) {
            if (attachment == null) {
                if (attachObjects.Count > 1)
                    throw new System.InvalidOperationException("Specify an Attachment for GetOther() when the "
                            + "AttachPoint is connected to multiple Attachments");
                if (attachObjects.Count == 0)
                    return null;
                attachment = attachObjects[0].attachment;
            }
            AttachObject thisAO = GetAttachObject(attachment);
            return thisAO.GetOther();
        }

	}

}