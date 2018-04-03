namespace SubC.Attachments {

    using UnityEngine;

    [System.Serializable]
    public class ParamSelector {
        public Param defaultParam;
        public int provider, relativeToProvider;
        [SerializeField]
        ParamRelativityType relativityType = ParamRelativityType.None;
        public ParamNormalRelativity relativeTo = ParamNormalRelativity.Local;
        // options for vector3/position params
        [SerializeField]
        #pragma warning disable 0414
        bool showPositionOptions;
        public PositionTransformType useTransform = PositionTransformType.Full;
        public bool useSpriteFlip = true;

        public ParamSelector(Param defaultParam, int provider = 0, int relativeToProvider = 0, 
                ParamRelativityType relativityType = ParamRelativityType.None, 
                ParamNormalRelativity relativeTo = ParamNormalRelativity.Local) {
            this.defaultParam = defaultParam;
            this.provider = provider;
            this.relativeToProvider = relativeToProvider;
            this.relativityType = relativityType;
            this.relativeTo = relativeTo;
        }

        public static ParamSelector Position(string name = "position", int provider = 0, 
                Vector3 defaultValue = default(Vector3)) {
            Param def = new Param(defaultValue, name);
            ParamSelector ps = new ParamSelector(def, provider, relativityType: ParamRelativityType.Normal);
            ps.showPositionOptions = true;
            return ps;
        }

        public static ParamSelector Rotation(string name = "rotation", int provider = 0,
                Quaternion? defaultValue = null) {
            Param def = new Param(defaultValue ?? Quaternion.identity, name);
            ParamSelector ps = new ParamSelector(def, provider, relativityType: ParamRelativityType.Normal);
            return ps;
        }

        public static ParamSelector Direction(string name = "direction", int provider = 0, 
                Vector3 defaultValue = default(Vector3)) {
            Param def = new Param(defaultValue, name);
            ParamSelector ps = new ParamSelector(def, provider, relativityType: ParamRelativityType.Normal);
            return ps;
        }

        public AttachObject GetProvider(Attachment attachment, AttachObject reference = null) {
            if (reference == null)
                return attachment.strategy.ResolveProvider(this.provider, attachment);
            return attachment.strategy.ResolveProvider(this.provider, reference);
        }

        public AttachObject GetRelativeToProvider(Attachment attachment, AttachObject reference = null) {
            if (reference == null)
                return attachment.strategy.ResolveProvider(this.relativeToProvider, attachment);
            return attachment.strategy.ResolveProvider(this.relativeToProvider, reference);
        }

        public Param GetParam(Attachment attachment, AttachObject reference = null) {
            AttachObject provider = GetProvider(attachment, reference);
            return provider.resolvedParams.GetParam(defaultParam);
        }

        public Vector3 GetWorldPosition(Attachment attachment, AttachObject reference = null) {
            if (defaultParam.type != ParamType.Vector3)
                throw new System.InvalidOperationException("Param type is not Vector3");
            AttachObject provider = GetProvider(attachment, reference);
            if (provider == null)
                throw new System.InvalidOperationException("No provider found");
            Param param = provider.resolvedParams.GetParam(defaultParam);
            if (relativityType == ParamRelativityType.None)
                return param.vector3Value;
            if (relativityType == ParamRelativityType.Normal) {
                if (relativeTo == ParamNormalRelativity.World)
                    return param.vector3Value;
                AttachObject relativeToObject = provider;
                if (relativeTo == ParamNormalRelativity.Object)
                    relativeToObject = GetRelativeToProvider(attachment, reference);
                return param.GetWorldPosition(relativeToObject.paramsRelativeTo, 
                        useSpriteFlip ? relativeToObject.spriteRenderer : null, useTransform: useTransform);
            }
            throw new System.NotImplementedException("Param relativity type not supported for this operation");
        }

        public Quaternion GetWorldRotation(Attachment attachment, AttachObject reference = null) {
            if (defaultParam.type != ParamType.Rotation)
                throw new System.InvalidOperationException("Param type is not Rotation");
            AttachObject provider = GetProvider(attachment, reference);
            Param param = provider.resolvedParams.GetParam(defaultParam);
            if (relativityType == ParamRelativityType.None)
                return param.quaternionValue;
            if (relativityType == ParamRelativityType.Normal) {
                if (relativeTo == ParamNormalRelativity.World)
                    return param.quaternionValue;
                AttachObject relativeToObject = provider;
                if (relativeTo == ParamNormalRelativity.Object)
                    relativeToObject = GetRelativeToProvider(attachment, reference);
                return param.GetWorldRotation(relativeToObject.paramsRelativeTo);
            }
            throw new System.NotImplementedException("Param relativity type not supported for this operation");
       }

        public Vector3 GetWorldDirection(Attachment attachment, AttachObject reference = null) {
            if (defaultParam.type != ParamType.Vector3)
                throw new System.InvalidOperationException("Param type is not Vector3");
            AttachObject provider = GetProvider(attachment, reference);
            Param param = provider.resolvedParams.GetParam(defaultParam);
            if (relativityType == ParamRelativityType.None)
                return param.vector3Value;
            if (relativityType == ParamRelativityType.Normal) {
                if (relativeTo == ParamNormalRelativity.World)
                    return param.vector3Value;
                AttachObject relativeToObject = provider;
                if (relativeTo == ParamNormalRelativity.Object)
                    relativeToObject = GetRelativeToProvider(attachment, reference);
                return param.GetWorldDirection(relativeToObject.paramsRelativeTo);
            }
            throw new System.NotImplementedException("Param relativity type not supported for this operation");
        }

    }

}