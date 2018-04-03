namespace SubC.Attachments {

    [System.Serializable]
    public class ParamProviderDefinition {
        public int id;
        public string label;

        public ParamProviderDefinition(int id, string label) {
            this.id = id;
            this.label = label;
        }
    }

}