using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VNPTPM.Model.Commons;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Ad;
using VNPTPM.Web.Api.Home;

namespace VNPTPM.Web.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (string.IsNullOrEmpty(publicClientId))
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //local use below else comment
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var lang = context.Request.Headers["Accept-Language"];
            if (string.IsNullOrEmpty(lang))
            {
                lang = "vi-VN";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            UserInformationVM userInformation = null;
            Task task = Task.Factory.StartNew<UserInformationVM>(() =>
            {
                return (new UserController().Login(context.UserName, context.Password));
            })
            .ContinueWith((result) =>
            {
                userInformation = result.Result;
            });

            await task;

            if (!string.IsNullOrEmpty(userInformation.Msg))
            {
                context.SetError("invalid_grant", VNPTResources.Instance.Get(userInformation.Msg));
                return;
            }
            
            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim("sub", context.UserName));
            oAuthIdentity.AddClaim(new Claim("role", "user"));
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

            ClaimsIdentity cookiesIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
            cookiesIdentity.AddClaim(new Claim("sub", context.UserName));
            cookiesIdentity.AddClaim(new Claim("role", "user"));
            cookiesIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

            AuthenticationProperties properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                {"UserName", userInformation.UserName },
                {"FullName", userInformation.FullName },
                {"RoleID", userInformation.RoleID },
                {"RoleName", userInformation.RoleName }
            });

            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}