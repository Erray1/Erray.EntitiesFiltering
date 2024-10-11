using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erray.EntitiesFiltering
{
    public class ParsingFiltersFailedException : Exception
    {
        public ParsingFiltersFailedException(string propertyName, string error) : base($"Error parsing filter query at {propertyName} with error: {error}")
        {
            
        }

        public ParsingFiltersFailedException(string error) : base(error) { }
        public ParsingFiltersFailedException(Exception innerException) : base("Error parsing filter query", innerException)
        {
            
        }
    }
}
