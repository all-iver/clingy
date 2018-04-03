namespace SubC.Attachments {

    using UnityEngine;

    public static class ClingyUtils {

        public static Quaternion LookAt2D(Vector3 currentPos, Vector3 lookAtPos, bool flipX = false) {
            Vector2 delta = lookAtPos - currentPos;
            float radians = Mathf.Atan2(delta.y, delta.x);
            return Quaternion.Euler(0, 0, radians * Mathf.Rad2Deg + (flipX ? 180 : 0));
        }

        public static void MovePosition(Vector3 position, Transform transform, MoveMethod moveMethod, 
                Rigidbody rb = null, Rigidbody2D rb2D = null) {
            if (moveMethod == MoveMethod.Translate) {
                transform.position = position;
            } else if (moveMethod == MoveMethod.SetPhysics) {
                if (rb)
                    rb.position = position;
                else if (rb2D && rb2D.simulated)
                    rb2D.position = position;
                else
                    transform.position = position;
            } else if (moveMethod == MoveMethod.MovePhysics) {
                if (rb)
                    rb.MovePosition(position);
                else if (rb2D && rb2D.simulated)
                    rb2D.MovePosition(position);
                else
                    transform.position = position;
            }
        }

        public static void MoveRotation(Quaternion rotation, Transform transform, RotateMethod rotateMethod, 
                Rigidbody rb = null, Rigidbody2D rb2D = null) {
            if (rotateMethod == RotateMethod.Translate) {
                transform.rotation = rotation;
            } else if (rotateMethod == RotateMethod.SetPhysics) {
                if (rb)
                    rb.rotation = rotation;
                else if (rb2D && rb2D.simulated)
                    rb2D.rotation = rotation.eulerAngles.z;
                else
                    transform.rotation = rotation;
            } else if (rotateMethod == RotateMethod.MovePhysics) {
                if (rb)
                    rb.MoveRotation(rotation);
                else if (rb2D && rb2D.simulated)
                    rb2D.MoveRotation(rotation.eulerAngles.z);
                else
                    transform.rotation = rotation;
            }
        }
    
    }

}