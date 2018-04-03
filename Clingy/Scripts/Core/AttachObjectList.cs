namespace SubC.Attachments {

    using System;
    using System.Collections;
    using System.Collections.Generic;
	using UnityEngine;

    [Serializable]
	public class AttachObjectList {

        public struct Enumerator {
            AttachObjectList outer;
            int category;
            AttachObjectPhase phase;
            int currentIndex;
            
            public AttachObject Current { get { return outer.objects[currentIndex]; } }

            internal Enumerator(AttachObjectList outer, int category = -1, 
                    AttachObjectPhase phase = AttachObjectPhase.Any) {
                this.outer = outer;
                this.category = category;
                this.phase = phase;
                currentIndex = -1;
            }

            public bool MoveNext() {
                while (true) {
                    currentIndex ++;
                    if (currentIndex >= outer.objects.Count)
                        return false;
                    AttachObject obj = outer.objects[currentIndex];
                    if ((category == -1 || obj.category == category) 
                            && (phase == AttachObjectPhase.Any || obj.phase == phase))
                        break;
                }
                return true;
            }

            public void Reset() {
                this.currentIndex = -1;
            }
        }

        public AttachObjectListEvent OnChanged = new AttachObjectListEvent();

        [SerializeField]
        List<AttachObject> objects = new List<AttachObject>();

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        public Enumerator GetEnumerator(int category = -1, AttachObjectPhase phase = AttachObjectPhase.Any) {
            return new Enumerator(this, category, phase);
        }

        public int Count(int category = -1, AttachObjectPhase phase = AttachObjectPhase.Any) {
            int c = 0;
            Enumerator e = GetEnumerator(category, phase);
            while (e.MoveNext())
                c++;
            return c;
        }

        int GetFlattenedIndex(int category, int indexInCategory) {
            int count = 0;
            for (int i = 0; i < objects.Count; i++) {
                if (objects[i].category == category) {
                    if (count == indexInCategory)
                        return i;
                    count ++;
                }
            }
            throw new ArgumentOutOfRangeException("Index is out of bounds.");
        }

        int GetFlattenedIndex(AttachObject obj) {
            return GetFlattenedIndex(obj.category, GetIndexInCategory(obj));
        }

        public AttachObject Get(int category, int indexInCategory) {
            int c = 0;
            Enumerator e = GetEnumerator(category);
            while (e.MoveNext()) {
                if (c == indexInCategory)
                    return e.Current;
                c ++;
            }
            return null;
        }

        public int GetIndexInCategory(AttachObject obj) {
            int c = 0;
            for (int i = 0; i < objects.Count; i++) {
                if (obj == objects[i])
                    return c;
                if (obj.category == objects[i].category)
                    c ++;
            }
            throw new ArgumentException("Object not found in category.");
        }

        int GetIndexInCategory(int flattenedIndex, int category) {
            int count = 0;
            for (int i = 0; i < objects.Count; i++) {
                if (flattenedIndex == i)
                    return count;
                if (objects[i].category == category)
                    count ++;
            }
            throw new ArgumentOutOfRangeException("Flattened index is out of bounds.");
        }

        void SendEvent(AttachObjectListEventType eventType, AttachObject obj = null, int oldIndexInCategory = -1) {
            if (OnChanged == null)
                return;
            AttachObjectListEventInfo eventInfo = new AttachObjectListEventInfo(eventType, this, obj, 
                    oldIndexInCategory);
            OnChanged.Invoke(eventInfo);
        }

        public void Insert(int indexInCategory, AttachObject obj) {
            int c = 0;
            int flattened = -1;
            for (int i = 0; i < objects.Count; i++) {
                if (objects[i].category == obj.category) {
                    if (c == indexInCategory) {
                        flattened = i;
                        break;
                    }
                    c++;
                }
            }
            if (flattened == -1) {
                if (indexInCategory != c)
                    throw new ArgumentOutOfRangeException();
                flattened = objects.Count;
            }
            objects.Insert(flattened, obj);
            SendEvent(AttachObjectListEventType.ObjectAdded, obj);
        }

        public void Insert(int category, int indexInCategory, GameObject seedObject, string keyPattern = "", 
                ParamList defaultParams = null) {
            Insert(indexInCategory, new AttachObject(seedObject, category, keyPattern, defaultParams));
        }

        public void Add(AttachObject obj) {
            objects.Add(obj);
            SendEvent(AttachObjectListEventType.ObjectAdded, obj);
        }

        public void Add(GameObject seedObject, int category = 0, string keyPattern = "", 
                ParamList defaultParams = null) {
            Add(new AttachObject(seedObject, category, keyPattern, defaultParams));
        }

        public void Remove(AttachObject obj) {
            int oldIndexInCategory = GetIndexInCategory(obj);
            SendEvent(AttachObjectListEventType.ObjectWillBeRemoved, obj);
            objects.Remove(obj);
            SendEvent(AttachObjectListEventType.ObjectRemoved, obj, oldIndexInCategory: oldIndexInCategory);
        }

        public void Remove(int category, int index) {
            Remove(Get(category, index));
        }

        // called by the editor after the reorder has already happened (via ReorderableList)
        public void HandleFlattenedReorder(int oldFlattenedIndex, int newFlattenedIndex) {
            AttachObject obj = objects[newFlattenedIndex];
            int newIndexInCategory = GetIndexInCategory(obj);
            // move the object back where it was
            objects.RemoveAt(newFlattenedIndex);
            objects.Insert(oldFlattenedIndex, obj);
            if (GetIndexInCategory(obj) == newIndexInCategory) {
                objects.RemoveAt(oldFlattenedIndex);
                objects.Insert(newFlattenedIndex, obj);
                return;
            }
            ReorderInCategory(obj, newIndexInCategory);
        }

        public void ReorderInCategory(AttachObject obj, int newIndexInCategory) {
            if (newIndexInCategory == GetIndexInCategory(obj))
                return;
            Remove(obj);
            Insert(newIndexInCategory, obj);
        }

        public void Replace(AttachObject existingObj, AttachObject newObj) {
            int flattened = GetFlattenedIndex(existingObj);
            Remove(existingObj);
            objects.Insert(flattened, newObj);
            SendEvent(AttachObjectListEventType.ObjectAdded, newObj);
        }

        public void Set(AttachObject obj, int indexInCategory = 0) {
            int count = Count(obj.category);
            if (indexInCategory > count)
                throw new ArgumentOutOfRangeException();
            if (indexInCategory == count) {
                Add(obj);
                return;
            }
            AttachObject existingObj = Get(obj.category, indexInCategory);
            Remove(existingObj);
            Insert(indexInCategory, obj);
        }

	}

}