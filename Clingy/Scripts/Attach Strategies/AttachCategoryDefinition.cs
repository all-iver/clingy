namespace SubC.Attachments {

    public class AttachCategoryDefinition {
        public string label, objectLabel;
        public int min, max;
        public bool hasMax { get { return max >= 0; } }

        public AttachCategoryDefinition(string label, string objectLabel, int min = 0, int max = -1) {
            this.label = label;
            this.objectLabel = objectLabel;
            this.min = min;
            this.max = max;
        }

        public string GetLabelForObject(int index) {
            if (max == 1)
                return objectLabel;
            return objectLabel + " #" + index;
        }
    }

}