namespace SubC.Attachments {

    using UnityEngine;

	public struct AttachEventInfo {
        public AttachEventType eventType;
        public MonoBehaviour sender;
		public Attachment attachment;
        public AttachObject attachObject;

        public AttachEventInfo(AttachEventType eventType, MonoBehaviour sender = null) {
            this.eventType = eventType;
            this.sender = sender;
            this.attachment = null;
            this.attachObject = null;
        }

        public AttachEventInfo(AttachEventType eventType, Attachment attachment, MonoBehaviour sender = null) {
            this.eventType = eventType;
            this.attachment = attachment;
            this.sender = null;
            this.attachObject = null;
        }

        public AttachEventInfo(AttachEventType eventType, AttachObject attachObject, MonoBehaviour sender = null) {
            this.eventType = eventType;
            this.attachObject = attachObject;
            this.attachment = attachObject.attachment;
            this.sender = null;
        }

        // this constructor is useful when you don't know if you have an attachment or an attachObject and can't be
        // bothered to check
        public AttachEventInfo(AttachEventType eventType, Attachment attachment, AttachObject attachObject, 
                MonoBehaviour sender = null) {
            this.eventType = eventType;
            this.attachment = attachment;
            this.attachObject = attachObject;
            this.sender = null;
        }
	}

}