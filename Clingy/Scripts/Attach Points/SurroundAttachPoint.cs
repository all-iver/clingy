namespace SubC.Attachments {

	using UnityEngine;

	[AddComponentMenu("Clingy/Surround Attach Point", 2)]
	public class SurroundAttachPoint : AttachPoint {

        public string outputPosition = "surroundPos";
        public float distance;

        public override void ApplyParamsForOther(AttachObject other, AttachObject self) {
            int count = self.attachment.objects.Count(other.category);
            float degree = (360 / count) * other.indexInCategory;
            float radians = degree * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * distance;
            other.resolvedParams.SetParam(new Param(pos, outputPosition));
        }

	}

}