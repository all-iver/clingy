namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;
    using ClingyPhysics2D;

    [CustomPropertyDrawer(typeof(AttachStrategyJointDescription2D), true)]
    public class AttachStrategyJointDescription2DPropertyDrawer : PropertyDrawer {

		static string[] fixedJoint2DProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "dampingRatio", "frequency", "breakForce", "breakTorque", "enableCollision" };
		static string[] hingeJoint2DProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "useLimits", "limitsParam", "useMotor", "motor", "breakForce", "breakTorque", "enableCollision" };
		static string[] distanceJoint2DProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
				"distanceParam", "maxDistanceOnly", "autoConfigureDistance", "breakForce", "breakTorque", 
                "enableCollision" };
		static string[] springJoint2DProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
				"autoConfigureDistance", "distanceParam", "dampingRatio", "frequency", "breakForce", "breakTorque", 
                "enableCollision" };
		static string[] targetJoint2DProps = new string[] { "jointType", "anchorParam", "targetParam", 
                "maxForce", "dampingRatio", "frequency", "breakForce", "enableCollision" };
		static string[] relativeJoint2DProps = new string[] { "jointType", "maxForce", "maxTorque", "correctionScale", 
                "autoConfigureOffset", "linearOffset", "angularOffset", "breakForce", "breakTorque", 
                "enableCollision" };
		static string[] wheelJoint2DProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "breakForce", "breakTorque", "enableCollision" };
		static string[] sliderJoint2DProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "breakForce", "breakTorque", "enableCollision" };
		static string[] frictionJoint2DProps = new string[] { "jointType", "anchorParam", "connectedAnchorParam", 
                "maxFrictionForce", "maxFrictionTorque", "breakForce", "breakTorque", "enableCollision" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			SerializedProperty jointTypeProp = property.FindPropertyRelative("jointType");
			JointType2D jointType = (JointType2D) jointTypeProp.intValue;
			if (jointType == JointType2D.DistanceJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, distanceJoint2DProps);
			else if (jointType == JointType2D.FrictionJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, frictionJoint2DProps);
			else if (jointType == JointType2D.HingeJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, hingeJoint2DProps);
			else if (jointType == JointType2D.RelativeJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, relativeJoint2DProps);
			else if (jointType == JointType2D.TargetJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, targetJoint2DProps);
			else if (jointType == JointType2D.SpringJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, springJoint2DProps);
			else if (jointType == JointType2D.FixedJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, fixedJoint2DProps);
			else if (jointType == JointType2D.WheelJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, wheelJoint2DProps);
			else if (jointType == JointType2D.SliderJoint2D)
				return ClingyEditorUtils.GetPropertyHeight(property, sliderJoint2DProps);
			throw new System.NotImplementedException("Clingy: Unrecognized joint type");
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
			SerializedProperty jointTypeProp = property.FindPropertyRelative("jointType");
			JointType2D jointType = (JointType2D) jointTypeProp.intValue;
			if (jointType == JointType2D.DistanceJoint2D)
				ClingyEditorUtils.DrawProperties(property, distanceJoint2DProps, rect);
			else if (jointType == JointType2D.FrictionJoint2D)
				ClingyEditorUtils.DrawProperties(property, frictionJoint2DProps, rect);
			else if (jointType == JointType2D.HingeJoint2D)
				ClingyEditorUtils.DrawProperties(property, hingeJoint2DProps, rect);
			else if (jointType == JointType2D.RelativeJoint2D)
				ClingyEditorUtils.DrawProperties(property, relativeJoint2DProps, rect);
			else if (jointType == JointType2D.TargetJoint2D)
				ClingyEditorUtils.DrawProperties(property, targetJoint2DProps, rect);
			else if (jointType == JointType2D.SpringJoint2D)
				ClingyEditorUtils.DrawProperties(property, springJoint2DProps, rect);
			else if (jointType == JointType2D.FixedJoint2D)
				ClingyEditorUtils.DrawProperties(property, fixedJoint2DProps, rect);
			else if (jointType == JointType2D.WheelJoint2D)
				ClingyEditorUtils.DrawProperties(property, wheelJoint2DProps, rect);
			else if (jointType == JointType2D.SliderJoint2D)
				ClingyEditorUtils.DrawProperties(property, sliderJoint2DProps, rect);
            else
    			throw new System.NotImplementedException("Clingy: Unrecognized joint type");
    	}

    }

}