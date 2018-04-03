namespace SubC.Attachments {

	using UnityEngine;

	[CreateAssetMenuAttribute(fileName = "New Parenting Many-to-One Attach Strategy", 
            menuName = "Clingy/Attach Strategies/Parenting Many-to-One Strategy")]
	public class ParentingManyToOneStrategy : ManyToOneAttachStrategy {

        public new enum Categories {
            Parent, Children
        }

        protected override void Reset() {
            base.Reset();
            // reattachWhenParamsUpdated = true;
        }

        public override string GetLabelForRoot() {
            return "Parent";
        }

        public override string GetLabelForLeaves() {
            return "Children";
        }

        public override string GetLabelForLeaf() {
            return "Child";
        }

		protected override void ConnectLeaf(AttachObject root, AttachObject leaf) {
            leaf.seedObject.transform.SetParent(root.seedObject.transform);
		}

        protected override void DisconnectLeaf(AttachObject root, AttachObject leaf) {
            leaf.seedObject.transform.SetParent(null);
        }

	}

}