namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;

	[CreateAssetMenu(fileName = "New Following Many-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Following Many-to-One Strategy")]
	public class FollowingManyToOneStrategy : ManyToOneAttachStrategy {

        public new enum Categories {
            Target = ManyToOneAttachStrategy.Categories.Root,
            Followers = ManyToOneAttachStrategy.Categories.Leaves
        }

        [System.Serializable]
        public class FollowingManyToOnePositionOptions : ClingyFollowing.AttachStrategyFollowPositionOptions {
            [ParamSelector(new int[] { (int) Categories.Target, (int) Categories.Followers })]
            public ParamSelector followerAnchor, targetAnchor;

            public override ParamSelector GetFollowerAnchorParamSelector() {
                return followerAnchor;
            }

            public override ParamSelector GetTargetAnchorParamSelector() {
                return targetAnchor;
            }

            public override void Reset() {
                base.Reset();
                followerAnchor = ParamSelector.Position(provider: (int) Providers.Leaf);
                targetAnchor = ParamSelector.Position(provider: (int) Providers.Root);
            }
        }

        [System.Serializable]
        public class FollowingManyToOneRotationOptions : ClingyFollowing.AttachStrategyFollowRotationOptions {
            [ParamSelector(new int[] { (int) Categories.Target, (int) Categories.Followers })]
            public ParamSelector rotationParam;
            [ParamSelector(new int[] { (int) Categories.Target, (int) Categories.Followers })]
            public ParamSelector forwardParam;
            [ParamSelector(new int[] { (int) Categories.Target, (int) Categories.Followers })]
            public ParamSelector upParam;
            [ParamSelector(new int[] { (int) Categories.Target, (int) Categories.Followers })]
            public ParamSelector lookAtPositionParam;
            [ParamSelector(new int[] { (int) Categories.Target, (int) Categories.Followers })]
            public ParamSelector offsetParam;

            public override ParamSelector GetRotationParamSelector() {
                return rotationParam;
            }

            public override ParamSelector GetForwardParamSelector() {
                return forwardParam;
            }

            public override ParamSelector GetUpParamSelector() {
                return upParam;
            }

            public override ParamSelector GetLookAtPositionParamSelector() {
                return lookAtPositionParam;
            }

            public override ParamSelector GetOffsetParamSelector() {
                return offsetParam;
            }

            public override void Reset() {
                base.Reset();
                rotationParam = ParamSelector.Rotation(provider: (int) Providers.Leaf);
                forwardParam = ParamSelector.Direction(defaultValue: Vector3.forward, provider: (int) Providers.Leaf);
                upParam = ParamSelector.Direction(defaultValue: Vector3.up, provider: (int) Providers.Leaf);
                lookAtPositionParam = ParamSelector.Position(provider: (int) Providers.Root);
                offsetParam = ParamSelector.Rotation("offset", (int) Providers.Leaf);
            }
        }
        
        public FollowingManyToOnePositionOptions positionOptions = new FollowingManyToOnePositionOptions();
        public FollowingManyToOneRotationOptions rotationOptions = new FollowingManyToOneRotationOptions();

        protected override void Reset() {
            base.Reset();
            positionOptions.Reset();
            rotationOptions.Reset();
        }

        public override string GetLabelForRoot() {
            return "Target";
        }

        public override string GetLabelForLeaves() {
            return "Followers";
        }

        public override string GetLabelForLeaf() {
            return "Follower";
        }

        AttachObject GetTarget(Attachment attachment) {
            return GetRoot(attachment);
        }

        protected override void ConnectLeaf(AttachObject root, AttachObject leaf) {
        }

        protected override void DisconnectLeaf(AttachObject root, AttachObject leaf) {
        }

        public override void DoLateUpdate(Attachment attachment) {
            AttachObject target = GetTarget(attachment);
            if (target == null || !target.isConnected)
                return;
            AttachObjectList.Enumerator e = attachment.objects.GetEnumerator(category: (int) Categories.Followers,
                    phase: AttachObjectPhase.Connected);
            while (e.MoveNext()) {
                AttachObject follower = e.Current;
                ClingyFollowing.FollowingAttachStrategyUtility.FollowTarget(target, follower, positionOptions, 
                        rotationOptions);
            }
        }

	}

}