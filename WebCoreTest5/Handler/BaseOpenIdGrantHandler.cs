using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebCoreTest5.Model;

namespace WebCoreTest5.Handler
{
    public abstract class BaseOpenIdGrantHandler<T> :
        IOpenIddictServerHandler<T>
        where T : OpenIddictServerEvents.BaseContext
    {
        private readonly OpenIddictApplicationManager<TestOpenIdClient> _applicationManager;
        private readonly OpenIddictAuthorizationManager<TestOpenIdAuthorization> _authorizationManager;
        protected readonly SignInManager<MyUser> _signInManager;
        protected readonly IOptions<IdentityOptions> _identityOptions;

        protected BaseOpenIdGrantHandler(
            OpenIddictApplicationManager<TestOpenIdClient> applicationManager,
            OpenIddictAuthorizationManager<TestOpenIdAuthorization> authorizationManager,
            SignInManager<MyUser> signInManager,
            IOptions<IdentityOptions> identityOptions)
        {
            _applicationManager = applicationManager;
            _authorizationManager = authorizationManager;
            _signInManager = signInManager;
            _identityOptions = identityOptions;
        }


        protected Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(OpenIddictRequest request, MyUser user)
        {
            return CreateClaimsPrincipalAsync(_applicationManager, _authorizationManager,
                _identityOptions.Value, _signInManager, request, user);
        }
        public abstract ValueTask HandleAsync(T notification);

        public async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(OpenIddictApplicationManager<TestOpenIdClient> applicationManager,
            OpenIddictAuthorizationManager<TestOpenIdAuthorization> authorizationManager,
            IdentityOptions identityOptions,
            SignInManager<MyUser> signInManager,
            OpenIddictRequest request,
            MyUser user)
        {
            var principal = await signInManager.CreateUserPrincipalAsync(user);
            
            return principal;
        }
    }

    public class TestOpenIdClient : OpenIddictEntityFrameworkCoreApplication<string, TestOpenIdAuthorization, TestOpenIdToken>
    {
        public string ApplicationUserId { get; set; }
        public MyUser ApplicationUser { get; set; }
    }

    public class TestOpenIdAuthorization : OpenIddictEntityFrameworkCoreAuthorization<string, TestOpenIdClient, TestOpenIdToken> { }

    public class TestOpenIdToken : OpenIddictEntityFrameworkCoreToken<string, TestOpenIdClient, TestOpenIdAuthorization> { }
}
