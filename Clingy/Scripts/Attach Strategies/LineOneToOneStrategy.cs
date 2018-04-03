namespace SubC.Attachments {

	using UnityEngine;
    using ClingyLines;

	[CreateAssetMenuAttribute(fileName = "New Line One-to-One Strategy", 
            menuName = "Clingy/Attach Strategies/Line One-to-One Strategy")]
	public class LineOneToOneStrategy : OneToOneAttachStrategy {

        [System.Serializable]
        public class OneToOneLineRendererDescription : ClingyLines.AttachStrategyLineRendererDescription {
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector position1Param = ParamSelector.Position();
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector position2Param = ParamSelector.Position(provider: (int) Providers.Object2);
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector width1Param = new ParamSelector(new Param(ParamType.Float, "width"));
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector width2Param = new ParamSelector(new Param(ParamType.Float, "width"),
                    (int) Providers.Object2);
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector color1Param = new ParamSelector(new Param(ParamType.Color));
            [ParamSelector(new int[] { (int) Providers.Object1, (int) Providers.Object2 })]
            public ParamSelector color2Param = new ParamSelector(new Param(ParamType.Color), (int) Providers.Object2);

            public override ParamSelector GetPositionParamSelector(AttachObject obj) {
                if (obj.category == (int) Categories.Object1)
                    return position1Param;
                return position2Param;
            }

            public override ParamSelector GetWidthParamSelector(AttachObject obj) {
                if (obj.category == (int) Categories.Object1)
                    return width1Param;
                return width2Param;
            }

            public override ParamSelector GetColorParamSelector(AttachObject obj) {
                if (obj.category == (int) Categories.Object1)
                    return color1Param;
                return color2Param;
            }
        }

        public OneToOneLineRendererDescription lineRendererDescription = new OneToOneLineRendererDescription();

        public bool hideLineRendererInInspector = true;

        protected override void Reset() {
            base.Reset();
            lineRendererDescription.Reset();
        }

		protected override void ConnectBoth(AttachObject obj1, AttachObject obj2) {
            LineObjectState state = new LineObjectState();
            state.lineState.lineRenderer = obj1.seedObject.AddComponent<LineRenderer>();
            state.objects = new AttachObject[2] { obj1, obj2 };
            obj1.state = state;
		}

        protected override void DisconnectBoth(AttachObject obj1, AttachObject obj2) {
            LineObjectState state = (LineObjectState) obj1.state;
            Destroy(state.lineState.lineRenderer);
            state.lineState.lineRenderer = null;
        }

        public override void DoLateUpdate(Attachment attachment) {
            RefreshLineRenderer(attachment);
        }

        void RefreshLineRenderer(Attachment attachment) {
            if (!IsConnected(attachment))
                return;
            LineObjectState state = (LineObjectState) GetObject1(attachment).state;
            ClingyLines.LineAttachStrategyUtility.RefreshLineRenderer(lineRendererDescription, state.lineState, 
                    state.objects, null, hideLineRendererInInspector);
        }

	}

}