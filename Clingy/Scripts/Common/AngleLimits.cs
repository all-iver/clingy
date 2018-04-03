namespace SubC.Attachments {

    using UnityEngine;
    using ClingyPhysics;

    [System.Serializable]
    public struct AngleLimits {
        public float min;
        public float max;

        public AngleLimits(float min = 0, float max = 359) {
            this.min = min;
            this.max = max;
        }

        public static bool operator ==(AngleLimits lhs, AngleLimits rhs) {
            if (System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null))
                return true;
            if (System.Object.ReferenceEquals(lhs, null) || System.Object.ReferenceEquals(rhs, null))
                return false;
            return lhs.min == rhs.min && lhs.max == rhs.max;
        }

        public static bool operator !=(AngleLimits lhs, AngleLimits rhs) {
            if (System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null))
                return false;
            if (System.Object.ReferenceEquals(lhs, null) || System.Object.ReferenceEquals(rhs, null))
                return true;
            return lhs.min != rhs.min || lhs.max != rhs.max;
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return (AngleLimits) obj == this;
        }
    
        public override int GetHashCode() {
            return (min + max).GetHashCode();
        }

        public JointAngleLimits2D ToJointAngleLimits2D() {
            JointAngleLimits2D limits = new JointAngleLimits2D();
            limits.min = min;
            limits.max = max;
            return limits;
        }

    }

}