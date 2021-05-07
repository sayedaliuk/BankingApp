using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pantheon.Banking.Web.UI.Dto
{
    public class BankAccountDetailResponse
    {
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public double Balance { get; set; }
    }
}
