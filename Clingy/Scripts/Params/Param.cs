namespace SubC.Attachments {

    using UnityEngine;

    [System.Serializable]
    public struct Param {

        public static System.Collections.Generic.Dictionary<ParamType, string> defaultNameForType 
                = new System.Collections.Generic.Dictionary<ParamType, string>() {
            {ParamType.AngleLimits, "angleLimits"},
            {ParamType.Bool, "bool"},
            {ParamType.Color, "color"},
            {ParamType.Curve, "curve"},
            {ParamType.Float, "float"},
            {ParamType.Gradient, "gradient"},
            {ParamType.Integer, "integer"},
            {ParamType.Layer, "layer"},
            {ParamType.Object, "object"},
            {ParamType.Rotation, "rotation"},
            {ParamType.String, "string"},
            {ParamType.Vector3, "vector3"},
        };

        public static string GetValuePropName(ParamType type) {
            switch(type) {
                case ParamType.AngleLimits:
                    return "angleLimitsValue";
                case ParamType.Bool:
                    return "boolValue";
                case ParamType.Color:
                    return "colorValue";
                case ParamType.Curve:
                    return "curveValue";
                case ParamType.Float:
                    return "floatValue";
                case ParamType.Gradient:
                    return "gradientValue";
                case ParamType.Integer:
                    return "intValue";
                case ParamType.Layer:
                    return "layerValue";
                case ParamType.Object:
                    return "objectValue";
                case ParamType.Rotation:
                    return "quaternionValue";
                case ParamType.String:
                    return "stringValue";
                case ParamType.Vector3:
                    return "vector3Value";
            }
            throw new System.NotImplementedException();
        }
        
        public ParamType type;
        [Delayed]
        public string name;
        public Vector3 vector3Value;
        [Rotation]
        public Quaternion quaternionValue;
        public bool boolValue;
        [Delayed]
        public int intValue;
        [Delayed]
        public float floatValue;
        [Delayed]
        public string stringValue;
        [Layer]
        public int layerValue; // layer has to be a LayerAttribute or multi-object editing doesn't work
        public AngleLimits angleLimitsValue;
        public Color colorValue;
        public Gradient gradientValue;
        public AnimationCurve curveValue;
        public GameObject objectValue;

        // even though we're a struct, any "owned" reference types like Gradient and AnimationCurve will be shared 
        // unless you call this and actually copy them!
        public Param Clone() {
            Param p = this;
            if (gradientValue != null) {
                p.gradientValue = new Gradient();
                p.gradientValue.SetKeys(gradientValue.colorKeys, gradientValue.alphaKeys); // fixme - is this it?
            }
            if (p.curveValue != null)
                p.curveValue = new AnimationCurve(curveValue.keys); // fixme - is this it?
            return p;
        }

        public string GetValuePropName() {
            return Param.GetValuePropName(type);
        }

        public bool Equals(Param other) {
            if (other.type != type || other.name != name)
                return false;
            switch(type) {
                case ParamType.AngleLimits:
                    return angleLimitsValue == other.angleLimitsValue;
                case ParamType.Bool:
                    return boolValue == other.boolValue;
                case ParamType.Color:
                    return colorValue == other.colorValue;
                case ParamType.Object:
                    return objectValue == other.objectValue;
                case ParamType.Gradient:
                    return gradientValue.Equals(other.gradientValue);
                case ParamType.Curve:
                    return curveValue.Equals(other.curveValue);
                case ParamType.Float:
                    return floatValue == other.floatValue;
                case ParamType.String:
                    return stringValue == other.stringValue;
                case ParamType.Vector3:
                    return vector3Value == other.vector3Value;
                case ParamType.Rotation:
                    return quaternionValue == other.quaternionValue;
                case ParamType.Integer:
                    return intValue == other.intValue;
                case ParamType.Layer:
                    return layerValue == other.layerValue;
            }
            throw new System.NotImplementedException();
        }

        public Param(ParamType type, string name = null) : this() {
            this.type = type;
            this.name = name ?? defaultNameForType[type];
        }

        public Param(Vector3 vector3Value, string name = null) : this() {
            this.type = ParamType.Vector3;
            this.name = name ?? defaultNameForType[type];
            this.vector3Value = vector3Value;
        }

        public Param(Quaternion quaternionValue, string name = null) : this() {
            this.type = ParamType.Rotation;
            this.name = name ?? defaultNameForType[type];
            this.quaternionValue = quaternionValue;
        }

        public Vector3 GetWorldPosition(GameObject local = null, SpriteRenderer spriteRenderer = null, 
                PositionTransformType useTransform = PositionTransformType.Full) {
            if (type != ParamType.Vector3)
                throw new System.InvalidOperationException("Param is not a Vector3 type");
            if (local == null)
                return vector3Value;
            Vector3 value = vector3Value;
            if (spriteRenderer != null && spriteRenderer.flipX)
                value.x = -value.x;
            if (spriteRenderer != null && spriteRenderer.flipY)
                value.y = -value.y;
            if (useTransform == PositionTransformType.None) {
                return local.transform.position + value;
            } else if (useTransform == PositionTransformType.RotationOnly) {
                // https://answers.unity.com/questions/1238142/version-of-transformtransformpoint-which-is-unaffe.html
                Matrix4x4 matrix = Matrix4x4.TRS(local.transform.position, local.transform.rotation, Vector3.one);
                return matrix.MultiplyPoint3x4(value);
            }
            return local.transform.TransformPoint(value);
        }

        public Quaternion GetWorldRotation(GameObject local = null) {
            if (type != ParamType.Rotation)
                throw new System.InvalidOperationException("Param is not a Rotation type");
            if (local == null)
                return quaternionValue;
            return local.transform.rotation * quaternionValue;
        }

        public Vector3 GetWorldDirection(GameObject local) {
            if (type != ParamType.Vector3)
                throw new System.InvalidOperationException("Param is not a Vector3 type");
            if (local == null)
                return vector3Value;
            return local.transform.TransformDirection(vector3Value);
        }

    }

}