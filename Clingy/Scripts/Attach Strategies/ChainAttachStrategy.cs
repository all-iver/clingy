namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;

	public abstract class ChainAttachStrategy : AttachStrategy {

        public enum Categories {
            Head,
            Links,
            Tail
        }

        AttachCategoryDefinition[] categories;

        public override AttachCategoryDefinition[] GetCategories() {
            if (categories == null) {
                categories = new AttachCategoryDefinition[] {
                    new AttachCategoryDefinition(GetLabelForHead(), GetLabelForHead(), 0, 1),
                    new AttachCategoryDefinition(GetLabelForLinks(), GetLabelForLink(), 0, -1),
                    new AttachCategoryDefinition(GetLabelForTail(), GetLabelForTail(), 0, 1)
                };
            }
            return categories;
        }

        public virtual string GetLabelForHead() {
            return "Head";
        }

        public virtual string GetLabelForTail() {
            return "Tail";
        }

        public virtual string GetLabelForLinks() {
            return "Links";
        }

        public virtual string GetLabelForLink() {
            return "Link";
        }

        public virtual string GetLabelForPreviousLink() {
            return "Previous " + GetLabelForLink();
        }

        public virtual string GetLabelForNextLink() {
            return "Next " + GetLabelForLink();
        }

        protected enum Providers {
            Head, Link, PreviousLink, NextLink, Tail
        }

        public override string GetLabelForProvider(int provider) {
            if (provider == (int) Providers.Link)
                return GetLabelForLink();
            if (provider == (int) Providers.NextLink)
                return GetLabelForNextLink();
            if (provider == (int) Providers.PreviousLink)
                return GetLabelForPreviousLink();
            if (provider == (int) Providers.Head)
                return GetLabelForHead();
            if (provider == (int) Providers.Tail)
                return GetLabelForTail();
            throw new System.InvalidOperationException("Unknown provider");
        }

        public override int[] GetProvidersForTransitioner(int category) {
            if (category == (int) Categories.Head)
                return new int[] { (int) Providers.Head, (int) Providers.NextLink, (int) Providers.Tail };
            if (category == (int) Categories.Tail)
                return new int[] { (int) Providers.Head, (int) Providers.PreviousLink, (int) Providers.Tail };
            return new int[] { (int) Providers.Head, (int) Providers.Link, (int) Providers.NextLink, 
                    (int) Providers.PreviousLink, (int) Providers.Tail };
        }

        public override AttachObject ResolveProvider(int provider, Attachment attachment) {
            if (provider == (int) Providers.Head)
                return GetHead(attachment);
            if (provider == (int) Providers.Tail)
                return GetTail(attachment);
            throw new System.NotSupportedException("Ambiguous provider; use ResolveProvider(int provider, "
                    + "AttachObject reference)");
        }

        public override AttachObject ResolveProvider(int provider, AttachObject reference) {
            if (provider == (int) Providers.Link)
                return reference;
            if (provider == (int) Providers.NextLink)
                return GetNextLink(reference);
            if (provider == (int) Providers.PreviousLink)
                return GetPreviousLink(reference);
            if (provider == (int) Providers.Head)
                return GetHead(reference.attachment);
            if (provider == (int) Providers.Tail)
                return GetTail(reference.attachment);
            throw new System.InvalidOperationException("Unknown provider");
        }

        protected AttachObject GetHead(Attachment attachment) {
            return attachment.objects.Get((int) Categories.Head, 0);
        }

        protected AttachObject GetTail(Attachment attachment) {
            return attachment.objects.Get((int) Categories.Tail, 0);
        }

        protected AttachObject GetNextLink(AttachObject link) {
            if (link.category == (int) Categories.Head) {
                if (link.attachment.objects.Count((int) Categories.Links) == 0)
                    return GetTail(link.attachment);
                return link.attachment.objects.Get((int) Categories.Links, 0);
            }
            if (link.category == (int) Categories.Tail)
                return null;
            if (link.indexInCategory + 1 == link.attachment.objects.Count(category: (int) Categories.Links))
                return GetTail(link.attachment);
            return link.attachment.objects.Get((int) Categories.Links, link.indexInCategory + 1);
        }

        protected AttachObject GetPreviousLink(AttachObject link) {
            if (link.category == (int) Categories.Tail) {
                if (link.attachment.objects.Count((int) Categories.Links) == 0)
                    return GetHead(link.attachment);
                return link.attachment.objects.Get((int) Categories.Links, 
                        link.attachment.objects.Count((int) Categories.Links) - 1);
            }
            if (link.category == (int) Categories.Head)
                return null;
            int indexInCategory = link.indexInCategory - 1;
            if (indexInCategory == -1)
                return GetHead(link.attachment);
            return link.attachment.objects.Get((int) Categories.Links, indexInCategory);
        }

        protected abstract void ConnectLinks(AttachObject link, AttachObject nextLink);

        protected abstract void DisconnectLinks(AttachObject link, AttachObject nextLink);

		public override bool ConnectObject(AttachObject link) {
            AttachObject nextLink = GetNextLink(link);
            AttachObject previousLink = GetPreviousLink(link);
            if (nextLink != null && nextLink.isConnected && previousLink != null && previousLink.isConnected)
                DisconnectLinks(previousLink, nextLink);
            if (previousLink != null && previousLink.isConnected)
                ConnectLinks(previousLink, link);
            if (nextLink != null && nextLink.isConnected)
                ConnectLinks(link, nextLink);
            return true;
        }

        public override bool DisconnectObject(AttachObject link) {
            DisconnectObjectImmediate(link);
            return true;
        }

        public override void DisconnectObjectImmediate(AttachObject link) {
            AttachObject nextLink = GetNextLink(link);
            AttachObject previousLink = GetPreviousLink(link);
            if (previousLink != null && previousLink.isConnected)
                DisconnectLinks(previousLink, link);
            if (nextLink != null && nextLink.isConnected)
                DisconnectLinks(link, nextLink);
        }

        public override void OnObjectWasRemoved(Attachment attachment, AttachObject obj, int oldIndexInCategory) {
            if (obj.category != (int) Categories.Links)
                return;
            AttachObject link;
            if (oldIndexInCategory == 0)
                link = GetHead(attachment);
            else
                link = attachment.objects.Get((int) Categories.Links, oldIndexInCategory - 1);
            if (link == null || !link.isConnected)
                return;
            AttachObject nextLink = GetNextLink(link);
            if (nextLink != null && nextLink.isConnected)
                ConnectLinks(link, nextLink);
        }

	}

}