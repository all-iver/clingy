namespace SubC.Attachments.ClingyLines {

	using UnityEngine;

	[System.Serializable]
	public class LineRendererDescription {

        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        public bool receiveShadows = true;
        public MotionVectorGenerationMode motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
        public Material[] materials;
        public bool loop;
        public bool useWorldSpace = true;
        public AnimationCurve widthCurve;
        public float widthMultiplier = 1;
        public Gradient colorGradient;
        public GradientColorKey[] colors;
        public GradientAlphaKey[] alphas;
        public int cornerVertices = 0;
        public int endCapVertices = 0;
        public LineAlignment alignment = LineAlignment.View;
        public LineTextureMode textureMode = LineTextureMode.Stretch;
        public bool generateLightingData;
        public int sortingLayerID;
        public int sortingOrder;
        public UnityEngine.Rendering.LightProbeUsage lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        public UnityEngine.Rendering.ReflectionProbeUsage reflectionProbes 
                = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        public LineRendererDescription() {
            Reset();
        }

        public virtual void Reset() {
            widthCurve = AnimationCurve.Linear(0, 0.1f, 1, 0.1f);
            materials = new Material[1];
        }

        public void UpdateLineRenderer(LineRenderer lineRenderer, Vector3[] points, Gradient colorGradient = null, 
                AnimationCurve widthCurve = null) {
            lineRenderer.widthMultiplier = widthMultiplier;
            lineRenderer.alignment = alignment;
            lineRenderer.lightProbeUsage = lightProbeUsage;
            lineRenderer.generateLightingData = generateLightingData;
            lineRenderer.useWorldSpace = useWorldSpace;
            lineRenderer.lightProbeUsage = lightProbeUsage;
            lineRenderer.textureMode = textureMode;
            lineRenderer.sortingOrder = sortingOrder;
            lineRenderer.sortingLayerID = sortingLayerID;
            lineRenderer.shadowCastingMode = shadowCastingMode;
            lineRenderer.receiveShadows = receiveShadows;
            lineRenderer.loop = loop;
            lineRenderer.motionVectorGenerationMode = motionVectorGenerationMode;
            lineRenderer.colorGradient = colorGradient ?? this.colorGradient;
            lineRenderer.materials = materials;
            lineRenderer.numCornerVertices = cornerVertices;
            lineRenderer.numCapVertices = endCapVertices;
            lineRenderer.positionCount = points.Length;
            lineRenderer.widthCurve = widthCurve ?? this.widthCurve;
            lineRenderer.SetPositions(points);
        }
	}

    // for use when we just have one LineRenderer for the entire strategy and want to put the LineRenderer on a new
    // GameObject rather than on one of our AttachObjects
    public class LineStrategyState : AttachStrategyState {
        public LineState lineState = new LineState();
        public AttachObject[] objects;
    }

    // for use when we want to put the LineRenderer on an AttachObject
    public class LineObjectState : AttachObjectState {
        public LineState lineState = new LineState();
        public AttachObject[] objects;
    }

    public class LineState {
        public LineRenderer lineRenderer;
        public Vector3[] points;
        public Gradient colorGradient;
        public AnimationCurve widthCurve;
    }

    [System.Serializable]
    public abstract class AttachStrategyLineRendererDescription : LineRendererDescription {
        public bool perObjectWidth = false;
        public bool perObjectColor = false;

        public abstract ParamSelector GetPositionParamSelector(AttachObject obj);
        public abstract ParamSelector GetWidthParamSelector(AttachObject obj);
        public abstract ParamSelector GetColorParamSelector(AttachObject obj);
    }

    public static class LineAttachStrategyUtility {

        public static void RefreshLineRenderer(AttachStrategyLineRendererDescription lineRendererDescription, 
                LineState state, AttachObject[] objects, AttachObject reference = null, 
                bool hideLineRendererInInspector = true) {
            AttachStrategyLineRendererDescription lrd = lineRendererDescription;
            Attachment attachment = objects[0].attachment;
            if (state.points == null || state.points.Length != objects.Length)
                state.points = new Vector3[objects.Length];
            state.widthCurve = new AnimationCurve();
            state.colorGradient = new Gradient();
            GradientColorKey[] colors = new GradientColorKey[objects.Length];
            GradientAlphaKey[] alphas = new GradientAlphaKey[objects.Length];
            int i = 0;
            foreach (AttachObject obj in objects) {
                ParamSelector selector = lrd.GetPositionParamSelector(obj);
                state.points[i] = selector.GetWorldPosition(attachment, reference ?? obj);
                if (lrd.perObjectWidth) {
                    selector = lrd.GetWidthParamSelector(obj);
                    state.widthCurve.AddKey(i / (objects.Length - 1), 
                            selector.GetParam(attachment, reference ?? obj).floatValue);
                }
                if (lrd.perObjectColor) {
                    selector = lrd.GetColorParamSelector(obj);
                    colors[i] = new GradientColorKey(selector.GetParam(attachment, reference ?? obj).colorValue, 
                            i / (objects.Length - 1));
                    alphas[i] = new GradientAlphaKey(selector.GetParam(attachment, reference ?? obj).colorValue.a, 
                            i / (objects.Length - 1));
                }
                i ++;
            }
            if (lrd.perObjectColor)
                state.colorGradient.SetKeys(colors, alphas);
            lrd.UpdateLineRenderer(state.lineRenderer, state.points, lrd.perObjectColor ? state.colorGradient : null, 
                    lrd.perObjectWidth ? state.widthCurve : null);
            state.lineRenderer.hideFlags = hideLineRendererInInspector ? HideFlags.HideInInspector : HideFlags.None;
        }
 
    }

}