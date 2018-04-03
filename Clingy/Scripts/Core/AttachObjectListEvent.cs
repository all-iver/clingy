namespace SubC.Attachments {

    public enum AttachObjectListEventType {
        ObjectAdded,
        ObjectWillBeRemoved,
        ObjectRemoved,
    }

    public struct AttachObjectListEventInfo {
        public AttachObjectListEventType eventType;
        public AttachObjectList sender;
        public AttachObject obj;
        public int oldIndexInCategory;

        public AttachObjectListEventInfo(AttachObjectListEventType eventType, AttachObjectList sender,
                AttachObject obj = null, int oldIndexInCategory = -1) {
            this.eventType = eventType;
            this.sender = sender;
            this.obj = obj;
            this.oldIndexInCategory = oldIndexInCategory;
        }
    }

    public class AttachObjectListEvent : UnityEngine.Events.UnityEvent<AttachObjectListEventInfo> {};

}