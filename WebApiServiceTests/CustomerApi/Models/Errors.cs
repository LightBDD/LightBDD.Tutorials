using System.Collections.Generic;

namespace CustomerApi.Models
{
    public class Errors
    {
        public Errors(params Error[] items)
        {
            Items = items;
        }

        public IReadOnlyList<Error> Items { get; }
    }
}