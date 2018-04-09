namespace SubC.Attachments {

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Attachment {

        public AttachmentPhase phase { get; private set; }

        public bool isDetached { get { return phase == AttachmentPhase.Detached; } }
        public bool isAttaching { get { return phase == AttachmentPhase.Attaching; } }
        public bool isAttached { get { return phase == AttachmentPhase.Attached; } }
        public bool isDetaching { get { return phase == AttachmentPhase.Detaching; } }

        public bool isActive { get { return !isDetached; } }

        bool _isConnected;
        public bool isConnected { 
            get {
                return _isConnected;
            } 
            private set {
                if (_isConnected == value)
                    return;
                _isConnected = value;
                if (_isConnected && events.OnConnected != null)
                    events.OnConnected.Invoke(new AttachEventInfo(AttachEventType.OnConnected, this));
                if (!_isConnected && events.OnDisconnected != null)
                    events.OnDisconnected.Invoke(new AttachEventInfo(AttachEventType.OnDisconnected, this));
            }
        }

        AttachObjectList _objects = new AttachObjectList();
        public AttachObjectList objects {
            get {
                return _objects;
            }
            set {
                if (value == _objects)
                    return;
                bool wasAttached = isAttached || isAttaching;
                DetachImmediate();
                CompleteOrphans();
                if (_objects != null) {
                    foreach (AttachObject obj in _objects)
                        obj.attachment = null;
                    _objects.OnChanged.RemoveListener(HandleObjectsListEvent);
                }
                _objects = value;
                if (_objects != null) {
                    _objects.OnChanged.AddListener(HandleObjectsListEvent);
                    foreach (AttachObject obj in _objects)
                        obj.attachment = this;
                }
                if (wasAttached)
                    Attach();
            }
        }

        List<AttachObject> orphans = new List<AttachObject>();

        public bool hasOrphans { get { return orphans.Count > 0; } }

        AttachStrategy _strategy;
        public AttachStrategy strategy { 
            get {
                return _strategy; 
            }
            set {
                if (_strategy == value)
                    return;
                bool wasAttached = isAttached || isAttaching;
                DetachImmediate();
                CompleteOrphans();
                _strategy = value;
                if (wasAttached)
                    Attach();
            }
        }

		public AttachStrategyState strategyState { get; private set; }

        public class Events {
            // attachment events
            public AttachEvent OnWillAttach = new AttachEvent();
            public AttachEvent OnAttached = new AttachEvent();
            public AttachEvent OnWillDetach = new AttachEvent();
            public AttachEvent OnDetached = new AttachEvent();
            public AttachEvent OnConnected = new AttachEvent();
            public AttachEvent OnDisconnected = new AttachEvent();
            // per-object events
            public AttachEvent OnObjectWillJoin = new AttachEvent();
            public AttachEvent OnObjectWillConnect = new AttachEvent();
            public AttachEvent OnObjectConnected = new AttachEvent();
            public AttachEvent OnObjectWillDisconnect = new AttachEvent();
            public AttachEvent OnObjectWillLeave = new AttachEvent();
            public AttachEvent OnObjectLeft = new AttachEvent();
        }
        public Events events = new Events();

        int _executionOrder;
        public int executionOrder { 
            get { return _executionOrder; } 
            set { 
                _executionOrder = value;
                ClingyComponent.instance.attachments.RemoveAt(ClingyComponent.instance.attachments.IndexOfValue(this));
                ClingyComponent.instance.attachments.Add(new ClingyComponent.ExecutionOrder(_executionOrder), this);
            } 
        }

        public Attachment(AttachStrategy strategy, params AttachObject[] objects) {
            this.strategy = strategy;
            _executionOrder = executionOrder;
            ClingyComponent.instance.attachments.Add(new ClingyComponent.ExecutionOrder(_executionOrder), this);
            events.OnObjectConnected.AddListener(OnObjectConnected);
            events.OnObjectWillDisconnect.AddListener(OnObjectWillDisconnect);
            events.OnObjectLeft.AddListener(OnObjectLeft);
            this.objects = new AttachObjectList();
            foreach (AttachObject obj in objects)
                this.objects.Add(obj);
        }

        public Attachment(AttachStrategy strategy, AttachObjectList objects, int executionOrder = 0) {
            this.strategy = strategy;
            _executionOrder = executionOrder;
            ClingyComponent.instance.attachments.Add(new ClingyComponent.ExecutionOrder(_executionOrder), this);
            events.OnObjectConnected.AddListener(OnObjectConnected);
            events.OnObjectWillDisconnect.AddListener(OnObjectWillDisconnect);
            events.OnObjectLeft.AddListener(OnObjectLeft);
            this.objects = objects;
        }

        // fixme - this seems to cause weird issues
        // ~Attachment() {
        //     ClingyComponent.instance.attachments.RemoveAt(ClingyComponent.instance.attachments.IndexOfValue(this));
        // }

        void HandleObjectsListEvent(AttachObjectListEventInfo info) {
            if (info.eventType == AttachObjectListEventType.ObjectAdded)
                HandleObjectAdded(info.obj);
            else if (info.eventType == AttachObjectListEventType.ObjectWillBeRemoved)
                HandleObjectWillBeRemoved(info.obj);
            else if (info.eventType == AttachObjectListEventType.ObjectRemoved)
                HandleObjectWasRemoved(info.obj, info.oldIndexInCategory);
        }

        void HandleObjectAdded(AttachObject obj) {
            orphans.Remove(obj);
            obj.attachment = this;
            // todo - check for non-identical matching objects in orphans and objects
            if (isActive)
                obj.PrepareForAttach(strategy.GetTransitionerForCategory(obj.category));
            UpdateIsConnected();
            if (isAttaching || isAttached) {
                ResolveParamsForObject(obj);
                obj.Attach();
            }
            UpdateIsConnected();
        }

        void HandleObjectWillBeRemoved(AttachObject obj) {
            if (!isActive)
                return;
            orphans.Add(obj);
            obj.Remove();
            UpdateIsConnected();
        }

        void HandleObjectWasRemoved(AttachObject obj, int oldIndexInCategory) {
            if (!isActive)
                return;
            strategy.OnObjectWasRemoved(this, obj, oldIndexInCategory);
            UpdateIsConnected();
        }

        void UpdateIsConnected() {
            if (isActive && objects.Count() > 1 
                    && objects.Count(phase: AttachObjectPhase.Connected) == objects.Count())
                isConnected = true;
            else
                isConnected = false;
        }

        void OnObjectConnected(AttachEventInfo info) {
            UpdateIsConnected();
        }

        void OnObjectWillDisconnect(AttachEventInfo info) {
            UpdateIsConnected();
        }

        void OnObjectLeft(AttachEventInfo info) {
            if (info.attachObject.isRemoved) {
                orphans.Remove(info.attachObject);
                info.attachObject.CleanUp();
                info.attachObject.attachment = null;
                return;
            }
            UpdateIsConnected();
            if (isDetaching && (objects.Count(phase: AttachObjectPhase.Left) 
                    + objects.Count(phase: AttachObjectPhase.Initial)) == objects.Count())
                BeginPhaseDetached();
        }

        bool ResolveParamsForObject(AttachObject obj) {
            obj.CycleResolvedParams();
            obj.resolvedParams.MergeFrom(obj.defaultParams);
            if (obj.attachPoint)
                obj.attachPoint.ApplyParamsForSelf(obj);
            foreach (AttachObject other in objects) {
                if (other == obj)
                    continue;
                if (other.attachPoint)
                    other.attachPoint.ApplyParamsForOther(obj, self: other); // obj == other, relative to the other
            }
            return !obj.ResolvedParamsChanged();
        }

        bool ResolveParams() {
            bool changed = false;
            foreach (AttachObject obj in objects) {
                if (ResolveParamsForObject(obj))
                    changed = true;
            }
            return changed;
        }

        public void DoFixedUpdate() {
			if (!isActive && !hasOrphans)
				return;
            foreach (AttachObject obj in objects) {
                if (obj.transitioner)
                    obj.transitioner.DoFixedUpdate(obj);
            }
            foreach (AttachObject obj in orphans) {
                if (obj.transitioner)
                    obj.transitioner.DoFixedUpdate(obj);
            }
            strategy.DoFixedUpdate(this);
        }

        public void DoUpdate() {
			if (!isActive && !hasOrphans)
				return;
            foreach (AttachObject obj in objects) {
                if (obj.transitioner)
                    obj.transitioner.DoUpdate(obj);
            }
            foreach (AttachObject obj in orphans) {
                if (obj.transitioner)
                    obj.transitioner.DoUpdate(obj);
            }
            strategy.DoUpdate(this);
        }

        public void DoLateUpdate() {
			if (!isActive && !hasOrphans)
				return;
            if (!isDetached) {
                bool changed = ResolveParams();
                if (changed) {
                    // fixme - this doesn't work anymore
                    // if (isAttached && strategy.reattachWhenParamsUpdated) {
                    //     Attach();
                    //     return;
                    // }
                    foreach (AttachObject obj in objects) {
                        if (obj.transitioner)
                            obj.transitioner.OnParamsUpdated(obj);
                    }
                    strategy.OnParamsUpdated(this);
                }
                foreach (AttachObject obj in objects) {
                    if (obj.transitioner)
                        obj.transitioner.DoLateUpdate(obj);
                }
                strategy.DoLateUpdate(this);
            }
            foreach (AttachObject obj in orphans) {
                if (obj.transitioner)
                    obj.transitioner.DoLateUpdate(obj);
            }
        }

        public void Attach() {
            if (isAttached || isAttaching)
                return;
            if (events.OnWillAttach != null)
                events.OnWillAttach.Invoke(new AttachEventInfo(AttachEventType.OnWillAttach, this));
            phase = AttachmentPhase.Attaching;
            if (strategyState == null)
                strategyState = strategy.CreateEmptyState();
            foreach (AttachObject obj in objects)
                obj.PrepareForAttach(strategy.GetTransitionerForCategory(obj.category));
            ResolveParams();
            foreach (AttachObject obj in objects)
                obj.Attach();
            phase = AttachmentPhase.Attached;
            if (events.OnAttached != null)
                events.OnAttached.Invoke(new AttachEventInfo(AttachEventType.OnAttached, this));            
        }

		public void Detach() {
            if (isDetached || isDetaching)
                return;
            if (events.OnWillDetach != null)
                events.OnWillDetach.Invoke(new AttachEventInfo(AttachEventType.OnWillDetach, this));
            phase = AttachmentPhase.Detaching;
            foreach (AttachObject obj in objects)
                obj.Detach();
		}

        public void DetachImmediate() {
            if (isDetached)
                return;
            if (!isDetaching && events.OnWillDetach != null)
                events.OnWillDetach.Invoke(new AttachEventInfo(AttachEventType.OnWillDetach, this));
            phase = AttachmentPhase.Detaching;
            foreach (AttachObject obj in objects)
                if (obj.isActive)
                    obj.DetachImmediate();
            BeginPhaseDetached();
        }

        public void CompleteOrphans() {
            for (int i = orphans.Count - 1; i >= 0; i--)
                orphans[i].DetachImmediate();
        }

        void BeginPhaseDetached() {
            foreach (AttachObject obj in objects)
                obj.CleanUp();
            strategy.DestroyState(strategyState);
            strategyState = null;
            phase = AttachmentPhase.Detached;
            isConnected = false;
            if (events.OnDetached != null)
                events.OnDetached.Invoke(new AttachEventInfo(AttachEventType.OnDetached, this));
        }

    }

}
