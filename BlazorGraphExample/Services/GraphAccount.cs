using System;

namespace BlazorGraphExample.Services
{
    public class GraphAccount
    {
        public GraphAccount() { }

        public GraphAccount(
            string accountId,
            string accountName,
            DateTime expires,
            string tenantId,
            string identityProvider,
            string azureADObjectId)
        {
            AccountId = accountId;
            AccountName = accountName;
            Expires = expires;
            TenantId = tenantId;
            IdentityProvider = identityProvider;
            AzureADObjectId = azureADObjectId;
        }


        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public DateTime Expires { get; set; }
        public string TenantId { get; set; }
        public string IdentityProvider { get; set; }
        public string AzureADObjectId { get; set; }

        public void Deconstruct(
            out string accountId,
            out string accountName,
            out DateTime expires,
            out string tenantId,
            out string identityProvider,
            out string azureADObjectId)
        {
            accountId = AccountId;
            accountName = AccountName;
            expires = Expires;
            tenantId = TenantId;
            identityProvider = IdentityProvider;
            azureADObjectId = AzureADObjectId;
        }
    }
}
