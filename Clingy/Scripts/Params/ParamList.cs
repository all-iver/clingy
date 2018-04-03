namespace SubC.Attachments {

    using UnityEngine;
    using System.Collections.Generic;

	[System.Serializable]
    public class ParamList {

        [SerializeField]
        // called _params because params is a reserved word
        protected List<Param> _params = new List<Param>();

        public void Clear() {
            _params.Clear();
        }

        public bool Equals(ParamList other) {
            if (Count != other.Count)
                return false;
            for (int i = 0; i < Count; i++)
                if (!_params[i].Equals(other._params[i]))
                    return false;
            return true;
        }

        public int Count { get { return _params.Count; } }

        public bool HasParam(ParamType type, string name) {
            foreach (Param p in _params)
                if (p.type == type && p.name == name)
                    return true;
            return false;
        }

        public Param GetParam(ParamType type, string name) {
            foreach (Param p in _params) {
                if (p.type == type && p.name == name)
                    return p;
            }
            throw new System.InvalidOperationException("Param not found");
        }

        public Param GetParam(Param defaultParam) {
            foreach (Param p in _params) {
                if (p.type == defaultParam.type && p.name == defaultParam.name)
                    return p;
            }
            return defaultParam;
        }

        public int GetIndexOfParam(ParamType type, string name) {
            for (int i = 0; i < _params.Count; i++)
                if (_params[i].type == type && _params[i].name == name)
                    return i;
            return -1;
        }

        public bool SetParam(Param param, bool replaceExisting = true) {
            int i = GetIndexOfParam(param.type, param.name);
            if (i == -1) {
                if (!replaceExisting)
                    return false;
                _params.Add(param);
            } else {
                _params[i] = param;
            }
            return true;
        }

        public void MergeFrom(ParamList other, bool replaceExisting = true) {
            foreach (Param p in other._params)
                SetParam(p, replaceExisting);
        }

        public ParamList Clone(bool cloneParams = false) {
            ParamList paramList = new ParamList();
            foreach (Param p in _params) {
                if (cloneParams)
                    paramList.SetParam(p.Clone());
                else
                    paramList.SetParam(p);
            }
            return paramList;
        }

   }

}