namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyPhysics;

	[CreateAssetMenu(fileName = "New Physics One-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Physics One-to-One Strategy")]
	public class PhysicsOneToOneStrategy : OneToOneAttachStrategy {

        public new enum Categories {
            JointedObject = OneToOneAttachStrategy.Categories.Object1,
            ConnectedObject = OneToOneAttachStrategy.Categories.Object2
        }

        [System.Serializable]
        public class OneToOneJointDescription : AttachStrategyJointDescription {
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector anchorParam;
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector connectedAnchorParam; 
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector axisParam;
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector swingAxisParam;
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector secondaryAxisParam;
            // [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            // public ParamSelector limitsParam;

            public override ParamSelector GetAnchorParamSelector() {
                return anchorParam;
            }

            public override ParamSelector GetConnectedAnchorParamSelector() {
                return connectedAnchorParam;
            }

            public override ParamSelector GetAxisParamSelector() {
                return axisParam;
            }

            public override ParamSelector GetSecondaryAxisParamSelector() {
                return secondaryAxisParam;
            }

            public override ParamSelector GetSwingAxisParamSelector() {
                return swingAxisParam;
            }

            public override void Reset() {
                base.Reset();
                anchorParam = ParamSelector.Position(provider: (int) Providers.Object1);
                connectedAnchorParam = ParamSelector.Position(provider: (int) Providers.Object2);
                axisParam = new ParamSelector(new Param(ParamType.Vector3, "axis"), (int) Providers.Object1);
                secondaryAxisParam = new ParamSelector(new Param(Vector3.right, "secondaryAxis"), 
                        (int) Providers.Object1);
                swingAxisParam = new ParamSelector(new Param(Vector3.right, "swingAxis"), (int) Providers.Object1);
                // limitsParam = new ParamSelector(Param.defaultNameForType[ParamType.AngleLimits], 
                //         new Param(ParamType.AngleLimits), (int) Providers.Object1);
            }
        }

		public OneToOneJointDescription[] jointDescriptions;

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
            jointDescriptions = new OneToOneJointDescription[1];
            jointDescriptions[0] = new OneToOneJointDescription();
            jointDescriptions[0].Reset();
        }

		protected override void ConnectBoth(AttachObject jointed, AttachObject connected) {
            PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(jointed, connected, jointDescriptions, 
                    null, hideJointsInInspector);
        }

        protected override void DisconnectBoth(AttachObject jointed, AttachObject connected) {
            PhysicsAttachStrategyHelper.DestroyJoints(jointed);
        }

        void UpdateFromParamsAndApply(Attachment attachment) {
            if (!IsConnected(attachment))
                return;
            AttachObject jointed = GetJointedObject(attachment);
            AttachObject connected = GetConnectedObject(attachment);
            // fixme - need to only update anchors if they have actually changed (same as 2D)?
            PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(jointed, connected, jointDescriptions, 
                    null, hideJointsInInspector);
        }

        public override void OnParamsUpdated(Attachment attachment) {
            UpdateFromParamsAndApply(attachment);
        }

        public override void UpdateForEditorChanges(Attachment attachment) {
            UpdateFromParamsAndApply(attachment);
        }

	}

}