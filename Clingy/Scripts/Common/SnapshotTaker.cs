namespace SubC.Attachments {

	using UnityEngine;

    [AddComponentMenu("Clingy/Snapshot Taker", 4)]
	public class SnapshotTaker : MonoBehaviour {

		public GameObject target;
		public SpriteRenderer spriteRenderer;
		public bool captureOnStart;
		public SnapshotRestoreOptions restoreOptions;
		[HideInInspector]
		public Snapshot snapshot;

		void Start() {
			if (captureOnStart)
				Capture();
		}

		public bool HasSnapshot() {
			return snapshot.captured && snapshot.target == GetTarget();
		}

		public void Capture() {
			snapshot = Snapshot.TakeSnapshot(restoreOptions, GetTarget(), GetSpriteRenderer());
		}

        // fixme - this is an inspector method but doesn't seem to be used like one
		public void Reset() {
			snapshot.Reset();
		}

		public GameObject GetTarget() {
			return target == null ? gameObject : target;
		}

		protected SpriteRenderer GetSpriteRenderer() {
			return spriteRenderer == null ? GetTarget().GetComponent<SpriteRenderer>() : spriteRenderer;
		}

		public void Restore() {
			if (!HasSnapshot())
				throw new System.InvalidOperationException(); // fixme
			snapshot.Apply();
		}

	}

}