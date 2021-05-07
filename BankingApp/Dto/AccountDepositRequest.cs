namespace Pantheon.Banking.Web.UI.Dto
{
    public class AccountTransactionRequest
    {
        public string Currency { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public double Amount { get; set; }
    }
}