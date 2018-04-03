namespace SubC.Attachments {

    using UnityEngine;

	public enum RigidbodyAttachBehavior {
		DoNothing = 0,
		Desimulate = 1,
		Destroy = 2,
        // skip one for restore so we can cast directly to RigidbodyRestoreBehavior
		SetDynamic = 4,
		SetKinematic = 5
	}

    [System.Serializable]
    public struct AdoptFlipXOptions {
        public bool enabled;
        [ParamSelector]
        public ParamSelector param;
        public bool keepUpdated;

        public void Reset() {
            enabled = false;
            param = new ParamSelector(new Param(ParamType.Bool));
            keepUpdated = true;
        }
    }

    [System.Serializable]
    public struct AdoptFlipYOptions {
        public bool enabled;
        [ParamSelector]
        public ParamSelector param;
        public bool keepUpdated;

        public void Reset() {
            enabled = false;
            param = new ParamSelector(new Param(ParamType.Bool));
            keepUpdated = true;
        }
    }

    [System.Serializable]
    public struct AdoptSortingOrderOptions {
        public bool enabled;
        [ParamSelector]
        public ParamSelector param;
        public bool keepUpdated;

        public void Reset() {
            enabled = false;
            param = new ParamSelector(new Param(ParamType.Integer));
            keepUpdated = true;
        }
    }

    [System.Serializable]
    public struct AdoptLayerOptions {
        public bool enabled;
        [ParamSelector]
        public ParamSelector param;

        public void Reset() {
            enabled = false;
            param = new ParamSelector(new Param(ParamType.Layer));
        }
    }

	[System.Serializable]
	public struct OnAttachOptions {
		public RigidbodyAttachBehavior rigidbodyBehavior;
		public PositionOptions positionOptions;
        public RotationOptions rotationOptions;
        [AdoptOption]
		public AdoptSortingOrderOptions adoptSortingOrderOptions;
        [AdoptOption]
		public AdoptFlipXOptions adoptFlipXOptions;
        [AdoptOption]
		public AdoptFlipYOptions adoptFlipYOptions;
        [AdoptOption(false)]
		public AdoptLayerOptions adoptLayerOptions;

        public void Reset() {
            positionOptions.Reset();
            rotationOptions.Reset();
            adoptSortingOrderOptions.Reset();
            adoptFlipXOptions.Reset();
            adoptFlipYOptions.Reset();
            adoptLayerOptions.Reset();
        }
	}

    [System.Serializable]
    public struct PositionOptions {
        public PositionBehavior behavior;
        [ParamSelector]
        public ParamSelector anchor1Param, anchor2Param;
        public MoveMethod moveMethod;
        public TweenOptions tweenOptions;

        public void Reset() {
            anchor1Param = ParamSelector.Position();
            anchor2Param = ParamSelector.Position();
            tweenOptions.Reset();
        }
    }

    [System.Serializable]
    public struct RotationOptions {
        public RotationBehavior behavior;
        [ParamSelector]
        public ParamSelector rotationParam, forwardParam, upParam, lookAtPositionParam, offsetParam;
        public RotateMethod rotateMethod;
        public TweenOptions tweenOptions;

        public void Reset() {
            lookAtPositionParam = ParamSelector.Position();
            forwardParam = ParamSelector.Direction("forward", defaultValue: Vector3.forward);
            upParam = ParamSelector.Direction("up", defaultValue: Vector3.up);
            rotationParam = ParamSelector.Rotation();
            offsetParam = ParamSelector.Rotation("offset");
            tweenOptions.Reset();
        }
    }

}