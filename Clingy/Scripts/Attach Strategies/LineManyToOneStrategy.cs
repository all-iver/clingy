namespace SubC.Attachments {

	using UnityEngine;
    using ClingyLines;

	[CreateAssetMenu(fileName = "New Line Many-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Line Many-to-One Strategy")]
	public class LineManyToOneStrategy : ManyToOneAttachStrategy {

        [System.Serializable]
        public class ManyToOneLineRendererDescription : ClingyLines.AttachStrategyLineRendererDescription {
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector rootPositionParam = ParamSelector.Position();
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector leafPositionParam = ParamSelector.Position(provider: (int) Providers.Leaf);
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector rootWidthParam = new ParamSelector(new Param(ParamType.Float, "width"));
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector leafWidthParam = new ParamSelector(new Param(ParamType.Float, "width"),
                    (int) Providers.Leaf);
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector rootColorParam = new ParamSelector(new Param(ParamType.Color));
            [ParamSelector(new int[] { (int) Providers.Root, (int) Providers.Leaf })]
            public ParamSelector leafColorParam = new ParamSelector(new Param(ParamType.Color), (int) Providers.Leaf);

            public override ParamSelector GetPositionParamSelector(AttachObject obj) {
                if (obj.category == (int) Categories.Root)
                    return rootPositionParam;
                return leafPositionParam;
            }

            public override ParamSelector GetWidthParamSelector(AttachObject obj) {
                if (obj.category == (int) Categories.Root)
                    return rootWidthParam;
                return leafWidthParam;
            }

            public override ParamSelector GetColorParamSelector(AttachObject obj) {
                if (obj.category == (int) Categories.Root)
                    return rootColorParam;
                return leafColorParam;
            }
        }

        public ManyToOneLineRendererDescription lineRendererDescription = new ManyToOneLineRendererDescription();

        public bool hideLineRenderersInInspector = true;

        protected override void Reset() {
            base.Reset();
            lineRendererDescription.Reset();
        }

        protected override void ConnectLeaf(AttachObject root, AttachObject leaf) {
            LineObjectState state = new LineObjectState();
            state.lineState.lineRenderer = leaf.seedObject.AddComponent<LineRenderer>();
            state.objects = new AttachObject[2];
            leaf.state = state;
        }

        protected override void DisconnectLeaf(AttachObject root, AttachObject leaf) {
            LineObjectState state = (LineObjectState) leaf.state;
            DestroyImmediate(state.lineState.lineRenderer);
            state.lineState.lineRenderer = null;
            leaf.state = null;
        }

        void RefreshLineRenderer(Attachment attachment) {
            AttachObject root = GetRoot(attachment);
            if (root == null || !root.isConnected)
                return;
            AttachObjectList.Enumerator e = attachment.objects.GetEnumerator((int) Categories.Leaves, 
                    phase: AttachObjectPhase.Connected);
            while (e.MoveNext()) {
                AttachObject leaf = e.Current;
                LineObjectState state = (LineObjectState) leaf.state;
                state.objects[0] = root;
                state.objects[1] = leaf;
                ClingyLines.LineAttachStrategyUtility.RefreshLineRenderer(lineRendererDescription, state.lineState,
                        state.objects, leaf, hideLineRenderersInInspector);
            }
        }

        public override void DoLateUpdate(Attachment attachment) {
            RefreshLineRenderer(attachment);
        }

	}

}