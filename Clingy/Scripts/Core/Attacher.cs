namespace SubC.Attachments {

    using UnityEngine;
    using System.Collections.Generic;

	[AddComponentMenu("Clingy/Attacher", 0)]
    public class Attacher : MonoBehaviour {

        [SerializeField]
        [HideInInspector]
		AttachObjectList _objects = new AttachObjectList();
        public AttachObjectList objects { 
            get { 
                return _objects; 
            } 
            private set {
                _objects = value; 
            } 
        }

		[TooltipAttribute("Calls Attach() from Start().")]
		public bool attachOnStart = false;

        Attachment _attachment;
		public Attachment attachment { 
            get { return _attachment; } 
            private set {
                if (_attachment != null) {
                    _attachment.events.OnWillAttach.RemoveListener(HandleEvent);
                    _attachment.events.OnAttached.RemoveListener(HandleEvent);
                    _attachment.events.OnWillDetach.RemoveListener(HandleEvent);
                    _attachment.events.OnDetached.RemoveListener(HandleEvent);
                    _attachment.events.OnConnected.RemoveListener(HandleEvent);
                    _attachment.events.OnDisconnected.RemoveListener(HandleEvent);
                    _attachment.events.OnObjectWillJoin.RemoveListener(HandleEvent);
                    _attachment.events.OnObjectWillConnect.RemoveListener(HandleEvent);
                    _attachment.events.OnObjectConnected.RemoveListener(HandleEvent);
                    _attachment.events.OnObjectWillDisconnect.RemoveListener(HandleEvent);
                    _attachment.events.OnObjectWillLeave.RemoveListener(HandleEvent);
                    _attachment.events.OnObjectLeft.RemoveListener(HandleEvent);
                }
                _attachment = value;
                _attachment.events.OnWillAttach.AddListener(HandleEvent);
                _attachment.events.OnAttached.AddListener(HandleEvent);
                _attachment.events.OnWillDetach.AddListener(HandleEvent);
                _attachment.events.OnDetached.AddListener(HandleEvent);
                _attachment.events.OnConnected.AddListener(HandleEvent);
                _attachment.events.OnDisconnected.AddListener(HandleEvent);
                _attachment.events.OnObjectWillJoin.AddListener(HandleEvent);
                _attachment.events.OnObjectWillConnect.AddListener(HandleEvent);
                _attachment.events.OnObjectConnected.AddListener(HandleEvent);
                _attachment.events.OnObjectWillDisconnect.AddListener(HandleEvent);
                _attachment.events.OnObjectWillLeave.AddListener(HandleEvent);
                _attachment.events.OnObjectLeft.AddListener(HandleEvent);
            }
        }

        public bool isDetached { get { return attachment != null && attachment.isDetached; } }
        public bool isAttaching { get { return attachment != null && attachment.isAttaching; } }
        public bool isAttached { get { return attachment != null && attachment.isAttached; } }
        public bool isDetaching { get { return attachment != null && attachment.isDetaching; } }
        public bool isConnected { get { return attachment != null && attachment.isConnected; } }
        
        public bool isActive { get { return attachment != null && attachment.isActive; } }

        [SerializeField]
        AttachStrategy _strategy;
        public AttachStrategy strategy { 
            get { 
                return _strategy; 
            } 
            set { 
                _strategy = value; 
                if (attachment != null)
                    attachment.strategy = value;
            } 
        }

        [HideInInspector]
        [SerializeField]
        int _executionOrder;
        public int executionOrder { 
            get { return _executionOrder; }
            set { 
                _executionOrder = value;
                if (attachment != null)
                    attachment.executionOrder = value; 
            }
        }

		private bool appQuit;

        [System.Serializable]
        public class AttacherEventTrigger : AttachEventTrigger {
            protected static AttachEventType[] _supportedEventTypes = new AttachEventType[] { 
                AttachEventType.OnWillAttach, AttachEventType.OnAttached, 
                AttachEventType.OnWillDetach, AttachEventType.OnDetached,
                AttachEventType.OnObjectWillJoin, AttachEventType.OnObjectWillConnect, 
                AttachEventType.OnObjectConnected, AttachEventType.OnObjectWillDisconnect,
                AttachEventType.OnObjectWillLeave, AttachEventType.OnObjectLeft
            };
            public override AttachEventType[] supportedEventTypes { get { return _supportedEventTypes; } }
            public AttachEvent OnWillAttach { get { return GetOrCreateEvent(AttachEventType.OnWillAttach); } }
            public AttachEvent OnAttached { get { return GetOrCreateEvent(AttachEventType.OnAttached); } }
            public AttachEvent OnWillDetach { get { return GetOrCreateEvent(AttachEventType.OnWillDetach); } }
            public AttachEvent OnDetached { get { return GetOrCreateEvent(AttachEventType.OnDetached); } }
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
        public AttacherEventTrigger events = new AttacherEventTrigger();

		void OnApplicationQuit() {
			appQuit = true;
		}

        void Awake() {
            attachment = new Attachment(strategy, objects, _executionOrder);
        }

		void OnDestroy() {
			// unity throws errors when we set parents to null from OnDestroy() if the application
			// is quitting, so this just avoids detaching on application quit altogether.
			if (appQuit)
				return;
			if (attachment.isAttached)
				attachment.DetachImmediate();
		}

		// Use this for initialization
		void Start() {
			if (attachOnStart)
				Attach();
		}

        void HandleEvent(AttachEventInfo info) {
            AttachEvent evt = events.GetOrCreateEvent(info.eventType);
            if (evt == null)
                return;
            AttachEventInfo newInfo = new AttachEventInfo(info.eventType, info.attachment, info.attachObject, 
                    sender: this);
            evt.Invoke(newInfo);
        }

		public void Attach() {
            attachment.Attach();
		}

		public void Detach() {
            attachment.Detach();
		}

        // for the unity editor
        void Reset() {
            // if (request.attach == null)
            //     request.attach = gameObject;
        }

        // for the unity editor
        void OnValidate() {
            // if (request.attach == null)
            //     request.attach = gameObject;
        }

    }

}
