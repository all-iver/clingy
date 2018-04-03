namespace SubC.Attachments {

	using UnityEngine;
    using ClingyPhysics2D;

	[CreateAssetMenu(fileName = "New Physics2D One-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Physics2D One-to-One Strategy")]
	public class Physics2DOneToOneStrategy : OneToOneAttachStrategy {

        public new enum Categories {
            JointedObject = OneToOneAttachStrategy.Categories.Object1,
            ConnectedObject = OneToOneAttachStrategy.Categories.Object2
        }

        [System.Serializable]
        public class OneToOneJointDescription2D : AttachStrategyJointDescription2D {
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector anchorParam;
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector connectedAnchorParam; 
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector targetParam;
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector limitsParam;
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector distanceParam;

            public override ParamSelector GetAnchorParamSelector() {
                return anchorParam;
            }

            public override ParamSelector GetConnectedAnchorParamSelector() {
                return connectedAnchorParam;
            }

            public override ParamSelector GetTargetParamSelector() {
                return targetParam;
            }

            public override ParamSelector GetLimitsParamSelector() {
                return limitsParam;
            }

            public override ParamSelector GetDistanceParamSelector() {
                return distanceParam;
            }

            public override void Reset() {
                base.Reset();
                anchorParam = ParamSelector.Position(provider: (int) Providers.Object1);
                connectedAnchorParam = ParamSelector.Position(provider: (int) Providers.Object2);
                targetParam = ParamSelector.Position(name: "target", provider: (int) Providers.Object2);
                limitsParam = new ParamSelector(new Param(ParamType.AngleLimits), (int) Providers.Object1);
                distanceParam = new ParamSelector(new Param(ParamType.Float, "distance"), (int) Providers.Object1);
            }
        }

		public OneToOneJointDescription2D[] jointDescriptions;

        public bool hideJointsInInspector = true;
		// public bool detachOnJointBreak = true;

        public override string GetLabelForObject1() {
            return "Jointed Object";
        }

        public override string GetLabelForObject2() {
            return "Connected Object";
        }

        public AttachObject GetJointedObject(Attachment attachment) {
            return GetObject1(attachment);
        }

        public AttachObject GetConnectedObject(Attachment attachment) {
            return GetObject2(attachment);
        }

        protected override void Reset() {
            base.Reset();
            jointDescriptions = new OneToOneJointDescription2D[1];
            jointDescriptions[0] = new OneToOneJointDescription2D();
            jointDescriptions[0].Reset();
        }

		protected override void ConnectBoth(AttachObject jointed, AttachObject connected) {
            Physics2DAttachStrategyHelper.CreateOrApplyAllJoints(jointed, connected, jointDescriptions, 
                    null, hideJointsInInspector);
        }

        protected override void DisconnectBoth(AttachObject jointed, AttachObject connected) {
            Physics2DAttachStrategyHelper.DestroyJoints(jointed);
        }

        void UpdateFromParamsAndApply(Attachment attachment) {
            if (!IsConnected(attachment))
                return;
            AttachObject jointed = GetJointedObject(attachment);
            AttachObject connected = GetConnectedObject(attachment);
            // fixme - updating the anchors will also change a hinge joint's reference angle, which means the limits 
            // will change if they are based on the starting rotation. :/  need to only update anchors if they have
            // actually changed.
            Physics2DAttachStrategyHelper.CreateOrApplyAllJoints(jointed, connected, jointDescriptions, 
                    null, hideJointsInInspector);
        }

        public override void OnParamsUpdated(Attachment attachment) {
            UpdateFromParamsAndApply(attachment);
        }

        // public override void DoLateUpdate(Attachment attachment) {
        //     // todo - is this just for updating the target if keepTargetUpdated is true???
        // }

        public override void UpdateForEditorChanges(Attachment attachment) {
            UpdateFromParamsAndApply(attachment);
        }

	}

}