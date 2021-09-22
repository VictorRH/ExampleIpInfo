using IpInfo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;
using static ExampleIpInfo.Dto.DtoIPInfo;

namespace ExampleIpInfo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IpInfoController : ControllerBase
    {
        [HttpGet("nugget")]
        public async Task<ActionResult<IPInfoRoot>> GetInfo()
        {
            var client = new HttpClient();
            var api = new IpInfoApi("TUTOKEN", client);
            var response = await api.GetCurrentInformationAsync();
            //GetCurrentInformationAsync trae toda la informacion, ip, ciudad, pais, etc..
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
            //para obtener la ip, en localhost, trae la ip ::1,
            ////en un servidor cambiaria a la ip de tu proveedor de internet
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var client = new RestClient($"https://ipinfo.io/{remoteIpAddress}?token=TUTOKEN")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {

                var content = response.Content;
                var datas = JsonConvert.DeserializeObject<IPInfoRoot>(content);

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
            var response = await httpclient.GetAsync($"https://ipinfo.io/{remoteIpAddress}?token=TUTOKEN");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync() ;
                var datas = JsonConvert.DeserializeObject<IPInfoRoot>(content);
                return new IPInfoRoot
                {
                    City = datas.City,
                    Country = datas.Country,
                    Ip = datas.Ip
                };
            }

            return BadRequest();

        }
    }
}
