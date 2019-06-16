using System.Collections.Generic;
using System.Linq;
using Thaliak.Network.Analyzer;

namespace Thaliak.Network.Filter
{
    public class Filters<T>
    {
        public FilterOperator FilterOperator { get; set; }
        public IList<PropertyFilter<T>> PropertyFilters { get; set; }

        public Filters(FilterOperator filterFilterOperator)
        {
            this.FilterOperator = filterFilterOperator;
            this.PropertyFilters = new List<PropertyFilter<T>>();
        }

        public bool IsMatch(T obj)
        {
            return this.FilterOperator == FilterOperator.AND
                ? this.PropertyFilters.All(x => x.IsMatch(obj))
                : this.PropertyFilters.Any(x => x.IsMatch(obj));
        }

        public List<MessageAttribute> WhichMatch(T obj)
        {
            return this.PropertyFilters.Where(x => x.IsMatch(obj)).Select(x => x.Label).ToList();
        }
    }
}