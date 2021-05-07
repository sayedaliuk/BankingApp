using BankingApp;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Pantheon.Banking.Domain;
using Newtonsoft.Json;
using Pantheon.Banking.Web.UI.Dto;
using System.Text;

namespace Pantheon.Banking.IntegrationTests
{
    public class BankAccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient Client { get; }
       
        public BankAccountControllerTests(WebApplicationFactory<Startup> fixture)
        {
            Client = fixture.CreateClient();
        }

        [Theory]
        [InlineData("BankAccount?accountNumber=22451190&sortCode=231120")]        
        public async Task Get_Should_Retrieve_AccountDetails(string endpoint)
        {
            var response = await Client.GetAsync(endpoint);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var res = JsonConvert.DeserializeObject<BankAccount>(await response.Content.ReadAsStringAsync());

            res.AccountName.ShouldBe("Albert Jorge");
            res.AccountNo.ShouldBe("22451190");
            res.SortCode.ShouldBe("231120");
        }

        [Theory]
        [InlineData("BankAccount/deposit/")]
        public async Task Deposit_Should_Return_Correct_Balance(string endpoint)
        {
            var res = await Client.PostAsync(endpoint
               , new StringContent(
               JsonConvert.SerializeObject(new AccountTransactionRequest()
               {
                   AccountNumber = "22451190",
                   SortCode = "231120",
                   Amount = 200,
                   Currency = "GBP"
                   
               }),
            Encoding.UTF8,
           "application/json"));

            res.EnsureSuccessStatusCode();
            res.StatusCode.ShouldBe(HttpStatusCode.OK);

            var updatedBalanced = JsonConvert.DeserializeObject<double?>(await res.Content.ReadAsStringAsync());

            var getResult = await Client.GetAsync("BankAccount?accountNumber=22451190&sortCode=231120");
            getResult.StatusCode.ShouldBe(HttpStatusCode.OK);

            var account = JsonConvert.DeserializeObject<BankAccount>(await getResult.Content.ReadAsStringAsync());

            account.Balance.ShouldBe(updatedBalanced);
        }

        [Theory]
        [InlineData("BankAccount/withdraw/")]
        public async Task Withdrawal_Should_Return_Correct_Balance(string endpoint)
        {
            var res = await Client.PostAsync(endpoint
               , new StringContent(
               JsonConvert.SerializeObject(new AccountTransactionRequest()
               {
                   AccountNumber = "22451190",
                   SortCode = "231120",
                   Amount = 200,
                   Currency = "GBP"

               }),
            Encoding.UTF8,
           "application/json"));

            res.EnsureSuccessStatusCode();
            res.StatusCode.ShouldBe(HttpStatusCode.OK);

            var updatedBalanced = JsonConvert.DeserializeObject<double?>(await res.Content.ReadAsStringAsync());

            var getResult = await Client.GetAsync("BankAccount?accountNumber=22451190&sortCode=231120");
            getResult.StatusCode.ShouldBe(HttpStatusCode.OK);

            var account = JsonConvert.DeserializeObject<BankAccount>(await getResult.Content.ReadAsStringAsync());

            account.Balance.ShouldBe(updatedBalanced);
        }
    }
}
