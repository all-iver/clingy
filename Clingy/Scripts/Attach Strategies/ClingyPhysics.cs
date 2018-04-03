namespace SubC.Attachments.ClingyPhysics {

    using UnityEngine;

	public enum JointType {
		CharacterJoint = 5, 
		ConfigurableJoint = 10, 
		FixedJoint = 15,
		HingeJoint = 20,
		SpringJoint = 25
	}

	[System.Serializable]
	public struct SerializableJointSpring {
		public float spring;
		public float damper;
		public float targetPosition;

        public void Reset() {
            spring = 10f;
            damper = 0.2f;
        }

		public JointSpring ToJointSpring() {
			JointSpring s = new JointSpring();
			s.damper = damper;
			s.spring = spring;
			s.targetPosition = targetPosition;
			return s;
		}
	}

	[System.Serializable]
	public struct SerializableSoftJointLimitSpring {
		public float spring;
		public float damper;

		public SoftJointLimitSpring ToSoftJointLimitSpring() {
			SoftJointLimitSpring s = new SoftJointLimitSpring();
			s.damper = damper;
			s.spring = spring;
			return s;
		}
	}

	[System.Serializable]
	public struct SerializableSoftJointLimit {
		public float limit;
		public float bounciness;
		public float contactDistance;

		public SerializableSoftJointLimit(float limit = 0, float bounciness = 0, float contactDistance = 0) {
			this.limit = limit;
			this.bounciness = bounciness;
			this.contactDistance = contactDistance;
		}

		public SoftJointLimit ToSoftJointLimit() {
			SoftJointLimit l = new SoftJointLimit();
			l.limit = limit;
			l.bounciness = bounciness;
			l.contactDistance = contactDistance;
			return l;
		}
	}

	public enum JointLimitsRelativeTo {
		StartingRotation,
		ZeroRotation
	}

	[System.Serializable]
	public struct SerializableJointMotor {
		public float force;
		public bool freeSpin;
		public float targetVelocity;

		public JointMotor ToJointMotor() {
			JointMotor m = new JointMotor();
			m.force = force;
			m.freeSpin = freeSpin;
			m.targetVelocity = targetVelocity;
			return m;
		}
	}

	[System.Serializable]
	public struct SerializableJointDrive {
		public float positionSpring, positionDamper;
		public float maximumForce;

        public void Reset() {
            maximumForce = 3.402823e+38f;
        }

		public JointDrive ToJointDrive() {
			JointDrive d = new JointDrive();
			d.maximumForce = maximumForce;
			d.positionDamper = positionDamper;
			d.positionSpring = positionSpring;
			return d;
		}
	}

	[System.Serializable]
	public struct SerializableJointLimits {
		public float min;
		public float max;
		public float bounciness;
		public float bounceMinVelocity;
		public float contactDistance;
		public JointLimitsRelativeTo relativeTo;

        public void Reset() {
            bounceMinVelocity = 0.2f;
        }

		public JointLimits ToJointLimits() {
			JointLimits l = new JointLimits();
			l.min = min;
			l.max = max;
			l.bounciness = bounciness;
			l.bounceMinVelocity = bounceMinVelocity;
			l.contactDistance = contactDistance;
			return l;
		}
	}

	[System.Serializable]
	public class JointDescription {
		public JointType jointType;
        public Vector3 anchor, connectedAnchor;
		public Vector3 axis, swingAxis;
		public float breakForce, breakTorque;
		public bool enableCollision;
		public bool enablePreprocessing;
		public bool useLimits, useMotor, useSpring;
		public float tolerance;
		public float minDistance, maxDistance;
		public SerializableJointSpring spring;
		public SerializableSoftJointLimitSpring twistLimitSpring, swingLimitSpring;
		public SerializableJointMotor motor;
		public SerializableJointLimits limits;
		public SerializableSoftJointLimit lowTwistLimit, highTwistLimit, swing1Limit, swing2Limit;
		public bool enableProjection;
		public float projectionDistance;
		public float projectionAngle;
		// configurable joint props
		public Vector3 secondaryAxis;
		public ConfigurableJointMotion xMotion, yMotion, zMotion, angularXMotion, angularYMotion, angularZMotion;
		public SerializableSoftJointLimitSpring linearLimitSpring, angularXLimitSpring, angularYZLimitSpring;
		public SerializableSoftJointLimit linearLimit, lowAngularXLimit, highAngularXLimit, angularYLimit, 
                angularZLimit;
		public Vector3 targetPosition, targetVelocity, targetAngularVelocity;
		public SerializableJointDrive xDrive, yDrive, zDrive, angularXDrive, angularYZDrive, slerpDrive;
		public Quaternion targetRotation;
		public RotationDriveMode rotationDriveMode;
		public JointProjectionMode projectionMode;
		public bool configuredInWorldSpace, swapBodies;
		public JointLimitsRelativeTo angularLimitsRelativeTo;
		// end configurable joint props

        public JointDescription() {
            Reset();
        }

        public virtual void Reset() {
            jointType = JointType.FixedJoint;
            anchor = connectedAnchor = Vector3.zero;
            axis = new Vector3(1, 0, 0);
            swingAxis = new Vector3(0, 1, 0);
            breakForce = Mathf.Infinity;
            breakTorque = Mathf.Infinity;
            enablePreprocessing = true;
            tolerance = 0.025f;
            spring.Reset();
            lowTwistLimit = new SerializableSoftJointLimit(-20, 0, 0);
            highTwistLimit = new SerializableSoftJointLimit(70, 0, 0); 
            swing1Limit = new SerializableSoftJointLimit(40, 0, 0);
            swing2Limit = new SerializableSoftJointLimit(40, 0, 0);
            projectionDistance = 0.1f;
            projectionAngle = 180;
            secondaryAxis = new Vector3(0, 1, 0);
            xMotion = ConfigurableJointMotion.Free;
            yMotion = ConfigurableJointMotion.Free;
            zMotion = ConfigurableJointMotion.Free;
            angularXMotion = ConfigurableJointMotion.Free;
            angularYMotion = ConfigurableJointMotion.Free; 
            angularZMotion = ConfigurableJointMotion.Free;
            xDrive.Reset();
            yDrive.Reset();
            zDrive.Reset();
            angularXDrive.Reset();
            angularYZDrive.Reset();
            slerpDrive.Reset();
            targetRotation = Quaternion.identity;
        }

		void ConfigureJoint(Joint joint, Rigidbody connectedBody) {
			joint.enableCollision = enableCollision;
			joint.connectedBody = connectedBody;
			if (breakForce != Mathf.Infinity)
				joint.breakForce = breakForce;
			if (breakTorque != Mathf.Infinity)
				joint.breakTorque = breakTorque;
			joint.autoConfigureConnectedAnchor = false;
			joint.connectedAnchor = connectedAnchor;
			joint.anchor = anchor;
			joint.enablePreprocessing = enablePreprocessing;
			joint.axis = axis;
		}

		// public void ApplyAnchors(Joint joint) {
		// 	joint.anchor = anchor;
		// 	joint.connectedAnchor = connectedAnchor;
		// }

        bool JointMatchesJointType(Joint joint) {
            if (!joint)
                return false;
            if (jointType == JointType.CharacterJoint)
                return joint.GetType() == typeof(CharacterJoint);
            if (jointType == JointType.ConfigurableJoint)
                return joint.GetType() == typeof(ConfigurableJoint);
            if (jointType == JointType.FixedJoint)
                return joint.GetType() == typeof(FixedJoint);
            if (jointType == JointType.HingeJoint)
                return joint.GetType() == typeof(HingeJoint);
            if (jointType == JointType.SpringJoint)
                return joint.GetType() == typeof(SpringJoint);
            throw new System.NotImplementedException("Unrecognized joint type.");
        }

		public Joint CreateOrApply(GameObject go, Rigidbody connectedBody, Joint joint = null) {
            if (joint && !JointMatchesJointType(joint)) {
                GameObject.Destroy(joint);
			    joint = null;
            }
			if (jointType == JointType.FixedJoint) {
				FixedJoint fixedJoint = (FixedJoint) joint ?? go.AddComponent<FixedJoint>();
				ConfigureJoint(fixedJoint, connectedBody);
				joint = fixedJoint;
			}
			if (jointType == JointType.HingeJoint) {
				Quaternion startRotation = go.transform.rotation;
				if (useLimits && limits.relativeTo == JointLimitsRelativeTo.ZeroRotation && connectedBody)
					go.transform.rotation = connectedBody.transform.rotation;
				HingeJoint hingeJoint = (HingeJoint) joint ?? go.AddComponent<HingeJoint>();
				ConfigureJoint(hingeJoint, connectedBody);
				hingeJoint.useMotor = useMotor;
				hingeJoint.motor = motor.ToJointMotor();
				hingeJoint.useLimits = useLimits;
				hingeJoint.limits = limits.ToJointLimits();
				hingeJoint.useSpring = useSpring;
				hingeJoint.spring = spring.ToJointSpring();
				joint = hingeJoint;
				if (useLimits && limits.relativeTo == JointLimitsRelativeTo.ZeroRotation && connectedBody)
					go.transform.rotation = startRotation;
			}
			if (jointType == JointType.SpringJoint) {
				SpringJoint springJoint = (SpringJoint) joint ?? go.AddComponent<SpringJoint>();
				ConfigureJoint(springJoint, connectedBody);
				JointSpring s = spring.ToJointSpring();
				springJoint.spring = s.spring;
				springJoint.damper = s.damper;
				springJoint.tolerance = tolerance;
				springJoint.minDistance = minDistance;
				springJoint.maxDistance = maxDistance;
				joint = springJoint;
			}
			if (jointType == JointType.CharacterJoint) {
				CharacterJoint characterJoint = (CharacterJoint) joint ?? go.AddComponent<CharacterJoint>();
				ConfigureJoint(characterJoint, connectedBody);
				characterJoint.swingAxis = swingAxis;
				characterJoint.twistLimitSpring = twistLimitSpring.ToSoftJointLimitSpring();
				characterJoint.lowTwistLimit = lowTwistLimit.ToSoftJointLimit();
				characterJoint.highTwistLimit = highTwistLimit.ToSoftJointLimit();
				characterJoint.swingLimitSpring = swingLimitSpring.ToSoftJointLimitSpring();
				characterJoint.swing1Limit = swing1Limit.ToSoftJointLimit();
				characterJoint.swing2Limit = swing2Limit.ToSoftJointLimit();
				characterJoint.enableProjection = enableProjection;
				characterJoint.projectionAngle = projectionAngle;
				characterJoint.projectionDistance = projectionDistance;
				joint = characterJoint;
			}
			if (jointType == JointType.ConfigurableJoint) {
				Quaternion startRotation = go.transform.rotation;
				if (angularLimitsRelativeTo == JointLimitsRelativeTo.ZeroRotation && connectedBody)
					go.transform.rotation = connectedBody.transform.rotation;
				ConfigurableJoint configurableJoint = (ConfigurableJoint) joint 
                        ?? go.AddComponent<ConfigurableJoint>();
				ConfigureJoint(configurableJoint, connectedBody);
				configurableJoint.secondaryAxis = secondaryAxis;
				configurableJoint.xMotion = xMotion;
				configurableJoint.yMotion = yMotion;
				configurableJoint.zMotion = zMotion;
				configurableJoint.angularXMotion = angularXMotion;
				configurableJoint.angularYMotion = angularYMotion;
				configurableJoint.angularZMotion = angularZMotion;
				configurableJoint.linearLimitSpring = linearLimitSpring.ToSoftJointLimitSpring();
				configurableJoint.linearLimit = linearLimit.ToSoftJointLimit();
				configurableJoint.angularXLimitSpring = angularXLimitSpring.ToSoftJointLimitSpring();
				// Vector3 worldAxis1 = GetWorldAxis(axis, axisRelativeTo, go.transform, connectedBody); // the "x" axis
				// Vector3 worldAxis2 = GetWorldAxis(secondaryAxis, axisRelativeTo, go.transform, connectedBody); // the "y" axis
				// Vector3 worldAxis3 = Vector3.Cross(worldAxis2, worldAxis1); // the "z" axis
				configurableJoint.lowAngularXLimit = lowAngularXLimit.ToSoftJointLimit();
				configurableJoint.highAngularXLimit = highAngularXLimit.ToSoftJointLimit();
				configurableJoint.angularYZLimitSpring = angularYZLimitSpring.ToSoftJointLimitSpring();
				configurableJoint.angularYLimit = angularYLimit.ToSoftJointLimit();
				configurableJoint.angularZLimit = angularZLimit.ToSoftJointLimit();
				configurableJoint.targetPosition = targetPosition;
				configurableJoint.targetVelocity = targetVelocity;
				configurableJoint.xDrive = xDrive.ToJointDrive();
				configurableJoint.yDrive = yDrive.ToJointDrive();
				configurableJoint.zDrive = zDrive.ToJointDrive();
				configurableJoint.angularXDrive = angularXDrive.ToJointDrive();
				configurableJoint.angularYZDrive = angularYZDrive.ToJointDrive();
				configurableJoint.targetRotation = targetRotation;
				// note that you must have a >0 damper on the drives to get targetAngularVelocity to do anything
				configurableJoint.targetAngularVelocity = targetAngularVelocity;
				configurableJoint.rotationDriveMode = rotationDriveMode;
				configurableJoint.projectionAngle = projectionAngle;
				configurableJoint.projectionDistance = projectionDistance;
				configurableJoint.projectionMode = projectionMode;
				configurableJoint.configuredInWorldSpace = configuredInWorldSpace;
				configurableJoint.swapBodies = swapBodies;
				joint = configurableJoint;
				if (angularLimitsRelativeTo == JointLimitsRelativeTo.ZeroRotation && connectedBody)
					go.transform.rotation = startRotation;
			}

			return joint;
		}
	}

    [System.Serializable]
    public abstract class AttachStrategyJointDescription : JointDescription {

        public abstract ParamSelector GetAnchorParamSelector();

        public abstract ParamSelector GetConnectedAnchorParamSelector();

        public abstract ParamSelector GetAxisParamSelector();

        public abstract ParamSelector GetSecondaryAxisParamSelector();

        public abstract ParamSelector GetSwingAxisParamSelector();

    }

    public class PhysicsObjectState : AttachObjectState {
        public Joint[] joints;
    }

    public static class PhysicsAttachStrategyHelper {

        public static PhysicsObjectState GetObjectState(AttachObject obj) {
            PhysicsObjectState state;
            if (obj.state == null) {
                state = new PhysicsObjectState();
                obj.state = state;
            }
            state = (PhysicsObjectState) obj.state;
            return state;
        }

        public static void DestroyJoints(AttachObject obj) {
            PhysicsObjectState state = GetObjectState(obj);
            for (int i = 0; i < state.joints.Length; i++) {
                if (state.joints[i]) {
                    GameObject.Destroy(state.joints[i]);
                    state.joints[i] = null;
                }
            }
        }

        static void UpdateJointDescriptionFromParams(AttachStrategyJointDescription jd, AttachObject jointed, 
                AttachObject connected, AttachObject reference = null) {
            Attachment attachment = jointed.attachment;

            ParamSelector selector; 
            
            selector = jd.GetAnchorParamSelector();
            // anchors must be relative to the associated Rigidbody's transform
            jd.anchor = jointed.rigidbody.transform.InverseTransformPoint(
                    selector.GetWorldPosition(attachment, reference));
            
            selector = jd.GetConnectedAnchorParamSelector();
            // anchors must be relative to the associated Rigidbody's transform
            jd.connectedAnchor = connected.rigidbody.transform.InverseTransformPoint(
                    selector.GetWorldPosition(attachment, reference));

            selector = jd.GetAxisParamSelector();
            // axes must be relative to the jointed Rigidbody's transform
            jd.axis = jointed.rigidbody.transform.InverseTransformDirection(
                    selector.GetWorldDirection(attachment, reference));

            selector = jd.GetSecondaryAxisParamSelector();
            // axes must be relative to the jointed Rigidbody's transform
            jd.secondaryAxis = jointed.rigidbody.transform.InverseTransformDirection(
                    selector.GetWorldDirection(attachment, reference));

            selector = jd.GetSwingAxisParamSelector();
            // axes must be relative to the jointed Rigidbody's transform
            jd.swingAxis = jointed.rigidbody.transform.InverseTransformDirection(
                    selector.GetWorldDirection(attachment, reference));

            // todo maybe
            // selector = jd.GetLimitsParamSelector();
            // provider = reference != null ? attachment.attachStrategy.ResolveProvider(selector.provider, reference)
            //         : attachment.attachStrategy.ResolveProvider(selector.provider, attachment);
            // jd.limits = provider.resolvedParams.GetParam(ParamType.AngleLimits, selector.name, selector.defaultParam)
            //         .angleLimitsValue;
        }

        // public static void UpdateJointDescriptionsFromParams(AttachObject jointed, AttachObject connected, 
        //         AttachStrategyJointDescription2D[] jointDescriptions) {
        //     PhysicsObjectState jointedState = GetObjectState(jointed);
        //     PhysicsObjectState connectedState = GetObjectState(connected);
        //     int i = 0;
        //     foreach (AttachStrategyJointDescription2D jd in jointDescriptions) {
        //         UpdateJointDescriptionFromParams(jd, jointed, connected);
        //         // jd.ApplyAnchors(jointedState.joints[i]);
        //         // jd.ApplyLimits(jointedState.joints[i], jointed.gameObject, connectedState.rb);
        //         i ++;
        //     }
        // }

        public static void CreateOrApplyAllJoints(AttachObject jointed, AttachObject connected, 
                AttachStrategyJointDescription[] jointDescriptions, AttachObject reference = null, 
                bool hideJointsInInspector = true) {
            PhysicsObjectState jointedState = GetObjectState(jointed);
            if (jointedState.joints == null || jointedState.joints.Length != jointDescriptions.Length)
                jointedState.joints = new Joint[jointDescriptions.Length];
            int i = 0;
            foreach (AttachStrategyJointDescription jd in jointDescriptions) {
                UpdateJointDescriptionFromParams(jd, jointed, connected, reference);
                jointedState.joints[i] = jd.CreateOrApply(jointed.rigidbody.gameObject, connected.rigidbody, 
                        joint: jointedState.joints[i]);
                jointedState.joints[i].hideFlags = hideJointsInInspector ? HideFlags.HideInInspector : HideFlags.None;
                i ++;
            }
        }

    }

}