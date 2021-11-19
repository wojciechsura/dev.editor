using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooksoft.VisualStateManager.Conditions
{
    public class SwitchCondition<T>
            where T : struct
    {
        private T current;
        private readonly Dictionary<T, SimpleCondition> conditions;

        public SwitchCondition()
        {
            conditions = new Dictionary<T, SimpleCondition>();
            current = default;
        }

        public SwitchCondition(params T[] newValues)
        {
            conditions = new Dictionary<T, SimpleCondition>();
            current = default;

            foreach (var value in newValues)
                Add(value);
        }

        public void Add(T value)
        {
            if (conditions.ContainsKey(value))
                throw new ArgumentException("value");

            var condition = new SimpleCondition(current.Equals(value));
            conditions.Add(value, condition);

            if (conditions.Count == 1)
            {
                current = value;
                condition.Value = true;
            }
        }

        public IEnumerable<T> Values
        {
            get
            {
                return conditions.Keys
                    .ToList();
            }
        }

        public T Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;

                foreach (var pair in conditions)
                {
                    pair.Value.Value = pair.Key.Equals(value);
                }
            }
        }

        public BaseCondition this[T value]
        {
            get
            {
                if (conditions.ContainsKey(value))
                    return conditions[value];
                else
                    return null;
            }
        }
    }
}
