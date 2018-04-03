namespace SubC.Attachments {
    
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [AddComponentMenu("")]
    public class Clingy {

        public static Attachment AttachOneToOne(AttachStrategy strategy, GameObject object1, GameObject object2) {
            Attachment attachment = new Attachment(strategy, new AttachObject(object1, 0), 
                    new AttachObject(object2, 1));
            attachment.Attach();
            return attachment;
        }

    }

}