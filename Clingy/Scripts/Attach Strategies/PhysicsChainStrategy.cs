namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyPhysics;

	[CreateAssetMenuAttribute(fileName = "New Physics Chain Strategy", 
            menuName = "Clingy/Attach Strategies/Physics Chain Strategy")]
	public class PhysicsChainStrategy : ChainAttachStrategy {

        [System.Serializable]
        public class ChainJointDescription : AttachStrategyJointDescription {
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector anchorParam;
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector connectedAnchorParam; 
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector axisParam;
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector swingAxisParam;
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector secondaryAxisParam;
            // [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.Leaves })]
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
                anchorParam = ParamSelector.Position(provider: (int) Providers.Link);
                connectedAnchorParam = ParamSelector.Position(provider: (int) Providers.NextLink);
                axisParam = new ParamSelector(new Param(ParamType.Vector3, "axis"), (int) Providers.Link);
                secondaryAxisParam = new ParamSelector(new Param(Vector3.right, "secondaryAxis"), 
                        (int) Providers.Link);
                swingAxisParam = new ParamSelector(new Param(Vector3.right, "swingAxis"), (int) Providers.Link);
                // limitsParam = new ParamSelector(Param.defaultNameForType[ParamType.AngleLimits], 
                //         new Param(ParamType.AngleLimits), (int) Providers.Object1);
            }
        }

        public bool hideJointsInInspector = true;

		public ChainJointDescription[] jointDescriptions;

        protected override void Reset() {
            base.Reset();
            jointDescriptions = new ChainJointDescription[1];
            jointDescriptions[0] = new ChainJointDescription();
            jointDescriptions[0].Reset();
        }

		protected override void ConnectLinks(AttachObject link, AttachObject nextLink) {
            PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(link, nextLink, jointDescriptions, 
                    link, hideJointsInInspector);
        }

        protected override void DisconnectLinks(AttachObject link, AttachObject nextLink) {
            PhysicsAttachStrategyHelper.DestroyJoints(link);
        }

        void UpdateFromParamsAndApply(Attachment attachment) {
            // fixme - updating the anchors will also change a hinge joint's reference angle, which means the limits 
            // will change if they are based on the starting rotation. :/  need to only update anchors if they have
            // actually changed.
            AttachObjectList.Enumerator e = attachment.objects.GetEnumerator(phase: AttachObjectPhase.Connected);
            while (e.MoveNext()) {
                AttachObject link = e.Current;
                AttachObject nextLink = GetNextLink(link);
                if (nextLink == null || !nextLink.isConnected)
                    continue;
                PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(link, nextLink, jointDescriptions, 
                        link, hideJointsInInspector);
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