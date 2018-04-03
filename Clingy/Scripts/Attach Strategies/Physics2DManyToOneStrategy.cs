namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyPhysics2D;

	[CreateAssetMenu(fileName = "New Physics2D Many-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Physics2D Many-to-One Strategy")]
	public class Physics2DManyToOneStrategy : ManyToOneAttachStrategy {

        [System.Serializable]
        public class ManyToOneJointDescription2D : AttachStrategyJointDescription2D {
            [ParamSelector(new int[] { (int) Categories.Root, (int) Categories.Leaves })]
            public ParamSelector anchorParam;
            [ParamSelector(new int[] { (int) Categories.Root, (int) Categories.Leaves })]
            public ParamSelector connectedAnchorParam; 
            [ParamSelector(new int[] { (int) Categories.Root, (int) Categories.Leaves })]
            public ParamSelector targetParam;
            [ParamSelector(new int[] { (int) Categories.Root, (int) Categories.Leaves })]
            public ParamSelector limitsParam;
            [ParamSelector(new int[] { (int) Categories.Root, (int) Categories.Leaves })]
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
                anchorParam = ParamSelector.Position(provider: (int) Providers.Leaf);
                connectedAnchorParam = ParamSelector.Position(provider: (int) Providers.Root);
                targetParam = ParamSelector.Position(provider: (int) Providers.Root);
                limitsParam = new ParamSelector(new Param(ParamType.AngleLimits), (int) Providers.Leaf);
                distanceParam = new ParamSelector(new Param(ParamType.Float), (int) Providers.Leaf);
            }
        }

		public ManyToOneJointDescription2D[] jointDescriptions;

        public bool hideJointsInInspector = true;
		// public bool detachOnJointBreak = true;

        protected override void Reset() {
            base.Reset();
            jointDescriptions = new ManyToOneJointDescription2D[1];
            jointDescriptions[0] = new ManyToOneJointDescription2D();
            jointDescriptions[0].Reset();
        }

		protected override void ConnectLeaf(AttachObject root, AttachObject leaf) {
            Physics2DAttachStrategyHelper.CreateOrApplyAllJoints(leaf, root, jointDescriptions, leaf, 
                    hideJointsInInspector);
        }

        protected override void DisconnectLeaf(AttachObject root, AttachObject leaf) {
            Physics2DAttachStrategyHelper.DestroyJoints(leaf);
        }

        void UpdateFromParamsAndApply(Attachment attachment) {
            AttachObject root = GetRoot(attachment);
            if (root == null || !root.isConnected)
                return;
            AttachObjectList.Enumerator e = attachment.objects.GetEnumerator(phase: AttachObjectPhase.Connected, 
                    category: (int) Categories.Leaves);
            while (e.MoveNext()) {
                AttachObject leaf = e.Current;
                // fixme - updating the anchors will also change a hinge joint's reference angle, which means the limits 
                // will change if they are based on the starting rotation. :/  need to only update anchors if they have
                // actually changed.
                Physics2DAttachStrategyHelper.CreateOrApplyAllJoints(leaf, root, jointDescriptions, leaf, 
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