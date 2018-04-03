namespace SubC.Attachments {

    using UnityEngine;
    using System;
    using System.Collections;

    public enum TweenMethod {
        None,
        Speed,
        Time
    }

    [System.Serializable]
    public struct TweenOptions {
        public TweenMethod tweenMethod;
        public float speed;
        public float duration;
        // public UpdateMethod updateMethod;
        public EaseType easing;
        public float delay;
        public bool completeOnCancel;
        public bool dynamicTarget;

        public void Reset() {
            easing = EaseType.Linear;
            speed = 1;
            duration = 1;
            dynamicTarget = true;
        }
    }

    public delegate void TweenCompleteDelegate();
    public delegate void TweenCancelledDelegate();
    public delegate Vector3 GetDestinationPositionDelegate();
    public delegate Quaternion GetDestinationRotationDelegate();

    public abstract class Tweener {
        protected bool cancelled;
        Coroutine _coroutine;
        protected Coroutine coroutine { 
            get { return _coroutine; } 
            set { 
                if (_coroutine != null && value != null)
                    throw new System.InvalidOperationException(
                            "Attempting to set a coroutine when one already exists.");
                _coroutine = value;
            }
        }
        protected TweenCompleteDelegate TweenComplete;
        protected TweenCancelledDelegate TweenCancelled;
        protected GameObject gameObject;
        protected TweenOptions tweenOptions;
        public bool tweening { get { return coroutine != null; } }

        public Tweener(GameObject gameObject, TweenOptions tweenOptions, 
                TweenCompleteDelegate TweenComplete = null, TweenCancelledDelegate TweenCancelled = null) {
            this.gameObject = gameObject;
            this.tweenOptions = tweenOptions;
            this.TweenComplete = TweenComplete;
            this.TweenCancelled = TweenCancelled;
        }

        protected abstract IEnumerator Tween();

        public void Start() {
            if (tweening)
                throw new System.InvalidOperationException("Tween is already started.");
            coroutine = ClingyComponent.instance.StartCoroutine(Tween());
            // note - coroutine may be null if the tween completes on the first iteration
        }

        protected abstract void CompleteTween();

        protected void Complete() {
            // note - coroutine will == null if the tween completes on the first iteration
            coroutine = null;
            if (TweenComplete != null)
                TweenComplete();
        }

        public void Cancel() {
            if (!tweening)
                return;
            ClingyComponent.instance.StopCoroutine(coroutine);
            coroutine = null;
            if (tweenOptions.completeOnCancel)
                CompleteTween();
            if (TweenCancelled != null)
                TweenCancelled();
        }

        public static Tweener TweenPosition(GameObject gameObject, TweenOptions tweenOptions, 
                MoveMethod moveMethod, GetDestinationPositionDelegate GetDestinationPosition, 
                TweenCompleteDelegate TweenComplete = null, TweenCancelledDelegate TweenCancelled = null) {
            Tweener tweener = new ClingyPositionTweener(gameObject, tweenOptions, moveMethod, 
                    GetDestinationPosition, TweenComplete, TweenCancelled);
            tweener.Start();
            return tweener;
        }

        public static Tweener TweenRotation(GameObject gameObject, TweenOptions tweenOptions, 
                RotateMethod rotateMethod, GetDestinationRotationDelegate GetDestinationRotation, 
                TweenCompleteDelegate TweenComplete = null, TweenCancelledDelegate TweenCancelled = null) {
            Tweener tweener = new ClingyRotationTweener(gameObject, tweenOptions, rotateMethod, 
                    GetDestinationRotation, TweenComplete, TweenCancelled);
            tweener.Start();
            return tweener;
        }
    }
    
    public class ClingyPositionTweener : Tweener {

        protected MoveMethod moveMethod;
        protected GetDestinationPositionDelegate GetDestinationPosition;
        protected Rigidbody rb;
        protected Rigidbody2D rb2D;
        protected Vector3 firstDestination;

        public ClingyPositionTweener(GameObject gameObject, TweenOptions tweenOptions, 
                MoveMethod moveMethod, GetDestinationPositionDelegate GetDestinationPosition, 
                TweenCompleteDelegate TweenComplete = null, TweenCancelledDelegate TweenCancelled = null) 
                : base(gameObject, tweenOptions, TweenComplete, TweenCancelled) {
            this.moveMethod = moveMethod;
            this.GetDestinationPosition = GetDestinationPosition;
        }

        protected override void CompleteTween() {
            Vector3 destination = tweenOptions.dynamicTarget ? GetDestinationPosition() : firstDestination;
            ClingyUtils.MovePosition(destination, gameObject.transform, moveMethod, rb, rb2D);
        }

        protected override IEnumerator Tween() {
            if (tweenOptions.delay > 0)
                yield return new WaitForSeconds(tweenOptions.delay);

            Transform transform = gameObject.transform;
            rb = gameObject.GetComponent<Rigidbody>();
            rb2D = gameObject.GetComponent<Rigidbody2D>();

            Vector3 startPosition = transform.position;
            float timer = 0;
            float duration = Mathf.Max(0, tweenOptions.duration);
            float speed = Mathf.Max(0.00001f, tweenOptions.speed);
            Easing.EasingFunction ease = Easing.FromEaseType(tweenOptions.easing);
            firstDestination = GetDestinationPosition();

            bool finished = false;
            while (true) {
                Vector3 current = transform.position;
                Vector3 destination = tweenOptions.dynamicTarget ? GetDestinationPosition() : firstDestination;
                Vector3 nextPosition = destination;
                if (tweenOptions.tweenMethod == TweenMethod.Speed && timer == 0)
                    duration = (destination - startPosition).magnitude / speed;
                if (tweenOptions.tweenMethod != TweenMethod.None && duration > 0) {
                    timer = Mathf.Min(timer + Time.deltaTime, duration);
                    float t = ease(timer, 0, 1, duration);
                    nextPosition = Vector3.LerpUnclamped(startPosition, destination, t);
                    if (timer == duration)
                        finished = true;
                } else {
                    finished = true;
                }

                ClingyUtils.MovePosition(nextPosition, transform, moveMethod, rb, rb2D);

                if (finished)
                    break;
                yield return null;
            }

            Complete();
        }

    }

    public class ClingyRotationTweener : Tweener {

        protected RotateMethod rotateMethod;
        protected GetDestinationRotationDelegate GetDestinationRotation;
        protected Rigidbody rb;
        protected Rigidbody2D rb2D;
        protected Quaternion firstDestination;

        public ClingyRotationTweener(GameObject gameObject, TweenOptions tweenOptions, 
                RotateMethod rotateMethod, GetDestinationRotationDelegate GetDestinationRotation, 
                TweenCompleteDelegate TweenComplete = null, TweenCancelledDelegate TweenCancelled = null) 
                : base(gameObject, tweenOptions, TweenComplete, TweenCancelled) {
            this.rotateMethod = rotateMethod;
            this.GetDestinationRotation = GetDestinationRotation;
        }

        protected override void CompleteTween() {
            Quaternion destination = tweenOptions.dynamicTarget ? GetDestinationRotation() : firstDestination;
            ClingyUtils.MoveRotation(destination, gameObject.transform, rotateMethod, rb, rb2D);
        }

        protected override IEnumerator Tween() {
            if (tweenOptions.delay > 0)
                yield return new WaitForSeconds(tweenOptions.delay);

            Transform transform = gameObject.transform;
            rb = gameObject.GetComponent<Rigidbody>();
            rb2D = gameObject.GetComponent<Rigidbody2D>();

            Quaternion startRotation = transform.rotation;
            float timer = 0;
            float duration = Mathf.Max(0, tweenOptions.duration);
            float speed = Mathf.Max(0.00001f, tweenOptions.speed);
            Easing.EasingFunction ease = Easing.FromEaseType(tweenOptions.easing);
            firstDestination = GetDestinationRotation();

            bool finished = false;
            while (true) {
                Quaternion current = transform.rotation;
                Quaternion destination = tweenOptions.dynamicTarget ? GetDestinationRotation() : firstDestination;
                Quaternion nextRotation = destination;
                if (tweenOptions.tweenMethod == TweenMethod.Speed && timer == 0)
                    duration = Quaternion.Angle(startRotation, destination) / speed;
                if (tweenOptions.tweenMethod != TweenMethod.None && duration > 0) {
                    timer = Mathf.Min(timer + Time.deltaTime, duration);
                    float t = ease(timer, 0, 1, duration);
                    nextRotation = Quaternion.LerpUnclamped(startRotation, destination, t);
                    if (timer == duration)
                        finished = true;
                } else {
                    finished = true;
                }

                ClingyUtils.MoveRotation(nextRotation, transform, rotateMethod, rb, rb2D);

                if (finished)
                    break;
                yield return null;
            }

            Complete();
        }

    }

}