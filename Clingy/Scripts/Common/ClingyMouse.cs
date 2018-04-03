namespace SubC.Attachments {

	using UnityEngine;

	public class ClingyMouse : MonoBehaviour {

        // [System.Serializable]
        // public class ClingyMouseEventTrigger : ClingyEventTrigger {
        //     protected static ClingyEventType[] _supportedEventTypes = new ClingyEventType[] { 
        //            ClingyEventType.OnMouse0Down, ClingyEventType.OnMouse0Up,
        //            ClingyEventType.OnMouse1Down, ClingyEventType.OnMouse1Up };
        //     public override ClingyEventType[] supportedEventTypes { get { return _supportedEventTypes; } }
        //     public ClingyEvent OnMouse0Down { get { return GetOrCreateEvent(ClingyEventType.OnMouse0Down); } }
        //     public ClingyEvent OnMouse0Up { get { return GetOrCreateEvent(ClingyEventType.OnMouse0Up); } }
        //     public ClingyEvent OnMouse1Down { get { return GetOrCreateEvent(ClingyEventType.OnMouse1Down); } }
        //     public ClingyEvent OnMouse1Up { get { return GetOrCreateEvent(ClingyEventType.OnMouse1Up); } }
        // }
        // public ClingyMouseEventTrigger events = new ClingyMouseEventTrigger();

		void Update () {
			Vector3 mouseWorldPos = Input.mousePosition;
			mouseWorldPos.z = -Camera.main.transform.position.z;
			mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseWorldPos);
			transform.position = mouseWorldPos;

			// if (events.OnMouse0Down != null && Input.GetMouseButtonDown(0))
			// 	events.OnMouse0Down.Invoke(new ClingyEventInfo(ClingyEventType.OnMouse0Down, this));
			// if (events.OnMouse0Up != null && Input.GetMouseButtonUp(0))
			// 	events.OnMouse0Up.Invoke(new ClingyEventInfo(ClingyEventType.OnMouse0Up, this));
			// if (events.OnMouse1Down != null && Input.GetMouseButtonDown(1))
			// 	events.OnMouse1Down.Invoke(new ClingyEventInfo(ClingyEventType.OnMouse1Down, this));
			// if (events.OnMouse1Up != null && Input.GetMouseButtonUp(1))
			// 	events.OnMouse1Up.Invoke(new ClingyEventInfo(ClingyEventType.OnMouse1Up, this));
		}

	}

}