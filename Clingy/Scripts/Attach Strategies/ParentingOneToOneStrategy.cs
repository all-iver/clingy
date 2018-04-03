namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

	[CreateAssetMenuAttribute(fileName = "New Parenting One-to-One Attach Strategy", 
            menuName = "Clingy/Attach Strategies/Parenting One-to-One Strategy")]
	public class ParentingOneToOneStrategy : OneToOneAttachStrategy {

        public new enum Categories {
            Parent = OneToOneAttachStrategy.Categories.Object1,
            Child = OneToOneAttachStrategy.Categories.Object2
        }

        protected override void Reset() {
            base.Reset();
            // reattachWhenParamsUpdated = true;
        }

        public override string GetLabelForObject1() {
            return "Parent";
        }

        public override string GetLabelForObject2() {
            return "Child";
        }

        public AttachObject GetParent(Attachment attachment) {
            return GetObject1(attachment);
        }

        public AttachObject GetChild(Attachment attachment) {
            return GetObject2(attachment);
        }

		protected override void ConnectBoth(AttachObject parent, AttachObject child) {
            child.attachable.transform.SetParent(parent.attachable.transform);
        }

        protected override void DisconnectBoth(AttachObject parent, AttachObject child) {
            child.attachable.transform.SetParent(null);
        }

	}

}