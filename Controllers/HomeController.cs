using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace webApidemo4.Controllers
{
    public class HomeController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            try
            {
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://mtaDemo.vault.azure.net/secrets/mySecret")
                    .ConfigureAwait(false);

                ViewBag.Secret = $"Secret: {secret.Value } ";
                ViewBag.Error = $"You are in!!!";
                ViewBag.Principal = azureServiceTokenProvider.PrincipalUsed != null ? $"Principal Used: {azureServiceTokenProvider.PrincipalUsed}" : string.Empty;
                String auth = azureServiceTokenProvider.PrincipalUsed.IsAuthenticated.ToString();
                string tenant = azureServiceTokenProvider.PrincipalUsed.UserPrincipalName;



            }
            catch (Exception exp)
            {
                // ViewBag.Error = $"Something went wrong: {exp.Message}";
                ViewBag.Error = $"you ar not authorized to this site!";
            }



            return View();
        }

       // public ActionResult Index()
       // {
       //     ViewBag.Title = "Home Page";

        //    return View();
       // }
    }
}
