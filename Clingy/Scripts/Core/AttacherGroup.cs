namespace SubC.Attachments {

    using UnityEngine;
    using System.Collections.Generic;

	[AddComponentMenu("Clingy/Attacher Group", 0)]
    public class AttacherGroup : MonoBehaviour {

        public List<Attacher> attachers;

        public void Attach() {
            foreach (Attacher a in attachers)
                a.Attach();
        }

        public void Detach() {
            foreach (Attacher a in attachers)
                a.Detach();
        }

    }

}