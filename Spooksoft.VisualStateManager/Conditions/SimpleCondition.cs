using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooksoft.VisualStateManager.Conditions
{
    public class SimpleCondition : BaseCondition
    {
        // Private fields ----------------------------------------------------

        private bool value;

        // Private methods ---------------------------------------------------

        private void SetValue(bool value)
        {
            if (value == this.value)
                return;

            this.value = value;
            OnValueChanged(this.value);
        }

        // Public methods ----------------------------------------------------

        public SimpleCondition(bool newValue = false)
        {
            value = newValue;
        }

        public override bool GetValue()
        {
            return value;
        }

        // Public properties -------------------------------------------------

        public bool Value
        {
            get
            {
                return value;
            }
            set
            {
                SetValue(value);
            }
        }
    }
}
