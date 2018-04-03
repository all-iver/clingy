namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;
    using ClingyPhysics;

    [CustomPropertyDrawer(typeof(AttachStrategyJointDescription), true)]
    public class AttachStrategyJointDescriptionPropertyDrawer : PropertyDrawer {

		static string[] fixedJointProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "breakForce", "breakTorque", "enableCollision", "enablePreprocessing" };
		static string[] hingeJointProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "axisParam", "useSpring", "spring", "useMotor", "motor", "useLimits", "limits", "breakForce", 
                "breakTorque", "enableCollision", "enablePreprocessing" };
		static string[] characterJointProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "axisParam", "swingAxisParam", "twistLimitSpring", "lowTwistLimit", 
                "highTwistLimit", "swingLimitSpring", "swing1Limit", "swing2Limit", "enableProjection", 
                "projectionAngle", "projectionDistance", "breakForce", "breakTorque", "enableCollision", 
                "enablePreprocessing" };
		static string[] springJointProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "spring.spring", "spring.damper", "minDistance", "maxDistance", "tolerance", "breakForce", 
                "breakTorque", "enableCollision", "enablePreprocessing" };
		static string[] configurableJointProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "axisParam", "secondaryAxisParam", "xMotion", "yMotion", "zMotion", "angularXMotion", "angularYMotion", 
                "angularZMotion", "linearLimitSpring", "linearLimit", "angularXLimitSpring", "lowAngularXLimit", 
                "highAngularXLimit", "angularYZLimitSpring", "angularYLimit", "angularZLimit", 
                "angularLimitsRelativeTo", "targetPosition", "targetVelocity", "xDrive", "yDrive", "zDrive", 
                "targetRotation", "targetAngularVelocity", "rotationDriveMode", "angularXDrive", "angularYZDrive", 
                "slerpDrive", "projectionMode", "projectionDistance", "projectionAngle", "configuredInWorldSpace", 
                "swapBodies", "breakForce", "breakTorque", "enableCollision", "enablePreprocessing" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			SerializedProperty jointTypeProp = property.FindPropertyRelative("jointType");
			JointType jointType = (JointType) jointTypeProp.intValue;
			if (jointType == JointType.CharacterJoint)
				return ClingyEditorUtils.GetPropertyHeight(property, characterJointProps);
			else if (jointType == JointType.ConfigurableJoint)
				return ClingyEditorUtils.GetPropertyHeight(property, configurableJointProps);
			else if (jointType == JointType.HingeJoint)
				return ClingyEditorUtils.GetPropertyHeight(property, hingeJointProps);
			else if (jointType == JointType.SpringJoint)
				return ClingyEditorUtils.GetPropertyHeight(property, springJointProps);
			else if (jointType == JointType.FixedJoint)
				return ClingyEditorUtils.GetPropertyHeight(property, fixedJointProps);
			throw new System.NotImplementedException("Clingy: Unrecognized joint type");
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
			SerializedProperty jointTypeProp = property.FindPropertyRelative("jointType");
			JointType jointType = (JointType) jointTypeProp.intValue;
			if (jointType == JointType.CharacterJoint)
				ClingyEditorUtils.DrawProperties(property, characterJointProps, rect);
			else if (jointType == JointType.ConfigurableJoint)
				ClingyEditorUtils.DrawProperties(property, configurableJointProps, rect);
			else if (jointType == JointType.HingeJoint)
				ClingyEditorUtils.DrawProperties(property, hingeJointProps, rect);
			else if (jointType == JointType.SpringJoint)
				ClingyEditorUtils.DrawProperties(property, springJointProps, rect);
			else if (jointType == JointType.FixedJoint)
				ClingyEditorUtils.DrawProperties(property, fixedJointProps, rect);
            else
    			throw new System.NotImplementedException("Clingy: Unrecognized joint type");
    	}

    }

}