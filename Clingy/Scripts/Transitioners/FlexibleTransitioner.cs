namespace SubC.Attachments {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class FlexibleTransitioner : Transitioner {

        public const string displayName = "Flexible Transitioner";

        new public static string GetEditorClass() {
            return "SubC.Attachments.ClingyEditor.FlexibleTransitionerEditor";
        }

        public OnAttachOptions onAttachOptions = new OnAttachOptions();
        [Space]
        public SnapshotRestoreOptions onDetachOptions = new SnapshotRestoreOptions();

        public class FlexibleTransitionerState : TransitionerState {
            public Snapshot detachedSnapshot, attachedSnapshot;
            public bool finishedPosition, finishedRotation;
        }

        public override TransitionerState CreateEmptyState() {
            return new FlexibleTransitionerState();
        }

        public override void Reset(AttachStrategy strategy) {
            onAttachOptions.Reset();
            onDetachOptions.Reset();
        }

        void Cancel(AttachObject obj) {
            FlexibleTransitionerState state = (FlexibleTransitionerState) obj.transitionerState;
            if (state.attachedSnapshot != null && state.attachedSnapshot.tweening)
                state.attachedSnapshot.Cancel();
            if (state.detachedSnapshot != null && state.detachedSnapshot.tweening)
                state.detachedSnapshot.Cancel();
        }

        public override bool Join(AttachObject obj) {
            FlexibleTransitionerState state = (FlexibleTransitionerState) obj.transitionerState;
            if (state.detachedSnapshot == null)
                state.detachedSnapshot = Snapshot.TakeSnapshot(onDetachOptions, obj.attachable, obj.spriteRenderer);

            // convert our attach settings to snapshot-friendly restore options, and put our data in the snapshot
            state.attachedSnapshot = new Snapshot(obj.attachable, obj.spriteRenderer);
            state.attachedSnapshot.options.rigidbodyBehavior = (RigidbodyRestoreBehavior) 
                    onAttachOptions.rigidbodyBehavior;
            
            state.attachedSnapshot.options.restorePosition = onAttachOptions.positionOptions.behavior 
                    != PositionBehavior.DoNothing;
            state.attachedSnapshot.options.tweenPositionOptions = onAttachOptions.positionOptions.tweenOptions;
            state.attachedSnapshot.options.moveMethod = onAttachOptions.positionOptions.moveMethod;

            state.attachedSnapshot.options.restoreRotation = onAttachOptions.rotationOptions.behavior 
                    != RotationBehavior.DoNothing;
            state.attachedSnapshot.options.tweenRotationOptions = onAttachOptions.rotationOptions.tweenOptions;
            state.attachedSnapshot.options.rotateMethod = onAttachOptions.rotationOptions.rotateMethod;

            // state.attachedSnapshot.options.restoreSortingOrder = onAttachOptions.adoptSortingOrderOptions.enabled;
            // if (state.attachedSnapshot.options.restoreSortingOrder) {
            //     ParamSelector selector = onAttachOptions.adoptSortingOrderOptions.param;
            //     AttachObject provider = obj.ResolveProvider(selector);
            //     if (provider != null)
            //         state.attachedSnapshot.data.sortingOrder = provider.GetSortingOrder(selector.name, 
            //                 selector.defaultParam);
            // }
            // // todo - sorting layer?

            // state.attachedSnapshot.options.restoreFlipX = onAttachOptions.adoptFlipXOptions.enabled;
            // if (state.attachedSnapshot.options.restoreFlipX) {
            //     ParamSelector selector = onAttachOptions.adoptFlipXOptions.param;
            //     AttachObject provider = obj.ResolveProvider(selector);
            //     if (provider != null)
            //         state.attachedSnapshot.data.flipX = provider.GetFlipX(selector.name, selector.defaultParam);
            // }

            // state.attachedSnapshot.options.restoreFlipY = onAttachOptions.adoptFlipYOptions.enabled;
            // if (state.attachedSnapshot.options.restoreFlipY) {
            //     ParamSelector selector = onAttachOptions.adoptFlipYOptions.param;
            //     AttachObject provider = obj.ResolveProvider(selector);
            //     if (provider != null)
            //         state.attachedSnapshot.data.flipY = provider.GetFlipX(selector.name, selector.defaultParam);
            // }
            
            // state.attachedSnapshot.options.restoreLayer = onAttachOptions.adoptLayerOptions.enabled;
            // if (state.attachedSnapshot.options.restoreLayer) {
            //     ParamSelector selector = onAttachOptions.adoptLayerOptions.param;
            //     AttachObject provider = obj.ResolveProvider(selector);
            //     if (provider != null)
            //         state.attachedSnapshot.data.layer = obj.GetLayer(selector.name, selector.defaultParam);
            // }

            state.attachedSnapshot.Apply(
                GetDestinationPosition: () => { return GetDesiredPositionForObject(obj); }, 
                GetDestinationRotation: () => { return GetDesiredRotationForObject(obj); }, 
                SnapshotComplete: () => {
                    state.attachedSnapshot = null;
                    obj.SetJoined();
                }, tweenFirst: false); // fixme - make tweenFirst optional
            
            return false;
        }

        Vector3 GetDesiredPositionForObject(AttachObject obj) {
            PositionOptions options = onAttachOptions.positionOptions;
            return ClingyFollowing.FollowingUtility.GetDesiredPositionForObject(obj, options.behavior, 
                    options.anchor1Param, options.anchor2Param);
        }

        Quaternion GetDesiredRotationForObject(AttachObject obj) {
            RotationOptions options = onAttachOptions.rotationOptions;
            return ClingyFollowing.FollowingUtility.GetDesiredRotationForObject(obj, options.behavior, 
                    options.rotationParam, options.lookAtPositionParam, options.upParam, options.forwardParam, 
                    options.offsetParam);
        }

        public override bool Leave(AttachObject obj) {
            Cancel(obj);
            FlexibleTransitionerState state = (FlexibleTransitionerState) obj.transitionerState;
            if (state.detachedSnapshot == null)
                return true;
            state.detachedSnapshot.Apply(
                    SnapshotComplete: () => { 
                        obj.SetLeft();
                    },
                    tweenFirst: true);
            return false;
        }

        public override void LeaveImmediate(AttachObject obj) {
            Cancel(obj);
        }

        public override void DoUpdate(AttachObject obj) {
            ApplyContinuousOptions(obj);
        }

        void ApplyContinuousOptions(AttachObject obj) {
            if (!obj.spriteRenderer)
                return;
            // if (onAttachOptions.adoptSortingOrderOptions.enabled) {
            //     ParamSelector selector = onAttachOptions.adoptSortingOrderOptions.param;
            //     AttachObject provider = obj.ResolveProvider(selector);
            //     if (provider != null)
            //         obj.spriteRenderer.sortingOrder = provider.GetSortingOrder(selector.name, selector.defaultParam);
            // }
            // if (onAttachOptions.adoptFlipXOptions.enabled) {
            //     ParamSelector selector = onAttachOptions.adoptFlipXOptions.param;
            //     AttachObject provider = obj.ResolveProvider(selector);
            //     if (provider != null)
            //         obj.spriteRenderer.flipX = provider.GetFlipX(selector.name, selector.defaultParam);
            // }
            // if (onAttachOptions.adoptFlipYOptions.enabled) {
            //     ParamSelector selector = onAttachOptions.adoptFlipYOptions.param;
            //     AttachObject provider = obj.ResolveProvider(selector);
            //     if (provider != null)
            //         obj.spriteRenderer.flipY = provider.GetFlipY(selector.name, selector.defaultParam);
            // }
        }

    }

}