namespace SubC.Attachments {

	using UnityEngine;

    [RequireComponent(typeof(Grid))]
	[AddComponentMenu("Clingy/Grid Attach Point", 2)]
	public class GridAttachPoint : AttachPoint {

        public string inputPosition = "position";
        public string outputPosition = "gridposition";
        Param defaultInputPosition = new Param(ParamType.Vector3);

        Grid grid;

        public void Awake() {
            grid = GetComponent<Grid>();
        }

        public override void ApplyParamsForOther(AttachObject other, AttachObject self) {
            defaultInputPosition.name = inputPosition;
            Vector3 worldPos = other.resolvedParams.GetParam(defaultInputPosition)
                    .GetWorldPosition(other.seedObject, other.spriteRenderer);
            Vector3Int cell = grid.WorldToCell(worldPos);
            Vector3 translated = grid.GetCellCenterWorld(cell);
            other.resolvedParams.SetParam(new Param(translated, outputPosition));
        }

	}

}