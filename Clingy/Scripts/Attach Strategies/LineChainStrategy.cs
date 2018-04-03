namespace SubC.Attachments {

	using UnityEngine;
    using System.Collections.Generic;
    using ClingyLines;

	[CreateAssetMenu(fileName = "New Line Chain Strategy", menuName = "Clingy/Attach Strategies/Line Chain Strategy")]
	public class LineChainStrategy : ChainAttachStrategy {

        [System.Serializable]
        public class ChainLineRendererDescription : AttachStrategyLineRendererDescription {
            [ParamSelector(new int[] { (int) Providers.Link })]
            public ParamSelector positionParam = ParamSelector.Position();
            [ParamSelector(new int[] { (int) Providers.Link })]
            public ParamSelector widthParam = new ParamSelector(new Param(ParamType.Float, "width"));
            [ParamSelector(new int[] { (int) Providers.Link })]
            public ParamSelector colorParam = new ParamSelector(new Param(ParamType.Color));

            public override ParamSelector GetPositionParamSelector(AttachObject obj) {
                return positionParam;
            }

            public override ParamSelector GetWidthParamSelector(AttachObject obj) {
                return widthParam;
            }

            public override ParamSelector GetColorParamSelector(AttachObject obj) {
                return colorParam;
            }
        }

        public ChainLineRendererDescription lineRendererDescription = new ChainLineRendererDescription();

        public bool hideLineRendererInInspector = true;

        protected override void Reset() {
            base.Reset();
            lineRendererDescription.Reset();
        }

        public override AttachStrategyState CreateEmptyState() {
            LineStrategyState state = new LineStrategyState();
            GameObject go = ClingyComponent.instance.CreateGameObject();
            state.lineState.lineRenderer = go.AddComponent<LineRenderer>();
            return state;
        }

        public override void DestroyState(AttachStrategyState _state) {
            LineStrategyState state = (LineStrategyState) _state;
            Destroy(state.lineState.lineRenderer.gameObject);
            state.lineState.lineRenderer = null;
        }

        protected override void ConnectLinks(AttachObject link, AttachObject nextLink) {
        }

        protected override void DisconnectLinks(AttachObject link, AttachObject nextLink) {
        }

        void RefreshLineRenderer(Attachment attachment) {
            if (attachment.objects.Count(phase: AttachObjectPhase.Connected) != attachment.objects.Count())
                return;
            LineStrategyState state = (LineStrategyState) attachment.strategyState;
            if (state.objects == null || state.objects.Length != attachment.objects.Count())
                state.objects = new AttachObject[attachment.objects.Count()];
            int i = 0;
            for (int c = 0; c < GetCategories().Length; c++) {
                AttachObjectList.Enumerator e = attachment.objects.GetEnumerator(c);
                while (e.MoveNext()) {
                    state.objects[i] = e.Current;
                    i ++;
                }
            }
            LineAttachStrategyUtility.RefreshLineRenderer(lineRendererDescription, state.lineState, state.objects, 
                    null, hideLineRendererInInspector);
        }

        public override void DoLateUpdate(Attachment attachment) {
            RefreshLineRenderer(attachment);
        }

	}

}