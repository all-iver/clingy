namespace SubC.Attachments {

    using System;
    using UnityEngine;

    [Serializable]
    public class AttachObject {

        [SerializeField]
        GameObject _seedObject;
        public GameObject seedObject {
            get { 
                return _seedObject; 
            }
            set { 
                if (attachment != null)
                    throw new InvalidOperationException("Can't set seedObject on a bound AttachObject; use "
                            + "objects.Set() instead");
                _seedObject = value;
            }
        }

        [SerializeField]
        string _keyPattern;
        public string keyPattern { 
            get {
                return _keyPattern;
            } 
            set {
                if (attachment != null)
                    throw new InvalidOperationException("Can't set keyPattern on a bound AttachObject; use "
                            + "objects.Set() instead");
                _keyPattern = value;
            }
        }

        [SerializeField]
        ParamList _defaultParams;
        public ParamList defaultParams { 
            get {
                return _defaultParams;
            }
            set {
                _defaultParams = value;
            }
        }

        Attachment _attachment;
        public Attachment attachment {
            get {
                return _attachment;
            }
            set {
                if (_attachment == value)
                    return;
                if (isActive)
                    DetachImmediate();
                CleanUp();
                _attachment = value;
            }
        }

        AttachPoint _attachPoint;
        public AttachPoint attachPoint {
            get {
                if (isPrepped)
                    return _attachPoint;
                AttachPoint attachPoint;
                if (!AttachPoint.FindMatching(seedObject, keyPattern, out attachPoint))
                    throw new System.InvalidOperationException("No AttachPoint found for key: " + keyPattern);
                return attachPoint;
            }
        }

        Rigidbody _rigidbody;
        public Rigidbody rigidbody {
            get {
                if (isPrepped)
                    return _rigidbody;
                return attachable.GetComponent<Rigidbody>();
            }
        }

        Rigidbody2D _rigidbody2D;
        public Rigidbody2D rigidbody2D {
            get {
                if (isPrepped)
                    return _rigidbody2D;
                return attachable.GetComponent<Rigidbody2D>();
            }
        }

        ParamList _resolvedParams;
        public ParamList resolvedParams { 
            get {
                if (_resolvedParams == null)
                    _resolvedParams = new ParamList();
                return _resolvedParams;
            } 
            private set {
                _resolvedParams = value;
            }
        }

        ParamList _lastResolvedParams;
        public ParamList lastResolvedParams { 
            get {
                if (_lastResolvedParams == null)
                    _lastResolvedParams = new ParamList();
                return _lastResolvedParams;
            } 
            private set {
                _lastResolvedParams = value;
            }
        }

        SpriteRenderer _spriteRenderer;
        public SpriteRenderer spriteRenderer { 
            get {
                if (isPrepped)
                    return _spriteRenderer;
                if (attachPoint)
                    return attachPoint.GetSpriteRenderer();
                else
                    return seedObject.GetComponentInChildren<SpriteRenderer>();
            }
        }

        public AttachObjectState state;

        [SerializeField]
        int _category;
        public int category { 
            get {
                return _category;
            }
            private set {
                _category = value;
            }
        }

        // when removed, this is the index we had before we were removed
        int oldIndexInCategory;
        public int indexInCategory { 
            get {
                if (attachment == null)
                    throw new InvalidOperationException("AttachObject has no Attachment");
                if (isRemoved)
                    return oldIndexInCategory;
                return attachment.objects.GetIndexInCategory(this);
            } 
        }

        Transitioner _transitioner;
        public Transitioner transitioner { 
            get {
                return _transitioner;
            }
            set {
                if (isActive)
                    throw new InvalidOperationException("Can't set the transitioner for an active AttachObject");
                _transitioner = value;
                if (value)
                    transitionerState = value.CreateEmptyState();
                else
                    transitionerState = null;
            }
        }
        public Transitioner.TransitionerState transitionerState { get; private set; }

        public GameObject attachable { get { return attachPoint ? attachPoint.attachable : seedObject; } }
        public GameObject paramsRelativeTo { get { return attachPoint ? attachPoint.gameObject : seedObject; } }

        public AttachObjectPhase phase { get; private set; }

        public bool isInitial { get { return phase == AttachObjectPhase.Initial; } }
        public bool isJoining { get { return phase == AttachObjectPhase.Joining; } }
        public bool isConnecting { get { return phase == AttachObjectPhase.Connecting; } }
        public bool isConnected { get { return phase == AttachObjectPhase.Connected; } }
        public bool isDisconnecting { get { return phase == AttachObjectPhase.Disconnecting; } }
        public bool isLeaving { get { return phase == AttachObjectPhase.Leaving; } }
        public bool isLeft { get { return phase == AttachObjectPhase.Left; } }

        public bool isActive { get { return !isInitial && !isLeft; } }
        public bool isStrategyActive { get { return isConnecting || isConnected || isDisconnecting; } }
        public bool isTransitionerPending { get { return isJoining || isLeaving; } }

        public bool isPrepped { get; private set; }
        public bool isRemoved { get; private set; }

        const string OnWillJoinMessage = "OnObjectWillJoin";
        const string OnWillConnectMessage = "OnObjectWillConnect";
        const string OnConnectedMessage = "OnObjectConnected";
        const string OnWillDisconnectMessage = "OnObjectWillDisconnect";
        const string OnWillLeaveMessage = "OnObjectWillLeave";
        const string OnLeftMessage = "OnObjectLeft";

        public AttachObject(GameObject seedObject = null, int category = 0, string keyPattern = "", 
                ParamList defaultParams = null) {
            this.seedObject = seedObject;
            this.category = category;
            this.keyPattern = keyPattern;
            this.defaultParams = defaultParams ?? new ParamList();
        }

        // we keep two ParamLists and swap them every frame.  this prevents us from having to instantiate a new 
        // ParamList every frame but still be able to keep track of the previous frame's params so we can know what has 
        // changed.
        public void CycleResolvedParams() {
            ParamList temp = resolvedParams;
            _resolvedParams = lastResolvedParams;
            _lastResolvedParams = temp;
            _resolvedParams.Clear();
        }

        // checks if the current resolved params have changed since the last frame
        public bool ResolvedParamsChanged() {
            return _resolvedParams.Equals(_lastResolvedParams);
        }

        public bool ResolvesToSameObject(AttachObject other) {
            return attachPoint == other.attachPoint && attachable == other.attachable 
                    && category == other.category && spriteRenderer == other.spriteRenderer;
        }

        void Notify(AttachEvent evt, string message, AttachEventType eventType) {
            AttachEventInfo info = new AttachEventInfo(eventType, this);
            seedObject.SendMessage(message, info, SendMessageOptions.DontRequireReceiver);
            if (evt != null)
                evt.Invoke(info);
        }

        public void PrepareForAttach(Transitioner transitioner) {
            Debug.Assert(attachment != null);
            isRemoved = false;
            if (isPrepped)
                return;
            this.transitioner = transitioner;
            _attachPoint = attachPoint;
            _rigidbody = rigidbody;
            _rigidbody2D = rigidbody2D;
            _spriteRenderer = spriteRenderer;
            if (attachPoint)
                attachPoint.RegisterAttachObject(this);
            isPrepped = true;
        }

        public void CleanUp() {
            Debug.Assert(!isActive);
            if (!isPrepped)
                return;
            transitioner = null;
            if (attachPoint)
                attachPoint.DeregisterAttachObject(this);
            _attachPoint = null;
            _rigidbody = null;
            _rigidbody2D = null;
            _spriteRenderer = null;
            isPrepped = false;
        }

        public void Attach() {
            Debug.Assert(attachment != null);
            Debug.Assert(isPrepped);
            if (isActive) {
                if (isStrategyActive)
                    attachment.strategy.DisconnectObjectImmediate(this);
                if (transitioner)
                    transitioner.LeaveImmediate(this);
            }
            phase = AttachObjectPhase.Initial;
            Join();
        }

        void Join() {
            Debug.Assert(attachment != null);
            Debug.Assert(isPrepped);
            Debug.Assert(isInitial);
            Notify(attachment.events.OnObjectWillJoin, OnWillJoinMessage, AttachEventType.OnObjectWillJoin);
            isRemoved = false;
            phase = AttachObjectPhase.Joining;
            if (!transitioner || transitioner.Join(this))
                SetJoined();
        }

        public void SetJoined() {
            Debug.Assert(isJoining);
            Debug.Assert(isPrepped);
            Connect();
        }

        void Connect() {
            Debug.Assert(isJoining);
            Debug.Assert(isPrepped);
            Notify(attachment.events.OnObjectWillConnect, OnWillConnectMessage, AttachEventType.OnObjectWillConnect);
            phase = AttachObjectPhase.Connecting;
            if (attachment.strategy.ConnectObject(this))
                SetConnected();
        }

        public void SetConnected() {
            Debug.Assert(isConnecting);
            Debug.Assert(isPrepped);
            phase = AttachObjectPhase.Connected;
            if (transitioner)
                transitioner.OnConnected(this);
            Notify(attachment.events.OnObjectConnected, OnConnectedMessage, AttachEventType.OnObjectConnected);
        }

        public void Remove() {
            Debug.Assert(isActive && !isRemoved);
            Debug.Assert(isPrepped);
            this.oldIndexInCategory = indexInCategory;
            isRemoved = true;
            Detach();
        }

        public void Detach() {
            Debug.Assert(isActive);
            Debug.Assert(isPrepped);
            if (isDisconnecting || isLeaving)
                return;
            if (isJoining)
                Leave();
            else if (isConnecting || isConnected)
                Disconnect();
        }

        public void DetachImmediate() {
            Debug.Assert(isActive);
            Debug.Assert(isPrepped);
            if (isStrategyActive)
                DisconnectImmediate();
            else
                LeaveImmediate();
        }

        void Disconnect() {
            Debug.Assert(isConnecting || isConnected);
            Debug.Assert(isPrepped);
            Notify(attachment.events.OnObjectWillDisconnect, OnWillDisconnectMessage, 
                    AttachEventType.OnObjectWillDisconnect);
            phase = AttachObjectPhase.Disconnecting;
            if (attachment.strategy.DisconnectObject(this))
                SetDisconnected();
        }

        public void SetDisconnected() {
            Debug.Assert(isDisconnecting);
            Debug.Assert(isPrepped);
            Leave();
        }

        void Leave() {
            Debug.Assert(isDisconnecting || isJoining);
            Debug.Assert(isPrepped);
            Notify(attachment.events.OnObjectWillLeave, OnWillLeaveMessage, AttachEventType.OnObjectWillLeave);
            phase = AttachObjectPhase.Leaving;
            if (!transitioner || transitioner.Leave(this))
                SetLeft();
        }

        void DisconnectImmediate() {
            Debug.Assert(isConnecting || isConnected || isDisconnecting);
            Debug.Assert(isPrepped);
            if (!isDisconnecting)
                Notify(attachment.events.OnObjectWillDisconnect, OnWillDisconnectMessage, 
                        AttachEventType.OnObjectWillDisconnect);
            phase = AttachObjectPhase.Disconnecting;
            attachment.strategy.DisconnectObjectImmediate(this);
            LeaveImmediate();
        }

        void LeaveImmediate() {
            Debug.Assert(isJoining || isDisconnecting || isLeaving);
            Debug.Assert(isPrepped);
            if (!isLeaving)
                Notify(attachment.events.OnObjectWillLeave, OnWillLeaveMessage, AttachEventType.OnObjectWillLeave);
            phase = AttachObjectPhase.Leaving;
            if (transitioner)
                transitioner.LeaveImmediate(this);
            SetLeft();
        }

        public void SetLeft() {
            Debug.Assert(isLeaving);
            Debug.Assert(isPrepped);
            phase = AttachObjectPhase.Left;
            Notify(attachment.events.OnObjectLeft, OnLeftMessage, AttachEventType.OnObjectLeft);
        }

        public AttachObject GetOther() {
            if (attachment == null)
                throw new InvalidOperationException("AttachObject has no Attachment");
            if (isRemoved)
                throw new InvalidOperationException("AttachObject has been removed");
            if (attachment.objects.Count() != 2)
                throw new InvalidOperationException("GetOther() requires exactly 2 objects in the Attachment");
            bool found = false;
            AttachObject other = null;
            foreach (AttachObject obj in attachment.objects) {
                if (obj == this) {
                    found = true;
                    continue;
                }
                other = obj;
            }
            if (!found)
                throw new System.InvalidOperationException("Attachment does not contain this AttachObject");
            if (other != null)
                return other;
            throw new System.InvalidOperationException("This Attachment has no other object");
        }

        // resolves a param provider using this object as the reference
        public AttachObject ResolveProvider(int provider) {
            return attachment.strategy.ResolveProvider(provider, this);
        }

    }

}