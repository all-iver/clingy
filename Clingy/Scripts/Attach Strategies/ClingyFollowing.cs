namespace SubC.Attachments.ClingyFollowing {

    using UnityEngine;

    [System.Serializable]
    public class FollowPositionOptions {
        public PositionBehavior behavior;
        public MoveMethod moveMethod;
        public float smoothing;

        public virtual void Reset() {
        }
    }

    [System.Serializable]
    public abstract class AttachStrategyFollowPositionOptions : FollowPositionOptions {
        public bool followX, followY, followZ;

        public abstract ParamSelector GetFollowerAnchorParamSelector();
        public abstract ParamSelector GetTargetAnchorParamSelector();
    }

    [System.Serializable]
    public class FollowRotationOptions {
        public RotationBehavior behavior;
        public RotateMethod rotateMethod;
        public float speed;

        public virtual void Reset() {
        }
    }

    [System.Serializable]
    public abstract class AttachStrategyFollowRotationOptions : FollowRotationOptions {
        public abstract ParamSelector GetRotationParamSelector();
        public abstract ParamSelector GetForwardParamSelector();
        public abstract ParamSelector GetUpParamSelector();
        public abstract ParamSelector GetLookAtPositionParamSelector();
        public abstract ParamSelector GetOffsetParamSelector();
    }

    public static class FollowingUtility {

        // returns a world position
        public static Vector3 GetDesiredPositionForObject(AttachObject obj, PositionBehavior behavior,
                ParamSelector anchor1, ParamSelector anchor2) {
            switch (behavior) {
                case PositionBehavior.DoNothing:
                    throw new System.InvalidOperationException("PositionBehavior is to do nothing");
                    // return obj.gameObject.transform.position;
                case PositionBehavior.Snap:
                    return GetSnapPosition(obj.attachable, obj.attachment, anchor1, anchor2, obj);
            }
            throw new System.NotImplementedException("Position behavior not implemented.");
        }

        // returns a world rotation
        public static Quaternion GetDesiredRotationForObject(AttachObject obj, RotationBehavior behavior,
                ParamSelector rotation, ParamSelector lookAtPosition, ParamSelector up, ParamSelector forward,
                ParamSelector offset) {
            Quaternion result;
            switch (behavior) {
                case RotationBehavior.DoNothing:
                    throw new System.InvalidOperationException("RotationBehavior is to do nothing");
                case RotationBehavior.Snap:
                    result = rotation.GetWorldRotation(obj.attachment, obj);
                    break;
                case RotationBehavior.LookAt:
                    result = GetLookAtRotation(obj.attachable, obj.attachment, lookAtPosition, up, obj);
                    break;
                case RotationBehavior.LookAt2D:
                    result = GetLookAt2DRotation(obj.attachable, obj.attachment, lookAtPosition, obj,
                            obj.spriteRenderer ? obj.spriteRenderer.flipX : false);
                    break;
                case RotationBehavior.LookDirection:
                    result = GetLookDirectionRotation(obj.attachable, obj.attachment, forward, up, obj);
                    break;
                default:
                    throw new System.NotImplementedException("Rotation behavior not implemented.");
            }
            Quaternion off = offset.GetWorldRotation(obj.attachment, obj);
            return result * off;
        }

        // returns a world position
        public static Vector3 GetSnapPosition(GameObject gameObjectToSnap, Attachment attachment, 
                ParamSelector anchor1, ParamSelector anchor2, AttachObject reference = null) {
            Vector3 worldAnchor1 = anchor1.GetWorldPosition(attachment, reference);
            Vector3 worldAnchor2 = anchor2.GetWorldPosition(attachment, reference);
			return worldAnchor2 - (worldAnchor1 - gameObjectToSnap.transform.position); 
        }

        // returns a world rotation
		public static Quaternion GetLookAt2DRotation(GameObject gameObjectToRotate, Attachment attachment, 
                ParamSelector lookAtPos, AttachObject reference = null, bool flipX = false) {
            Vector3 lookAtWorldPos = lookAtPos.GetWorldPosition(attachment, reference);
            if (gameObjectToRotate.transform.position == lookAtWorldPos)
                return gameObjectToRotate.transform.rotation;
            return ClingyUtils.LookAt2D(gameObjectToRotate.transform.position, lookAtWorldPos, flipX);
		}

        // returns a world rotation
		public static Quaternion GetLookAtRotation(GameObject gameObjectToRotate, Attachment attachment, 
                ParamSelector lookAtPos, ParamSelector up, AttachObject reference = null) {
            Vector3 lookAtWorldPos = lookAtPos.GetWorldPosition(attachment, reference);
            Vector3 worldUp = up.GetWorldDirection(attachment, reference);
            if (gameObjectToRotate.transform.position == lookAtWorldPos)
                return gameObjectToRotate.transform.rotation;
            return Quaternion.LookRotation(lookAtWorldPos - gameObjectToRotate.transform.position, worldUp);
		}

        // returns a world rotation
		public static Quaternion GetLookDirectionRotation(GameObject gameObjectToRotate, Attachment attachment, 
                ParamSelector forward, ParamSelector up, AttachObject reference = null) {
            Vector3 worldForward = forward.GetWorldDirection(attachment, reference);
            Vector3 worldUp = up.GetWorldDirection(attachment, reference);
            return Quaternion.LookRotation(worldForward, worldUp);
		}

    }

    public class FollowingObjectState : AttachObjectState{
        public Vector3 velocity;
    }

    public static class FollowingAttachStrategyUtility {

        static FollowingObjectState GetObjectState(AttachObject obj) {
            FollowingObjectState state;
            if (obj.state == null) {
                state = new FollowingObjectState();
                obj.state = state;
            }
            state = (FollowingObjectState) obj.state;
            return state;
        }

        public static void FollowTarget(AttachObject target, AttachObject follower,
                AttachStrategyFollowPositionOptions positionOptions,
                AttachStrategyFollowRotationOptions rotationOptions) {
            if (rotationOptions.behavior != RotationBehavior.DoNothing) {
                Quaternion desiredRotation = FollowingUtility.GetDesiredRotationForObject(follower, 
                        rotationOptions.behavior, rotationOptions.GetRotationParamSelector(),
                        rotationOptions.GetLookAtPositionParamSelector(), rotationOptions.GetUpParamSelector(),
                        rotationOptions.GetForwardParamSelector(), rotationOptions.GetOffsetParamSelector());
                Quaternion nextRotation;
                if (rotationOptions.speed == 0)
                    nextRotation = desiredRotation;
                else
                    nextRotation = Quaternion.RotateTowards(follower.attachable.transform.rotation, desiredRotation,
                            Time.deltaTime * rotationOptions.speed);
                ClingyUtils.MoveRotation(nextRotation, follower.attachable.transform, rotationOptions.rotateMethod,
                        follower.rigidbody, follower.rigidbody2D);
            }
            // todo - move before rotate, rotate before move...it's situationally dependent
            if (positionOptions.behavior != PositionBehavior.DoNothing) {
                FollowingObjectState state = GetObjectState(follower);
                Vector3 desiredPos = FollowingUtility.GetDesiredPositionForObject(follower, positionOptions.behavior,
                        positionOptions.GetFollowerAnchorParamSelector(),
                        positionOptions.GetTargetAnchorParamSelector());
                if (!positionOptions.followX)
                    desiredPos.x = follower.attachable.transform.position.x;
                if (!positionOptions.followY)
                    desiredPos.y = follower.attachable.transform.position.y;
                if (!positionOptions.followZ)
                    desiredPos.z = follower.attachable.transform.position.z;
                Vector3 nextPos;
                if (positionOptions.smoothing == 0)
                    nextPos = desiredPos;
                else
                    nextPos = Vector3.SmoothDamp(follower.attachable.transform.position, desiredPos,
                            ref state.velocity, positionOptions.smoothing);
                ClingyUtils.MovePosition(nextPos, follower.attachable.transform, positionOptions.moveMethod,
                        follower.rigidbody, follower.rigidbody2D);
            }
            // // note this sets localScale to lossyScale, so only works if there's no parent scale on the attacher
            // if (followScale)
            //     context.attacher.transform.localScale = context.receiver.transform.lossyScale;
        }

    }

}