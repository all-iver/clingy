namespace SubC.Attachments {

	using UnityEngine.Events;

    public enum AttachEventType {
        // attachment events
        OnWillAttach,
        OnAttached,
        OnWillDetach,
        OnDetached,
        OnConnected,
        OnDisconnected,
        // per-object events
        OnObjectWillJoin,
        OnObjectWillConnect,
        OnObjectConnected,
        OnObjectWillDisconnect,
        OnObjectWillLeave,
        OnObjectLeft,
        // miscellaneous events
        OnMouse0Down,
        OnMouse0Up,
        OnMouse1Down,
        OnMouse1Up
    }

	[System.Serializable]
	public class AttachEvent : UnityEvent<AttachEventInfo> {}

}