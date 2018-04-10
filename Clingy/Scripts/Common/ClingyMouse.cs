namespace SubC.Attachments {

	using UnityEngine;

	public class ClingyMouse : MonoBehaviour {

        [System.Serializable]
        public class ClingyMouseEventTrigger : AttachEventTrigger {
            protected static AttachEventType[] _supportedEventTypes = new AttachEventType[] { 
                   AttachEventType.OnMouse0Down, AttachEventType.OnMouse0Up,
                   AttachEventType.OnMouse1Down, AttachEventType.OnMouse1Up };
            public override AttachEventType[] supportedEventTypes { get { return _supportedEventTypes; } }
            public AttachEvent OnMouse0Down { get { return GetOrCreateEvent(AttachEventType.OnMouse0Down); } }
            public AttachEvent OnMouse0Up { get { return GetOrCreateEvent(AttachEventType.OnMouse0Up); } }
            public AttachEvent OnMouse1Down { get { return GetOrCreateEvent(AttachEventType.OnMouse1Down); } }
            public AttachEvent OnMouse1Up { get { return GetOrCreateEvent(AttachEventType.OnMouse1Up); } }
        }
        public ClingyMouseEventTrigger events = new ClingyMouseEventTrigger();

		void LateUpdate () {
			Vector3 mouseWorldPos = Input.mousePosition;
			mouseWorldPos.z = -Camera.main.transform.position.z;
			mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseWorldPos);
			transform.position = mouseWorldPos;

			if (events.OnMouse0Down != null && Input.GetMouseButtonDown(0))
				events.OnMouse0Down.Invoke(new AttachEventInfo(AttachEventType.OnMouse0Down, this));
			if (events.OnMouse0Up != null && Input.GetMouseButtonUp(0))
				events.OnMouse0Up.Invoke(new AttachEventInfo(AttachEventType.OnMouse0Up, this));
			if (events.OnMouse1Down != null && Input.GetMouseButtonDown(1))
				events.OnMouse1Down.Invoke(new AttachEventInfo(AttachEventType.OnMouse1Down, this));
			if (events.OnMouse1Up != null && Input.GetMouseButtonUp(1))
				events.OnMouse1Up.Invoke(new AttachEventInfo(AttachEventType.OnMouse1Up, this));
		}

	}

}