using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration
{
    public class ConfigurationModel : ConfigRoot
    {
        internal const string NAME = "DevEditor";

        private readonly EditorData editorConfiguration;
        private readonly BehaviorData behaviorConfiguration;
        private readonly InternalData internalConfiguration;

        public ConfigurationModel() : base(NAME)
        {
            editorConfiguration = new EditorData(this);
            behaviorConfiguration = new BehaviorData(this);
            internalConfiguration = new InternalData(this);
        }

        public EditorData Editor => editorConfiguration;
        public BehaviorData Behavior => behaviorConfiguration;
        public InternalData Internal => internalConfiguration;
    }
}
