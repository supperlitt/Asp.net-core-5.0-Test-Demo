using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenIddict.Core;
using OpenIddict.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTest5.Model;

namespace WebCoreTest5.Handler
{

    public class PasswordGrantTypeEventHandler : BaseOpenIdGrantHandler<OpenIddictServerEvents.HandleTokenRequestContext>
    {
        private readonly UserManager<MyUser> _userManager;

        public PasswordGrantTypeEventHandler(
            OpenIddictApplicationManager<TestOpenIdClient> applicationManager,
            OpenIddictAuthorizationManager<TestOpenIdAuthorization> authorizationManager,
            SignInManager<MyUser> signInManager,
            UserManager<MyUser> userManager,
            IOptions<IdentityOptions> identityOptions) : base(applicationManager,
            authorizationManager, signInManager, identityOptions)
        {
            _userManager = userManager;
        }

        public static OpenIddictServerHandlerDescriptor Descriptor { get; } =
            OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleTokenRequestContext>()
                        .UseScopedHandler<PasswordGrantTypeEventHandler>()
                        .Build();

        public override async ValueTask HandleAsync(
            OpenIddictServerEvents.HandleTokenRequestContext notification)
        {
            var request = notification.Request;
            //if (!request.IsPasswordGrantType())
            //{
            //    return;
            //}

            //var httpContext = notification.Transaction.GetHttpRequest().HttpContext;
            //if (user == null || await _u2FService.HasDevices(user.Id) ||
            //    !(await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true))
            //        .Succeeded)
            //{
            //    notification.Reject(
            //        error: OpenIddictConstants.Errors.InvalidGrant,
            //        description: "The specified credentials are invalid.");
            //    return;
            //}

            var user = await _userManager.FindByNameAsync(request.Username);
            notification.Principal = await CreateClaimsPrincipalAsync(request, user);
            notification.HandleRequest();
        }
    }
}
