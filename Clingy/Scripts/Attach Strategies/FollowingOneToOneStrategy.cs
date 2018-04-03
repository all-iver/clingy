namespace SubC.Attachments {

	using UnityEngine;
    using SubC.Attachments.ClingyFollowing;

	[CreateAssetMenu(fileName = "New Following One-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Following One-to-One Strategy")]
	public class FollowingOneToOneStrategy : OneToOneAttachStrategy {

        public new enum Categories {
            Target = OneToOneAttachStrategy.Categories.Object1,
            Follower = OneToOneAttachStrategy.Categories.Object2
        }

        public new enum Providers {
            Target = OneToOneAttachStrategy.Providers.Object1,
            Follower = OneToOneAttachStrategy.Providers.Object2
        }

        [System.Serializable]
        public class FollowingOneToOnePositionOptions : AttachStrategyFollowPositionOptions {
            [ParamSelector(new int[] { (int) Providers.Target, (int) Providers.Follower })]
            public ParamSelector followerAnchor, targetAnchor;

            public override ParamSelector GetFollowerAnchorParamSelector() {
                return followerAnchor;
            }

            public override ParamSelector GetTargetAnchorParamSelector() {
                return targetAnchor;
            }

            public override void Reset() {
                base.Reset();
                followerAnchor = ParamSelector.Position(provider: (int) Providers.Follower);
                targetAnchor = ParamSelector.Position(provider: (int) Providers.Target);
            }
        }

        [System.Serializable]
        public class FollowingOneToOneRotationOptions : AttachStrategyFollowRotationOptions {
            [ParamSelector(new int[] { (int) Providers.Target, (int) Providers.Follower })]
            public ParamSelector rotationParam;
            [ParamSelector(new int[] { (int) Providers.Target, (int) Providers.Follower })]
            public ParamSelector forwardParam;
            [ParamSelector(new int[] { (int) Providers.Target, (int) Providers.Follower })]
            public ParamSelector upParam;
            [ParamSelector(new int[] { (int) Providers.Target, (int) Providers.Follower })]
            public ParamSelector lookAtPositionParam;
            [ParamSelector(new int[] { (int) Providers.Target, (int) Providers.Follower })]
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
                rotationParam = ParamSelector.Rotation("rotation", (int) Providers.Target);
                forwardParam = ParamSelector.Direction("forward", (int) Providers.Target, Vector3.forward);
                upParam = ParamSelector.Direction("up", (int) Providers.Target, Vector3.up);
                lookAtPositionParam = ParamSelector.Position("position", (int) Providers.Target);
                offsetParam = new ParamSelector(new Param(ParamType.Rotation, "offset"), (int) Providers.Follower);
            }
        }
        
        public FollowingOneToOnePositionOptions positionOptions = new FollowingOneToOnePositionOptions();
        public FollowingOneToOneRotationOptions rotationOptions = new FollowingOneToOneRotationOptions();

        protected override void Reset() {
            base.Reset();
            positionOptions.Reset();
            rotationOptions.Reset();
        }

        public override string GetLabelForObject1() {
            return "Target";
        }

        public override string GetLabelForObject2() {
            return "Follower";
        }

        AttachObject GetTarget(Attachment attachment) {
            return GetObject1(attachment);
        }

        AttachObject GetFollower(Attachment attachment) {
            return GetObject2(attachment);
        }

        protected override void ConnectBoth(AttachObject target, AttachObject follower) {
            FollowingAttachStrategyUtility.FollowTarget(target, follower, positionOptions, rotationOptions);
        }

        protected override void DisconnectBoth(AttachObject target, AttachObject follower) {
        }

        public override void DoLateUpdate(Attachment attachment) {
            if (!IsConnected(attachment))
                return;
            AttachObject target = GetTarget(attachment);
            AttachObject follower = GetFollower(attachment);
            FollowingAttachStrategyUtility.FollowTarget(target, follower, positionOptions, rotationOptions);
        }

	}

}