namespace SubC.Attachments {

	using UnityEngine;

	[CreateAssetMenu(fileName = "New Symbolic One-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Symbolic One-to-One Strategy")]
	public class SymbolicOneToOneStrategy : OneToOneAttachStrategy {

        protected override void ConnectBoth(AttachObject obj1, AttachObject obj2) {
        }

        protected override void DisconnectBoth(AttachObject obj1, AttachObject obj2) {
        }

	}

}