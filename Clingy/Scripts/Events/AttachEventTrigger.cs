namespace SubC.Attachments {

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public abstract class AttachEventTrigger {

        [Serializable]
        public class Entry {
            public AttachEventType eventType;
            public AttachEvent callback = new AttachEvent();
            public bool hideInInspector = true;
        }

        public abstract AttachEventType[] supportedEventTypes { get; }

        [SerializeField]
        public List<Entry> entries = new List<Entry>();

        public AttachEvent GetOrCreateEvent(AttachEventType eventType, bool hideInInspector = true) {
            foreach (Entry e in entries) {
                if (e.eventType == eventType) {
                    if (!hideInInspector)
                        e.hideInInspector = false;
                    return e.callback;
                }
            }
            Entry entry = new Entry();
            entry.eventType = eventType;
            if (!hideInInspector)
                entry.hideInInspector = false;
            entries.Add(entry);
            return entry.callback;
        }

        public bool HasVisibleEntryForEventType(AttachEventType eventType) {
            foreach (Entry e in entries)
                if (e.eventType == eventType)
                    return !e.hideInInspector;
            return false;
        }

    }

}