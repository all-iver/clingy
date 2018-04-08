namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyPhysics;

	[CreateAssetMenuAttribute(fileName = "New Physics Chain Strategy", 
            menuName = "Clingy/Attach Strategies/Physics Chain Strategy")]
	public class PhysicsChainStrategy : ChainAttachStrategy {

        [System.Serializable]
        public class ChainJointDescription : AttachStrategyJointDescription {
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector anchorParam;
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector connectedAnchorParam; 
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector axisParam;
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector swingAxisParam;
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
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
                connectedAnchorParam = ParamSelector.Position(provider: (int) Providers.PreviousLink);
                axisParam = ParamSelector.Direction("axis", (int) Providers.PreviousLink, Vector3.forward);
                secondaryAxisParam = ParamSelector.Direction("secondaryAxis", (int) Providers.PreviousLink,
                        Vector3.right);
                swingAxisParam = ParamSelector.Direction("swingAxis", (int) Providers.PreviousLink, Vector3.right);
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
            // put the joint on nextLink - so it is also the reference object
            PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(nextLink, link, jointDescriptions, 
                    nextLink, hideJointsInInspector);
        }

        protected override void DisconnectLinks(AttachObject link, AttachObject nextLink) {
            PhysicsAttachStrategyHelper.DestroyJoints(nextLink);
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
                // put the joint on nextLink - so it is also the reference object
                PhysicsAttachStrategyHelper.CreateOrApplyAllJoints(nextLink, link, jointDescriptions, 
                        nextLink, hideJointsInInspector);
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