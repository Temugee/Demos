using System;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PhoneShopClient.Services;

namespace PhoneShopClient.Authentication
{
    public class CustomAuthenticationstateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationService authenticationService;
        public CustomAuthenticationstateProvider(AuthenticationService authenticationService, HttpClient httpClient)
        {
            this.authenticationService = authenticationService;
        }
        private ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var getUserSession = await authenticationService.GetUserDetails();
                if (getUserSession is null || string.IsNullOrEmpty(getUserSession.Email))
                    return await Task.FromResult(new AuthenticationState(anonymous));
                var claimsPrincipal = authenticationService.SetClaimPrincipal(getUserSession);
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(anonymous));
            }
        }
        public async Task UpdateAuthenticationState(TokenProp tokenProp)
        {
            ClaimsPrincipal claimsPrincipal = new();
            if(tokenProp is not null || !string.IsNullOrEmpty(tokenProp!.Token))
            {
                await authenticationService.SetTokenToLocalStorage(General.SerializeObj(tokenProp));
                var getUserSesion = await authenticationService.GetUserDetails();
                if (getUserSesion is not null || !string.IsNullOrEmpty(getUserSesion!.Email))
                    claimsPrincipal = authenticationService.SetClaimPrincipal(getUserSesion);
            }
            else
            {
                claimsPrincipal = anonymous;
                await authenticationService.RemoveTokenFromLocalStorage();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}

