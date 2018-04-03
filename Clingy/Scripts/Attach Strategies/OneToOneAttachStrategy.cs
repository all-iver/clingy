namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;

	public abstract class OneToOneAttachStrategy : AttachStrategy {

        public enum Categories {
            Object1, Object2
        }

        AttachCategoryDefinition[] categories;

        public override AttachCategoryDefinition[] GetCategories() {
            if (categories == null) {
                categories = new AttachCategoryDefinition[] {
                    new AttachCategoryDefinition(GetLabelForObject1(), GetLabelForObject1(), 1, 1),
                    new AttachCategoryDefinition(GetLabelForObject2(), GetLabelForObject2(), 1, 1)
                };
            }
            return categories;
        }

        public virtual string GetLabelForObject1() {
            return "Object1";
        }

        public virtual string GetLabelForObject2() {
            return "Object2";
        }

        protected enum Providers {
            Object1, Object2
        }

        public override string GetLabelForProvider(int provider) {
            if (provider == (int) Providers.Object1)
                return GetLabelForObject1();
            return GetLabelForObject2();
        }

        public override int[] GetProvidersForTransitioner(int category) {
            if (category == (int) Categories.Object1)
                return new int[] { (int) Providers.Object1, (int) Providers.Object2 };
            else
                return new int[] { (int) Providers.Object2, (int) Providers.Object1 };
        }

        public override AttachObject ResolveProvider(int provider, AttachObject reference) {
            return ResolveProvider(provider, reference.attachment);
        }

        public override AttachObject ResolveProvider(int provider, Attachment attachment) {
            if (provider == (int) Providers.Object1)
                return GetObject1(attachment);
            return GetObject2(attachment);
        }

        public AttachObject GetObject1(Attachment attachment) {
            return attachment.objects.Get((int) Categories.Object1, 0);
        }

        public AttachObject GetObject2(Attachment attachment) {
            return attachment.objects.Get((int) Categories.Object2, 0);
        }

        protected abstract void ConnectBoth(AttachObject obj1, AttachObject obj2);

        protected abstract void DisconnectBoth(AttachObject obj1, AttachObject obj2);

		public override bool ConnectObject(AttachObject obj) {
            if (obj.category == (int) Categories.Object1) {
                AttachObject obj2 = GetObject2(obj.attachment);
                if (obj2 != null && obj2.isConnected)
                    ConnectBoth(obj, obj2);
            } else {
                AttachObject obj1 = GetObject1(obj.attachment);
                if (obj1 != null && obj1.isConnected)
                    ConnectBoth(obj1, obj);
            }
            return true;
        }

        public override void DisconnectObjectImmediate(AttachObject obj) {
            // keep in mind obj may be orphaned at this point
            if (obj.category == (int) Categories.Object1) {
                AttachObject obj2 = GetObject2(obj.attachment);
                if (obj2 != null && obj2.isConnected)
                    DisconnectBoth(obj, obj2);
            } else {
                AttachObject obj1 = GetObject1(obj.attachment);
                if (obj1 != null && obj1.isConnected)
                    DisconnectBoth(obj1, obj);
            }
        }

        protected bool IsConnected(Attachment attachment) {
            AttachObject obj1 = GetObject1(attachment);
            AttachObject obj2 = GetObject2(attachment);
            return obj1 != null && obj1.isConnected && obj2 != null && obj2.isConnected;
        }

	}

}