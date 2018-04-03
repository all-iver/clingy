namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

	[CreateAssetMenuAttribute(fileName = "New Parenting Chain Attach Strategy", 
            menuName = "Clingy/Attach Strategies/Parenting Chain Strategy")]
	public class ParentingChainStrategy : ChainAttachStrategy {

        protected override void Reset() {
            base.Reset();
            // reattachWhenParamsUpdated = true;
        }

		protected override void ConnectLinks(AttachObject parent, AttachObject child) {
            child.attachable.transform.SetParent(parent.attachable.transform);
        }

        protected override void DisconnectLinks(AttachObject parent, AttachObject child) {
            child.attachable.transform.SetParent(null);
        }

	}

}