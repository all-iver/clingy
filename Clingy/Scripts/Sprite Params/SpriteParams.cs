namespace SubC.Attachments {

    using UnityEngine;
    using System.Collections.Generic;

	[CreateAssetMenu(fileName = "New Sprite Params", menuName = "Clingy/Sprite Params")]
    public class SpriteParams : ScriptableObject {

        [System.Serializable]
        public class SpriteFrame {
            public Sprite sprite;
            public ParamList paramList;
        }

        [SerializeField]
        List<SpriteFrame> frames = new List<SpriteFrame>();

        [SerializeField]
        protected int frameToEdit = 0; // for Unity editor purposes
        [SerializeField]
        Sprite previewSprite; // for Unity editor purposes

        public bool HasParam(SpriteRenderer spriteRenderer, ParamType type, string name) {
            foreach (SpriteFrame frame in frames) {
                if (frame.sprite == spriteRenderer.sprite)
                    return frame.paramList.HasParam(type, name);
            }
            throw new System.InvalidOperationException("Sprite frame not found");
        }

        public Param GetParam(SpriteRenderer spriteRenderer, ParamType type, string name) {
            foreach (SpriteFrame frame in frames) {
                if (frame.sprite == spriteRenderer.sprite)
                    return frame.paramList.GetParam(type, name);
            }
            throw new System.InvalidOperationException("Sprite frame not found");
        }

        public Param GetParam(SpriteRenderer spriteRenderer, Param defaultParam) {
            foreach (SpriteFrame frame in frames) {
                if (frame.sprite == spriteRenderer.sprite)
                    return frame.paramList.GetParam(defaultParam);
            }
            throw new System.InvalidOperationException("Sprite frame not found");
        }

        public void MergeParamsForCurrentSprite(ParamList intoParams, SpriteRenderer spriteRenderer) {
            foreach (SpriteFrame frame in frames) {
                if (frame.sprite == spriteRenderer.sprite) {
                    intoParams.MergeFrom(frame.paramList);
                    return;
                }
            }
            throw new System.InvalidOperationException("Sprite frame not found");
        }

        public bool HasSprite(Sprite sprite) {
            foreach (SpriteFrame frame in frames)
                if (frame.sprite == sprite)
                    return true;
            return false;
        }

        public void AddFrame(Sprite sprite) {
            if (HasSprite(sprite))
                return;
            SpriteFrame frame = new SpriteFrame();
            frame.sprite = sprite;
            frame.paramList = new ParamList();
            frames.Add(frame);
        }

        public void RemoveSprite(Sprite sprite) {
            for (int i = frames.Count - 1; i >= 0; i--)
                if (frames[i].sprite == sprite)
                    frames.RemoveAt(i);
        }

        public SpriteFrame GetFrame(int index) {
            return frames[index];
        }

    }

}