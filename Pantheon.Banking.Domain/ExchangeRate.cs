using System;
using System.Collections.Generic;

namespace Pantheon.Banking.Domain
{
    public class ExchangeRate
    {
        public string Base { get; set; }
        public Dictionary<string, double?> Rates { get; set; }
        public DateTime? Date { get; set; }
    }
}
