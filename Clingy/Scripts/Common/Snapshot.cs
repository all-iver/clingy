namespace SubC.Attachments {

	using UnityEngine;
    using System;

    public class Snapshot {
        public delegate void SnapshotCompleteDelegate();

		public struct SnapshotData {
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 scale;
			public bool hasRigidbody;
			public RigidbodyType2D bodyType;
			public PhysicsMaterial2D sharedMaterial;
			public bool simulated;
			public bool useFullKinematicContacts;
			public bool useAutoMass;
			public float mass, gravityScale, drag, angularDrag;
			public RigidbodyInterpolation2D interpolation;
			public RigidbodyConstraints2D constraints;
			public RigidbodySleepMode2D sleepMode;
			public CollisionDetectionMode2D collisionDetectionMode;
			public int layer;
			public int sortingLayerID, sortingOrder;
			public bool flipX, flipY;
		}

		public GameObject target;
		public SpriteRenderer spriteRenderer;
		public SnapshotRestoreOptions options = new SnapshotRestoreOptions();
		public SnapshotData data = new SnapshotData();
		public bool captured { get { return capturedTimestamp != 0; } }
        public int capturedTimestamp;
        public bool tweening { get { return (positionTweener != null && positionTweener.tweening) 
                || (rotationTweener != null && rotationTweener.tweening); } }
        protected bool tweenFirst;
        Tweener positionTweener, rotationTweener;

        public Snapshot() {
        }

		public Snapshot(SnapshotRestoreOptions options, GameObject target, 
                SpriteRenderer spriteRenderer = null) {
            this.options = options;
			this.target = target;
			this.spriteRenderer = spriteRenderer;
        }

		public Snapshot(GameObject target, SpriteRenderer spriteRenderer = null) {
			this.target = target;
			this.spriteRenderer = spriteRenderer;
		}

        // for the unity editor
		public void Reset() {
            options.Reset();
			data = new SnapshotData();
			capturedTimestamp = 0;
		}

        public SpriteRenderer GetSpriteRenderer() {
            return spriteRenderer ? spriteRenderer : this.target.GetComponent<SpriteRenderer>();
        }

		public void Capture() {
			data.layer = target.gameObject.layer;
			data.position = target.transform.position;
			data.rotation = target.transform.rotation;
			data.scale = target.transform.lossyScale;
            SpriteRenderer sr = GetSpriteRenderer();
			if (sr) {
				data.sortingLayerID = sr.sortingLayerID;
				data.sortingOrder = sr.sortingOrder;
				data.flipX = sr.flipX;
				data.flipY = sr.flipY;
			}
			Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
			data.hasRigidbody = rb != null;
			if (data.hasRigidbody) {
				data.useAutoMass = rb.useAutoMass;
				data.mass = rb.mass;
				data.drag = rb.drag;
				data.angularDrag = rb.angularDrag;
				data.gravityScale = rb.gravityScale;
				data.bodyType = rb.bodyType;
				data.simulated = rb.simulated;
				data.useFullKinematicContacts = rb.useFullKinematicContacts;
				data.interpolation = rb.interpolation;
				data.sleepMode = rb.sleepMode;
				data.collisionDetectionMode = rb.collisionDetectionMode;
				data.constraints = rb.constraints;
				data.sharedMaterial = rb.sharedMaterial;
			}
            // https://stackoverflow.com/questions/9453101/how-do-i-get-epoch-time-in-c
			capturedTimestamp = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		static public Snapshot TakeSnapshot(SnapshotRestoreOptions options, GameObject target, 
				SpriteRenderer spriteRenderer = null) {
			Snapshot snapshot = new Snapshot(options, target, spriteRenderer);
			snapshot.Capture();
			return snapshot;
		}

        public void Cancel(bool completeNonTweenables = true) {
            if (!tweening)
                return;
            if (positionTweener != null)
                positionTweener.Cancel();
            if (rotationTweener != null)
                rotationTweener.Cancel();
            positionTweener = rotationTweener = null;
            if (completeNonTweenables && tweenFirst)
                ApplyNonTweenables();
        }

        public void Apply(GetDestinationPositionDelegate GetDestinationPosition = null,
                GetDestinationRotationDelegate GetDestinationRotation = null, 
                SnapshotCompleteDelegate SnapshotComplete = null, bool tweenFirst = false) {
            if (tweening)
                throw new System.InvalidOperationException("Already applying.");
			if (!target)
                throw new System.InvalidOperationException("No target for snapshot.");

            this.tweenFirst = tweenFirst;

            SnapshotCompleteDelegate FinishIfAllTweensComplete = () => {
                if (tweening)
                    return; 
                positionTweener = rotationTweener = null;
                if (tweenFirst)
                    ApplyNonTweenables();
                if (SnapshotComplete != null)
                    SnapshotComplete();
            };

            if (!tweenFirst)
                ApplyNonTweenables();

            bool starting = true;

            // fixme - need to make it an option to translate position before rotation or vice versa I guess...
            if (options.restoreRotation) {
                if (GetDestinationRotation == null)
                    GetDestinationRotation = () => data.rotation;
                rotationTweener = Tweener.TweenRotation(target, options.tweenRotationOptions, options.rotateMethod, 
                        GetDestinationRotation, 
                    TweenComplete: () => {
                        if (!starting)
                            FinishIfAllTweensComplete();
                    }, TweenCancelled: () => {
                        rotationTweener = null;
                    }
                );
            }

            if (options.restorePosition) {
                if (GetDestinationPosition == null)
                    GetDestinationPosition = () => data.position;
                positionTweener = Tweener.TweenPosition(target, options.tweenPositionOptions, options.moveMethod, 
                        GetDestinationPosition, 
                    TweenComplete: () => {
                        if (!starting)
                            FinishIfAllTweensComplete();
                    }, TweenCancelled: () => {
                        positionTweener = null;
                    }
                );
            }

            starting = false;
            FinishIfAllTweensComplete();
        }

        protected void ApplyNonTweenables() {
			if (!target)
                throw new System.InvalidOperationException("No target for snapshot.");
			// Rigidbody rb = target.GetComponent<Rigidbody>(); // todo
			Rigidbody2D rb2D = target.GetComponent<Rigidbody2D>();
			if (options.restoreLayer)
				target.layer = data.layer; 
            SpriteRenderer sr = GetSpriteRenderer();
			if (options.restoreSortingOrder && sr) {
				sr.sortingLayerID = data.sortingLayerID;
				sr.sortingOrder = data.sortingOrder;
			}
			if (options.restoreFlipX && sr)
				sr.flipX = data.flipX;
			if (options.restoreFlipY && sr)
				sr.flipY = data.flipY;
			if (options.restoreScale) {
                Transform oldParent = target.transform.parent;
                target.transform.SetParent(null);
				target.transform.localScale = data.scale;
                target.transform.SetParent(oldParent);
            }
			if (options.rigidbodyBehavior == RigidbodyRestoreBehavior.Restore) {
				if (!data.hasRigidbody) {
					if (rb2D) {
						// avoid an extra physics frame after destroying
						rb2D.simulated = false;
						GameObject.Destroy(rb2D);
					}
				} else {
					if (!rb2D)
						rb2D = target.gameObject.AddComponent<Rigidbody2D>();
					rb2D.useAutoMass = data.useAutoMass;
					if (!data.useAutoMass)
						rb2D.mass = data.mass;
					rb2D.drag = data.drag;
					rb2D.angularDrag = data.angularDrag;
					rb2D.gravityScale = data.gravityScale;
					rb2D.bodyType = data.bodyType;
					rb2D.simulated = data.simulated;
					rb2D.useFullKinematicContacts = data.useFullKinematicContacts;
					rb2D.interpolation = data.interpolation;
					rb2D.sleepMode = data.sleepMode;
					rb2D.collisionDetectionMode = data.collisionDetectionMode;
					rb2D.constraints = data.constraints;
					rb2D.sharedMaterial = data.sharedMaterial;
				}
			} else if (options.rigidbodyBehavior == RigidbodyRestoreBehavior.Desimulate) {
				if (rb2D) {
                    rb2D.velocity = Vector2.zero;
                    rb2D.angularVelocity = 0;
					rb2D.simulated = false;
                }
			} else if (options.rigidbodyBehavior == RigidbodyRestoreBehavior.Destroy) {
				if (rb2D) {
					// avoid an extra physics frame after destroying
					rb2D.simulated = false;
					// warning - this gives errors and possibly crashes unity in some cases.  my case is using a 
                    // following attachment with a 2d collision auto-attach and a static rigidbody2d.  seems like 
                    // having contacts exist and then deleting the rigidbody2d, leaving behind a static collider, 
                    // causes problems for unity.  it should not be a common use-case though.  
                    // update - Unity says they fixed it in 5.6.0b5
					GameObject.Destroy(rb2D);
				}
			} else if (options.rigidbodyBehavior == RigidbodyRestoreBehavior.SetDynamic) {
				if (rb2D) {
					rb2D.bodyType = RigidbodyType2D.Dynamic;
					rb2D.simulated = true;
				}
			} else if (options.rigidbodyBehavior == RigidbodyRestoreBehavior.SetKinematic) {
				if (rb2D) {
                    rb2D.velocity = Vector2.zero;
                    rb2D.angularVelocity = 0;
					rb2D.bodyType = RigidbodyType2D.Kinematic;
					rb2D.simulated = true;
				}
			}
        }

    }

}
