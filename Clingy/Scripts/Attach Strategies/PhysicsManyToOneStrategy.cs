namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyPhysics;

	[CreateAssetMenu(fileName = "New Physics Many-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Physics Many-to-One Strategy")]
	public class PhysicsManyToOneStrategy : ManyToOneAttachStrategy {

        [System.Serializable]
        public class ManyToOneJointDescription : AttachStrategyJointDescription {
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector anchorParam;
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector connectedAnchorParam; 
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector axisParam;
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector swingAxisParam;
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector secondaryAxisParam;
            // [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaves })]
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
                anchorParam = ParamSelector.Position(provider: (int) Providers.Leaf);
                connectedAnchorParam = ParamSelector.Position(provider: (int) Providers.Root);
                axisParam = new ParamSelector(new Param(ParamType.Vector3, "axis"), (int) Providers.Root);
                secondaryAxisParam = new ParamSelector(new Param(Vector3.right, "secondaryAxis"), 
                        (int) Providers.Root);
                swingAxisParam = new ParamSelector(new Param(Vector3.right, "swingAxis"), (int) Providers.Root);
                // limitsParam = new ParamSelector(Param.defaultNameForType[ParamType.AngleLimits], 
                //         new Param(ParamType.AngleLimits), (int) Providers.Object1);
            }
        }

		public ManyToOneJointDescription[] jointDescriptions;

        public bool hideJointsInInspector = true;
		// public bool detachOnJointBreak = true;

        protected override void Reset() {
            base.Reset();
            jointDescriptions = new ManyToOneJointDescription[1];
            jointDescriptions[0] = new ManyToOneJointDescription();
            jointDescriptions[0].Reset();
        }

		protected override void ConnectLeaf(AttachObject root, AttachObject leaf) {
            PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(leaf, root, jointDescriptions, leaf, 
                    hideJointsInInspector);
        }

        protected override void DisconnectLeaf(AttachObject root, AttachObject leaf) {
            PhysicsAttachStrategyHelper.DestroyJoints(leaf);
        }

        void UpdateFromParamsAndApply(Attachment attachment) {
            AttachObject root = GetRoot(attachment);
            if (root == null || !root.isConnected)
                return;
            AttachObjectList.Enumerator e = attachment.objects.GetEnumerator(phase: AttachObjectPhase.Connected, 
                    category: (int) Categories.Leaves);
            while (e.MoveNext()) {
                AttachObject leaf = e.Current;
                PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(leaf, root, jointDescriptions, leaf, 
                        hideJointsInInspector);
            }
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