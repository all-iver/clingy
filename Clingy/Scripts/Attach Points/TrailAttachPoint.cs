namespace SubC.Attachments {

	using UnityEngine;

	[AddComponentMenu("Clingy/Trail Attach Point", 2)]
	public class TrailAttachPoint : AttachPoint {

        public string outputPosition = "trailposition";
        public Vector3 offset;

        public override void ApplyParamsForOther(AttachObject other, AttachObject self) {
            other.resolvedParams.SetParam(new Param(offset * (1 + other.indexInCategory), outputPosition));
        }

	}

}