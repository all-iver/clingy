namespace SubC.Attachments {

	public enum RigidbodyRestoreBehavior {
		DoNothing = 0,
		Desimulate = 1,
		Destroy = 2,
		Restore = 3,
		SetDynamic = 4,
		SetKinematic = 5
	}

	[System.Serializable]
	public struct SnapshotRestoreOptions {
		public RigidbodyRestoreBehavior rigidbodyBehavior;
		public bool restorePosition;
		public bool restoreRotation;
		public bool restoreScale;
		public bool restoreLayer;
		public bool restoreSortingOrder;
		public bool restoreFlipX;
		public bool restoreFlipY;

        public MoveMethod moveMethod;
        public RotateMethod rotateMethod;
        public TweenOptions tweenPositionOptions, tweenRotationOptions;
		
        public void Reset() {
            tweenPositionOptions.Reset();
            tweenRotationOptions.Reset();
        }
	}

}