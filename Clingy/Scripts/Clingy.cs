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

        public static Attachment AttachManyToOne(AttachStrategy strategy, GameObject root, 
                params GameObject[] leaves) {
            AttachObject[] objects = new AttachObject[leaves.Length + 1];
            objects[0] = new AttachObject(root, 0);
            for (int i = 0; i < leaves.Length; i++)
                objects[i + 1] = new AttachObject(leaves[i], 1);
            Attachment attachment = new Attachment(strategy, objects);
            attachment.Attach();
            return attachment;
        }

        public static Attachment AttachChain(AttachStrategy strategy, params GameObject[] links) {
            AttachObject[] objects = new AttachObject[links.Length];
            for (int i = 0; i < links.Length; i++)
                objects[i] = new AttachObject(links[i], 1);
            Attachment attachment = new Attachment(strategy, objects);
            attachment.Attach();
            return attachment;
        }

    }

}