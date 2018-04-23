using System;
using Chronos.Core.Coinbase.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Coinbase
{
    public class CoinbaseAccount : AggregateBase
    {
        private string _email;
        
        public CoinbaseAccount() {}

        public CoinbaseAccount(Guid accountId, string email)
        {
            When(new CoinbaseAccountCreated(accountId,email));
        }

        public void PurchaseCoin(Guid purchaseId,string coin, double quantity, double costPerUnit, double fee)
        {
            When(new CoinPurchased(Id,purchaseId,coin, quantity, costPerUnit, fee));
        }

        public void When(CoinbaseAccountCreated e)
        {
            Id = e.AccountId;
            _email = e.Email;
            base.When(e);    
        }

        public void When(CoinPurchased e)
        {
            base.When(e);
        }
    }
}