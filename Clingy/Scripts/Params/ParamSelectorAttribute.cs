namespace SubC.Attachments {

    using UnityEngine;
    using System.Reflection;

    public class ParamSelectorAttribute : PropertyAttribute {

        public int[] providers;
        public string[] labels = new string[] { "Loading..." };
        public bool hasLabels { get; private set; }

        // providers should only be null if we're in a transitioner. in that case, we'll ask the strategy for a list of
        // providers for the strategy's selected editor category (since transitioners don't know the categories).
        public ParamSelectorAttribute(int[] providers = null) {
            this.providers = providers;
        }

        public void BuildLabels(AttachStrategy strategy) {
            if (providers.Length == 0 || strategy == null)
                return;
            labels = new string[providers.Length];
            for (int i = 0; i < providers.Length; i++)
                labels[i] = strategy.GetLabelForProvider(providers[i]);
            hasLabels = true;
        }

        public void GetProvidersFromStrategy(AttachStrategy strategy) {
            if (strategy == null)
                return;
            FieldInfo field = typeof(AttachStrategy).GetField("_selectedCategory", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
            providers = strategy.GetProvidersForTransitioner((int) field.GetValue(strategy));
            BuildLabels(strategy);
        }

        public int GetIndexOfProvider(int provider) {
            for (int i = 0; i < providers.Length; i++)
                if (providers[i] == provider)
                    return i;
            throw new System.InvalidOperationException("Provider not found");
        }

    }

}