namespace SubC.Attachments {

    using UnityEngine;

    public class AdoptOptionAttribute : PropertyAttribute {

        public bool keepUpdated;

        public AdoptOptionAttribute(bool keepUpdated = true) {
            this.keepUpdated = keepUpdated;
        }

    }

}