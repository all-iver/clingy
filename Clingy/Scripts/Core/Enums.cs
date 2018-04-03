namespace SubC.Attachments {

    public enum AttachmentPhase {
        Detached,
        Attaching,
        Attached,
        Detaching,
        Any = -1
    }

    public enum AttachObjectPhase {
        Initial,
        Joining,
        Connecting,
        Connected,
        Disconnecting,
        Leaving,
        Left,
        Any = -1
    }
    
}