namespace SubC.Attachments {

	using UnityEngine;

	public abstract class ManyToOneAttachStrategy : AttachStrategy {

        public enum Categories {
            Root, Leaves
        }

        AttachCategoryDefinition[] categories;

        public override AttachCategoryDefinition[] GetCategories() {
            if (categories == null) {
                categories = new AttachCategoryDefinition[] {
                    new AttachCategoryDefinition(GetLabelForRoot(), GetLabelForRoot(), 1, 1),
                    new AttachCategoryDefinition(GetLabelForLeaves(), GetLabelForLeaf(), 0, -1)
                };
            }
            return categories;
        }

        public virtual string GetLabelForRoot() {
            return "Root";
        }

        public virtual string GetLabelForLeaves() {
            return "Leaves";
        }

        public virtual string GetLabelForLeaf() {
            return "Leaf";
        }

        protected enum Providers {
            Root, Leaf
        }

        public override string GetLabelForProvider(int provider) {
            if (provider == (int) Providers.Root)
                return GetLabelForRoot();
            return GetLabelForLeaf();
        }

        public override int[] GetProvidersForTransitioner(int category) {
            if (category == (int) Categories.Root)
                return new int[] { (int) Providers.Root };
            else
                return new int[] { (int) Providers.Root, (int) Providers.Leaf };
        }

        public override AttachObject ResolveProvider(int provider, Attachment attachment) {
            throw new System.NotSupportedException("Use ResolveProvider(int provider, "
                    + "AttachObject reference) with ManyToOneAttachStrategy strategies");
        }

        public override AttachObject ResolveProvider(int provider, AttachObject reference) {
            if (provider == (int) Providers.Root)
                return GetRoot(reference.attachment);
            if (reference.category == (int) Categories.Leaves)
                return reference;
            return null;
        }

        protected AttachObject GetRoot(Attachment attachment) {
            return attachment.objects.Get((int) Categories.Root, 0);
        }

        protected virtual void ConnectRoot(AttachObject root) {
        }

        protected virtual void DisconnectRoot(AttachObject root) {
        }

        protected abstract void ConnectLeaf(AttachObject root, AttachObject leaf);

        protected abstract void DisconnectLeaf(AttachObject root, AttachObject leaf);

		public override bool ConnectObject(AttachObject obj) {
            AttachObject root = GetRoot(obj.attachment);
            if (obj == root) {
                ConnectRoot(obj);
                AttachObjectList.Enumerator e = obj.attachment.objects.GetEnumerator((int) Categories.Leaves, 
                        phase: AttachObjectPhase.Connected);
                while (e.MoveNext())
                    ConnectLeaf(root, e.Current);
            } else {
                if (root == null || !root.isConnected)
                    return true;
                ConnectLeaf(root, obj);
            }
            return true;
        }

        public override bool DisconnectObject(AttachObject obj) {
            DisconnectObjectImmediate(obj);
            return true;
        }

        public override void DisconnectObjectImmediate(AttachObject obj) {
            AttachObject root = GetRoot(obj.attachment);
            if (obj == root) {
                AttachObjectList.Enumerator e = obj.attachment.objects.GetEnumerator((int) Categories.Leaves, 
                        phase: AttachObjectPhase.Connected);
                while (e.MoveNext())
                    DisconnectLeaf(root, e.Current);
                DisconnectRoot(root);
            } else {
                if (root == null || !root.isConnected)
                    return;
                DisconnectLeaf(root, obj);
            }
        }

	}

}