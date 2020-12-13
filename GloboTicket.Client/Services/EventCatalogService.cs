using GloboTicket.Web.Extensions;
using GloboTicket.Web.Models.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace GloboTicket.Web.Services
{
    public class EventCatalogService : IEventCatalogService
    {
        private readonly HttpClient client;
        private string _accessToken;

        public EventCatalogService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<Event>> GetAll()
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync("/api/events");
            return await response.ReadContentAs<List<Event>>();
        }

        public async Task<IEnumerable<Event>> GetByCategoryId(Guid categoryid)
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync($"/api/events/?categoryId={categoryid}");
            return await response.ReadContentAs<List<Event>>();
        }

        public async Task<Event> GetEvent(Guid id)
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync($"/api/events/{id}");
            return await response.ReadContentAs<Event>();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync("/api/categories");
            return await response.ReadContentAs<List<Category>>();
        }


        private async Task<string> GetToken()
        {
            if (!string.IsNullOrWhiteSpace(_accessToken))
                return _accessToken;

            var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5010");

            if (discoveryDocumentResponse.IsError)
                throw new Exception(discoveryDocumentResponse.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocumentResponse.TokenEndpoint,
                ClientId = "globoticket",
                ClientSecret = "ce766e16-df99-411d-8d31-0f5bbc6b8eba",
                Scope = "eventcatalog.read"
            });

            _accessToken = tokenResponse.AccessToken;

            return _accessToken;
        }

    }
}
