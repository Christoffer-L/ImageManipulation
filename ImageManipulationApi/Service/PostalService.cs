using ImageManipulationApi.Entities.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageManipulationApi.Service
{
    public interface IPostalService
    {
        public Task<BringPostResponse> GetPostalArea(string postalCode, string countryName); 
    }

    public class PostalService : IPostalService
    {
        private readonly HttpClient _Client;

        public PostalService(HttpClient client)
        {
            _Client = client;
        }

        public async Task<BringPostResponse> GetPostalArea(string postalCode, string countryName)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _Client.BaseAddress.ToString() + $"?pnr={postalCode}&country={countryName}");

            using HttpResponseMessage response = await _Client.SendAsync(request);
            try
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Response from bring API gave status code: {response.StatusCode}");

                return JsonSerializer.Deserialize<BringPostResponse>(await response.Content.ReadAsStringAsync());
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
