using BlazorApp.Helpers;
using BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorApp.Services
{
    public interface IAuthenticationService
    {
        User User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public User User { get; private set; }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        ) {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<User>(StaticValues.USERLS);
        }

        public async Task Login(string username, string password)
        {
            User = await _httpService.Post<User>("/login", new { username, password });
            User.AuthData = $"{username}:{password}".EncodeBase64();
            await _localStorageService.SetItem(StaticValues.USERLS, User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem(StaticValues.BEARER);
            await _localStorageService.RemoveItem(StaticValues.USERLS);
            _navigationManager.NavigateTo("login");
        }
    }
}