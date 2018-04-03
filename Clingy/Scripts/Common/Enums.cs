namespace SubC.Attachments {

    public enum PositionBehavior {
        DoNothing,
        Snap
    }

	public enum RotationBehavior {
		DoNothing,
		LookAt,
		LookAt2D,
		LookDirection,
		Snap
	}
    
    public enum MoveMethod {
        Translate,
        SetPhysics,
        MovePhysics
    }

    public enum RotateMethod {
        Translate,
        SetPhysics,
        MovePhysics
    }

    public enum UpdateMethod {
        Update,
        FixedUpdate,
        LateUpdate
    }

    public enum AngleLimitsRelativeTo {
        StartingRotation,
        ZeroRotation
    }

}