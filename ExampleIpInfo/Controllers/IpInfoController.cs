using IpInfo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using static ExampleIpInfo.Dto.DtoIPInfo;

namespace ExampleIpInfo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IpInfoController : ControllerBase
    {
        private readonly string? token = "YOURTOKEN";
        [HttpGet("nugget")]
        public async Task<ActionResult<IPInfoRoot>> GetInfo()
        {
            var client = new HttpClient();
            var api = new IpInfoApi(token!, client);
            var response = await api.GetCurrentInformationAsync();
            //GetCurrentInformationAsync get data information, ip, city, country, etc..
            return new IPInfoRoot
            {
                City = response.City,
                Country = response.Country,
                Ip = response.Ip
            };
        }
        [HttpGet("restsharp")]
        public async Task<ActionResult<IPInfoRoot>> GetInfoRestsharp()
        {
            //get ip, in localhost, your ip is ::1,
            ////in the server your up change
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var client = new RestClient($"https://ipinfo.io/{remoteIpAddress}?token={token!}");
            var request = new RestRequest("", Method.Get);
            var response = await client.ExecuteGetAsync(request);
            if (response.IsSuccessful)
            {
                var content = response.Content;
                var datas = JsonConvert.DeserializeObject<IPInfoRoot>(content!);
                return new IPInfoRoot
                {
                    City = datas?.City,
                    Ip = datas?.Ip,
                    Country = datas?.Country
                };
            }
            return BadRequest();
        }
        [HttpGet("httpclient")]
        public async Task<ActionResult<IPInfoRoot>> GetInfoHttpclient()
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://ipinfo.io/{remoteIpAddress}?token={token!}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var datas = JsonConvert.DeserializeObject<IPInfoRoot>(content);
                return new IPInfoRoot
                {
                    City = datas?.City,
                    Country = datas?.Country,
                    Ip = datas?.Ip
                };
            }
            return BadRequest();
        }
    }
}
