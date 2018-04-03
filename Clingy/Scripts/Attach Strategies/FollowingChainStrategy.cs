namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyFollowing;

	[CreateAssetMenu(fileName = "New Following Chain Strategy", 
            menuName = "Clingy/Attach Strategies/Following Chain Strategy")]
	public class FollowingChainStrategy : ChainAttachStrategy {

        // note we can't allow NextLink as a provider in any operation that could be applied to the last link in the 
        // chain.  following intrinsically makes PreviousLink the target and NextLink the follower, so there's always
        // a PreviousLink except for at the first link, but we don't need to operate on that link anyway because it has
        // nothing to follow.  however, we do have to operate on the last link and in that case there's no NextLink
        // provider so we can't allow using it.  it CAN be used for transitioners in the Links category but it will 
        // fail if there is no object in the Tail category (same with PreviousLink if there is no Head object).  the 
        // Tail transitioner cannot use NextLink (and the Head transitioner cannot use PreviousLink).

        [System.Serializable]
        public class FollowingChainPositionOptions : AttachStrategyFollowPositionOptions {
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector followerAnchor, targetAnchor;

            public override ParamSelector GetFollowerAnchorParamSelector() {
                return followerAnchor;
            }

            public override ParamSelector GetTargetAnchorParamSelector() {
                return targetAnchor;
            }

            public override void Reset() {
                base.Reset();
                followerAnchor = ParamSelector.Position(provider: (int) Providers.Link);
                targetAnchor = ParamSelector.Position(provider: (int) Providers.PreviousLink);
            }
        }

        [System.Serializable]
        public class FollowingChainRotationOptions : AttachStrategyFollowRotationOptions {
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector rotationParam;
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector forwardParam;
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector upParam;
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
            public ParamSelector lookAtPositionParam;
            [ParamSelector(new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Link,
                    (int) Providers.Tail })]
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
                rotationParam = ParamSelector.Rotation(provider: (int) Providers.PreviousLink);
                forwardParam = ParamSelector.Direction(name: "forward", provider: (int) Providers.PreviousLink,
                        defaultValue: Vector3.forward);
                upParam = ParamSelector.Direction(name: "up", provider: (int) Providers.PreviousLink,
                        defaultValue: Vector3.up);
                lookAtPositionParam = ParamSelector.Position(provider: (int) Providers.PreviousLink);
                offsetParam = new ParamSelector(new Param(ParamType.Rotation, "offset"), (int) Providers.Link);
            }
        }
        
        public FollowingChainPositionOptions positionOptions = new FollowingChainPositionOptions();
        public FollowingChainRotationOptions rotationOptions = new FollowingChainRotationOptions();

        protected override void Reset() {
            base.Reset();
            positionOptions.Reset();
            rotationOptions.Reset();
        }

        FollowingObjectState GetObjectState(AttachObject obj) {
            FollowingObjectState state;
            if (obj.state == null) {
                state = new FollowingObjectState();
                obj.state = state;
            }
            state = (FollowingObjectState) obj.state;
            return state;
        }

        protected override void ConnectLinks(AttachObject target, AttachObject follower) {
        }

        protected override void DisconnectLinks(AttachObject target, AttachObject follower) {
        }

        public override void DoLateUpdate(Attachment attachment) {
            AttachObject target = null;
            for (int c = 0; c < GetCategories().Length; c++) {
                AttachObjectList.Enumerator e = attachment.objects.GetEnumerator(c);
                while (e.MoveNext()) {
                    AttachObject follower = e.Current;
                    if (target == null || !follower.isConnected) {
                        target = follower;
                        continue;
                    }
                    FollowingAttachStrategyUtility.FollowTarget(target, follower, positionOptions, rotationOptions);
                    target = follower;
                }
;            }
        }        

	}

}