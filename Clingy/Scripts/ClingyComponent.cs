namespace SubC.Attachments {

    using UnityEngine;
    using System.Collections.Generic;

    [AddComponentMenu("")]
    public class ClingyComponent : MonoBehaviour {

        public struct ExecutionOrder {
            static int counter;
            public int executionOrder;
            public int id;

            public ExecutionOrder(int executionOrder) {
                counter ++;
                id = counter;
                this.executionOrder = executionOrder;
            }
        }

        class ExecutionOrderComparer : IComparer<ExecutionOrder> {
            public int Compare(ExecutionOrder a, ExecutionOrder b) {
                if (a.executionOrder == b.executionOrder)
                    return a.id.CompareTo(b.id);
                return a.executionOrder.CompareTo(b.executionOrder);
            }
        }

        public SortedList<ExecutionOrder, Attachment> attachments 
                = new SortedList<ExecutionOrder, Attachment>(new ExecutionOrderComparer());

        static ClingyComponent _instance;
        public static ClingyComponent instance {
            get {
                if (!_instance) {
                    GameObject go = new GameObject();
                    go.name = "[Clingy]";
                    Object.DontDestroyOnLoad(go);
                    _instance = go.AddComponent<ClingyComponent>();
                }
                return _instance;
            }
        }

        void FixedUpdate() {
            foreach (Attachment a in attachments.Values)
                a.DoFixedUpdate();
        }

        void Update() {
            foreach (Attachment a in attachments.Values)
                a.DoUpdate();
        }

        void LateUpdate() {
            foreach (Attachment a in attachments.Values)
                a.DoLateUpdate();
        }

        public GameObject CreateGameObject() {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            return go;
        }

    }

}