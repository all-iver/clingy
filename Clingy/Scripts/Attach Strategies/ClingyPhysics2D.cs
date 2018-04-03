namespace SubC.Attachments.ClingyPhysics2D {

    using UnityEngine;
    using System.Collections.Generic;

    [System.Serializable]
    public struct SerializableJointMotor2D {
        public float motorSpeed, maxMotorTorque;

        public void Reset() {
            motorSpeed = 0;
            maxMotorTorque = 10000;
        }
    }

    public enum JointType2D {
        DistanceJoint2D = 5, 
        FixedJoint2D = 10, 
        FrictionJoint2D = 15, 
        HingeJoint2D = 20, 
        RelativeJoint2D = 25,
        SliderJoint2D = 30,
        SpringJoint2D = 35,
        TargetJoint2D = 40,
        WheelJoint2D = 45
    }

    [System.Serializable]
    public class JointDescription2D {
        public Vector2 anchor, connectedAnchor, target;
        public JointType2D jointType;
        public bool useLimits;
        public AngleLimits limits = new AngleLimits();
        public SerializableJointMotor2D motor;
        public bool useMotor;
        public float distance;
        public bool maxDistanceOnly;
        public bool autoConfigureConnectedAnchor, autoConfigureDistance, autoConfigureOffset;
        public float maxForce, maxTorque; // for relative/target joint
        public float maxFrictionForce, maxFrictionTorque; // for friction joint
        public float correctionScale;
        public float dampingRatio, frequency;
        public bool enableCollision;
        public float breakForce, breakTorque;
        public Vector2 linearOffset;
        public float angularOffset;

        public JointDescription2D() {
            Reset();
        }

        public virtual void Reset() {
            jointType = JointType2D.FixedJoint2D;
            motor.Reset();
            anchor = connectedAnchor = target = Vector2.zero;
            limits = new AngleLimits();
            autoConfigureDistance = true;
            autoConfigureOffset = true;
            maxForce = 10000;
            maxTorque = 10000;
            maxFrictionForce = 1;
            maxFrictionTorque = 1;
            correctionScale = 0.3f;
            frequency = 1;
            breakForce = Mathf.Infinity;
            breakTorque = Mathf.Infinity;
        }

        void ConfigureJoint(Joint2D joint, Rigidbody2D connectedBody) {
            joint.enableCollision = enableCollision;
            joint.connectedBody = connectedBody;
            if (breakForce != Mathf.Infinity)
                joint.breakForce = breakForce;
            if (breakTorque != Mathf.Infinity)
                joint.breakTorque = breakTorque;
        }

        void ConfigureAnchoredJoint(AnchoredJoint2D joint, Rigidbody2D connectedBody) {
            ConfigureJoint(joint, connectedBody);
            joint.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
            joint.connectedAnchor = connectedAnchor;
            joint.anchor = anchor;
        }

        bool JointMatchesJointType(Joint2D joint) {
            if (!joint)
                return false;
            if (jointType == JointType2D.DistanceJoint2D)
                return joint.GetType() == typeof(DistanceJoint2D);
            if (jointType == JointType2D.FixedJoint2D)
                return joint.GetType() == typeof(FixedJoint2D);
            if (jointType == JointType2D.FrictionJoint2D)
                return joint.GetType() == typeof(FrictionJoint2D);
            if (jointType == JointType2D.HingeJoint2D)
                return joint.GetType() == typeof(HingeJoint2D);
            if (jointType == JointType2D.RelativeJoint2D)
                return joint.GetType() == typeof(RelativeJoint2D);
            if (jointType == JointType2D.SliderJoint2D)
                return joint.GetType() == typeof(SliderJoint2D);
            if (jointType == JointType2D.SpringJoint2D)
                return joint.GetType() == typeof(SpringJoint2D);
            if (jointType == JointType2D.TargetJoint2D)
                return joint.GetType() == typeof(TargetJoint2D);
            if (jointType == JointType2D.WheelJoint2D)
                return joint.GetType() == typeof(WheelJoint2D);
            throw new System.NotImplementedException("Unrecognized joint type.");
        }

        // public void ApplyAnchors(Joint2D joint) {
        // 	if (jointType == JointType2D.RelativeJoint2D)
        // 		return;
        // 	if (jointType == JointType2D.TargetJoint2D) {
        // 		((TargetJoint2D) joint).anchor = anchor;
        // 		((TargetJoint2D) joint).target = target; // needs to be world relative
        // 		return;
        // 	}
        // 	((AnchoredJoint2D) joint).anchor = anchor;
        // 	((AnchoredJoint2D) joint).connectedAnchor = connectedAnchor;			
        // }

        // public void ApplyLimits(Joint2D joint, GameObject go, Rigidbody2D connectedBody) {
        //     if (jointType != JointType2D.HingeJoint2D)
        //         return;
        //     Quaternion startRotation = go.transform.rotation;
        //     if (useLimits && limits.relativeTo == AngleLimitsRelativeTo.ZeroRotation && connectedBody) {
        //         go.transform.rotation = connectedBody.transform.rotation;
        //         // setting the connectedBody triggers a re-calculation of the reference angle
        //         joint.connectedBody = connectedBody;
        //     }
        //     ((HingeJoint2D) joint).limits = limits.ToJointAngleLimits2D();
        //     if (useLimits && limits.relativeTo == AngleLimitsRelativeTo.ZeroRotation && connectedBody)
        //         go.transform.rotation = startRotation;
        // }

        public Joint2D CreateOrApply(GameObject go, Rigidbody2D connectedBody, Joint2D joint = null) {
            if (joint && !JointMatchesJointType(joint)) {
                GameObject.Destroy(joint);
                joint = null;
            }
            if (jointType == JointType2D.DistanceJoint2D) {
                DistanceJoint2D distanceJoint = (DistanceJoint2D) joint ?? go.AddComponent<DistanceJoint2D>();
                ConfigureAnchoredJoint(distanceJoint, connectedBody);
                distanceJoint.maxDistanceOnly = maxDistanceOnly;
                distanceJoint.autoConfigureDistance = autoConfigureDistance;
                if (!autoConfigureDistance)
                    distanceJoint.distance = distance;
                joint = distanceJoint;
            }
            if (jointType == JointType2D.FixedJoint2D) {
                FixedJoint2D fixedJoint = (FixedJoint2D) joint ?? go.AddComponent<FixedJoint2D>();
                ConfigureAnchoredJoint(fixedJoint, connectedBody);
                fixedJoint.frequency = frequency;
                fixedJoint.dampingRatio = dampingRatio;
                joint = fixedJoint;
            }
            if (jointType == JointType2D.FrictionJoint2D) {
                FrictionJoint2D frictionJoint = (FrictionJoint2D) joint ?? go.AddComponent<FrictionJoint2D>();
                ConfigureAnchoredJoint(frictionJoint, connectedBody);
                frictionJoint.maxForce = maxFrictionForce;
                frictionJoint.maxTorque = maxFrictionTorque;
                joint = frictionJoint;
            }
            if (jointType == JointType2D.HingeJoint2D) {
                Quaternion startRotation = go.transform.rotation;
                // if (useLimits && limits.relativeTo == AngleLimitsRelativeTo.ZeroRotation && connectedBody)
                //     go.transform.rotation = connectedBody.transform.rotation;
                HingeJoint2D hingeJoint = (HingeJoint2D) joint ?? go.AddComponent<HingeJoint2D>();
                ConfigureAnchoredJoint(hingeJoint, connectedBody);
                hingeJoint.limits = limits.ToJointAngleLimits2D();
                hingeJoint.useLimits = useLimits;
                JointMotor2D m = new JointMotor2D();
                m.maxMotorTorque = motor.maxMotorTorque;
                m.motorSpeed = motor.motorSpeed;
                hingeJoint.motor = m;
                hingeJoint.useMotor = useMotor;
                joint = hingeJoint;
                // if (useLimits && limits.relativeTo == AngleLimitsRelativeTo.ZeroRotation && connectedBody)
                //     go.transform.rotation = startRotation;
            }
            if (jointType == JointType2D.RelativeJoint2D) {
                RelativeJoint2D relativeJoint = (RelativeJoint2D) joint ?? go.AddComponent<RelativeJoint2D>();
                ConfigureJoint(relativeJoint, connectedBody);
                relativeJoint.autoConfigureOffset = autoConfigureOffset;
                if (!autoConfigureOffset) {
                    relativeJoint.linearOffset = linearOffset;
                    relativeJoint.angularOffset = angularOffset;
                }
                relativeJoint.correctionScale = correctionScale;
                relativeJoint.maxForce = maxForce;
                relativeJoint.maxTorque = maxTorque;
                joint = relativeJoint;
            }
            if (jointType == JointType2D.SliderJoint2D) {
                SliderJoint2D sliderJoint = (SliderJoint2D) joint ?? go.AddComponent<SliderJoint2D>();
                ConfigureAnchoredJoint(sliderJoint, connectedBody);
                joint = sliderJoint;
            }
            if (jointType == JointType2D.SpringJoint2D) {
                SpringJoint2D springJoint = (SpringJoint2D) joint ?? go.AddComponent<SpringJoint2D>();
                ConfigureAnchoredJoint(springJoint, connectedBody);
                springJoint.autoConfigureDistance = autoConfigureDistance;
                if (!autoConfigureDistance)
                    springJoint.distance = distance;
                springJoint.dampingRatio = dampingRatio;
                springJoint.frequency = frequency;
                joint = springJoint;
            }
            if (jointType == JointType2D.TargetJoint2D) {
                TargetJoint2D targetJoint = (TargetJoint2D) joint ?? go.AddComponent<TargetJoint2D>();
                targetJoint.autoConfigureTarget = false;
                targetJoint.anchor = anchor;
                targetJoint.target = target;
                targetJoint.maxForce = maxForce;
                targetJoint.dampingRatio = dampingRatio;
                targetJoint.frequency = frequency;
                joint = targetJoint;
            }
            if (jointType == JointType2D.WheelJoint2D) {
                WheelJoint2D wheelJoint = (WheelJoint2D) joint ?? go.AddComponent<WheelJoint2D>();
                ConfigureJoint(wheelJoint, connectedBody);
                joint = wheelJoint;
            }
            return joint;
        }
    }

    [System.Serializable]
    public abstract class AttachStrategyJointDescription2D : JointDescription2D {

        public abstract ParamSelector GetAnchorParamSelector();

        public abstract ParamSelector GetConnectedAnchorParamSelector();

        public abstract ParamSelector GetTargetParamSelector();

        public abstract ParamSelector GetLimitsParamSelector();

        public abstract ParamSelector GetDistanceParamSelector();

    }

    public class Physics2DObjectState : AttachObjectState {
        public Joint2D[] joints;
    }

    public static class Physics2DAttachStrategyHelper {

        public static Physics2DObjectState GetObjectState(AttachObject obj) {
            Physics2DObjectState state;
            if (obj.state == null) {
                state = new Physics2DObjectState();
                obj.state = state;
            }
            state = (Physics2DObjectState) obj.state;
            return state;
        }

        public static void DestroyJoints(AttachObject obj) {
            Physics2DObjectState state = GetObjectState(obj);
            for (int i = 0; i < state.joints.Length; i++) {
                if (state.joints[i]) {
                    GameObject.Destroy(state.joints[i]);
                    state.joints[i] = null;
                }
            }
        }

        static void UpdateJointDescriptionFromParams(AttachStrategyJointDescription2D jd, AttachObject jointed, 
                AttachObject connected, AttachObject reference = null) {
            Attachment attachment = jointed.attachment;

            ParamSelector selector;
            selector = jd.GetAnchorParamSelector();
            // anchors must be relative to the associated Rigidbody's transform
            jd.anchor = jointed.rigidbody2D.transform.InverseTransformPoint(
                    selector.GetWorldPosition(attachment, reference));

            selector = jd.GetConnectedAnchorParamSelector();
            // anchors must be relative to the associated Rigidbody's transform
            jd.connectedAnchor = connected.rigidbody2D.transform.InverseTransformPoint(
                    selector.GetWorldPosition(attachment, reference));

            selector = jd.GetTargetParamSelector();
            jd.target = selector.GetWorldPosition(attachment, reference);

            // selector = jd.GetLimitsParamSelector();
            // jd.limits = selector.get
            // provider = reference != null ? attachment.attachStrategy.ResolveProvider(selector.provider, reference)
            //         : attachment.attachStrategy.ResolveProvider(selector.provider, attachment);
            // jd.limits = provider.resolvedParams.GetParam(ParamType.AngleLimits, selector.name, selector.defaultParam)
            //         .angleLimitsValue;

            selector = jd.GetDistanceParamSelector();
            jd.distance = selector.GetParam(attachment, reference).floatValue;
        }

        // public static void UpdateAllJointDescriptionsFromParams(AttachObject jointed, AttachObject connected, 
        //         AttachStrategyJointDescription2D[] jointDescriptions) {
        //     Physics2DObjectState jointedState = GetObjectState(jointed);
        //     Physics2DObjectState connectedState = GetObjectState(connected);
        //     int i = 0;
        //     foreach (AttachStrategyJointDescription2D jd in jointDescriptions) {
        //         UpdateJointDescriptionFromParams(jd, jointed, connected);
        //         // jd.ApplyAnchors(jointedState.joints[i]);
        //         // jd.ApplyLimits(jointedState.joints[i], jointed.gameObject, connected.rigidbody2D);
        //         i ++;
        //     }
        // }

        public static void CreateOrApplyAllJoints(AttachObject jointed, AttachObject connected, 
                AttachStrategyJointDescription2D[] jointDescriptions, AttachObject reference = null, 
                bool hideJointsInInspector = true) {
            Physics2DObjectState jointedState = GetObjectState(jointed);
            if (jointedState.joints == null || jointedState.joints.Length != jointDescriptions.Length)
                jointedState.joints = new Joint2D[jointDescriptions.Length];
            int i = 0;
            foreach (AttachStrategyJointDescription2D jd in jointDescriptions) {
                UpdateJointDescriptionFromParams(jd, jointed, connected, reference);
                jointedState.joints[i] = jd.CreateOrApply(jointed.rigidbody2D.gameObject, connected.rigidbody2D, 
                        joint: jointedState.joints[i]);
                jointedState.joints[i].hideFlags = hideJointsInInspector ? HideFlags.HideInInspector : HideFlags.None;
                i ++;
            }
        }

    }

}