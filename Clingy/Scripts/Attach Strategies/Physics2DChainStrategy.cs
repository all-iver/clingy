namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyPhysics2D;

	[CreateAssetMenuAttribute(fileName = "New Physics2D Chain Strategy", 
            menuName = "Clingy/Attach Strategies/Physics2D Chain Strategy")]
	public class Physics2DChainStrategy : ChainAttachStrategy {

        [System.Serializable]
        public class ChainJointDescription2D : AttachStrategyJointDescription2D {
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector anchorParam;
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector connectedAnchorParam; 
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector targetParam;
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
            public ParamSelector limitsParam;
            [ParamSelector(new int[] { (int) Providers.Link, (int) Providers.NextLink })]
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
                anchorParam = ParamSelector.Position(provider: (int) Providers.Link);
                connectedAnchorParam = ParamSelector.Position(provider: (int) Providers.NextLink);
                targetParam = ParamSelector.Position(provider: (int) Providers.NextLink);
                limitsParam = new ParamSelector(new Param(ParamType.AngleLimits), (int) Providers.Link);
                distanceParam = new ParamSelector(new Param(ParamType.Float, "distance"));
            }
        }

        public bool hideJointsInInspector = true;

		public ChainJointDescription2D[] jointDescriptions;

        protected override void Reset() {
            base.Reset();
            jointDescriptions = new ChainJointDescription2D[1];
            jointDescriptions[0] = new ChainJointDescription2D();
            jointDescriptions[0].Reset();
        }

		protected override void ConnectLinks(AttachObject link, AttachObject nextLink) {
            Physics2DAttachStrategyHelper.CreateOrApplyAllJoints(link, nextLink, jointDescriptions, 
                    link, hideJointsInInspector);
        }

        protected override void DisconnectLinks(AttachObject link, AttachObject nextLink) {
            Physics2DAttachStrategyHelper.DestroyJoints(link);
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
                Physics2DAttachStrategyHelper.CreateOrApplyAllJoints(link, nextLink, jointDescriptions, 
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