using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageManipulationApi.Database;
using ImageManipulationApi.Entities;
using ImageManipulationApi.Service;
using ImageManipulationApi.Repository;
using Microsoft.AspNetCore.SignalR;
using ImageManipulationApi.Hubs;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using ImageManipulationApi.Entities.HelperClasses;
using System.Text.Json;

namespace ImageManipulationApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ImageManipulationController : ControllerBase
    {
        private readonly IPostalService _Service;
        private readonly IImageManipulationRepository _Repo;
        private readonly IHubContext<ImageManipulationHub> _HubContext;

        public ImageManipulationController(IImageManipulationRepository repo, IPostalService service, IHubContext<ImageManipulationHub> hubContext)
        {
            _Repo = repo;
            _Service = service;
            _HubContext = hubContext;
        }

        [HttpGet]
        [Route("get-post-area-info")]
        public async Task<string> GetPostalCodeAreaNameAsync(string postalCode, string country)
        {
            await _HubContext.Clients.All.SendAsync("ReceivedNotification", $"Ser etter poststed for {postalCode}");
            
            BringPostResponse response = await _Service.GetPostalArea(postalCode, country);

            await _HubContext.Clients.All.SendAsync("ReceivedNotification", response.valid ? $"Poststed heter {response.result}!" : $"{response.result}");

            return JsonSerializer.Serialize(response);
        }

        [HttpGet]
        [Route("get-encrypted-image")]
        public async Task<string> GetUserManipulatedImage(string guid)
        {
            await _HubContext.Clients.All.SendAsync("ReceivedNotification", $"Ser etter bilde med GUID: {guid}");

            UserManipulatedImage image = await _Repo.GetAsync(guid);
            
            if(image != null)
            {
                await _HubContext.Clients.All.SendAsync("ReceivedNotification", $"Fant bilde med GUID: {guid}");
                return Convert.ToBase64String(image.EncryptedImage);
            }
            else
            {
                await _HubContext.Clients.All.SendAsync("ReceivedNotification", $"Fant ikke bildet med GUID: {guid}");
                return null;
            }
        }

        [HttpPost]
        [Route("post/manipulate-image")]
        public async Task<FileContentResult> PostManipulateImage([FromForm] UserImageInfo userImageInfo)
        {
            await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Bilde er mottatt");
            await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Starter manipulering av bilde");

            if (userImageInfo.UserFile == null || userImageInfo.UserFile.Length == 0)
                return null;
            
            FileContentResult imageResult = null;
            const int width = 1200;
            const int height = 800;

            using (var memoryStream = new MemoryStream())
            {
                await userImageInfo.UserFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                
                using (var image = Image.FromStream(memoryStream))
                {
                    // Notify user
                    await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Resizer bildet");
                  
                    using (var graphics = Graphics.FromImage(image))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.DrawImage(image, width, height);
                        
                        // Skriv på tekst på bildet
                        using Font arialFont = new Font("Arial", 5);
                        
                        graphics.DrawString(userImageInfo.PostalCodeAreaName, arialFont, Brushes.Red, new PointF(10, image.Height - 15));

                        // Notify user
                        await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Skriver på post steds navn");
                       
                        memoryStream.Position = 0;

                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        imageResult = File(memoryStream.GetBuffer(), "image/png");
                        imageResult.FileDownloadName = "manipulatedImage";
                        imageResult.LastModified = DateTime.Now;
                    }
                    // Notify user
                    await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Sender bildet tilbake");
                    return File(imageResult.FileContents, imageResult.ContentType);
                }
            }
        }

        [HttpPost]
        [Route("post/store-encrypted-image")]
        public async Task<ActionResult<string>> PostUserEncryptedImage([FromBody]string imgData)
        {
            UserManipulatedImage imageFile = new UserManipulatedImage();

            await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Kryptert bilde er mottatt");
            await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Oppretter en tilfeldig GUID");

            var bytes = Convert.FromBase64String(imgData);
            imageFile.Id = Guid.NewGuid().ToString();
            imageFile.EncryptedImage = bytes;

            if(await _Repo.AddAsync(imageFile))
            {
                await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Bildet ble lagret i databasen");
                return Ok(imageFile.Id);
            }
            else
            {
                await _HubContext.Clients.All.SendAsync("ReceivedNotification", "Bildet ble IKKE lagret");
                return BadRequest("Bilde ble ikke lagret");
            }
        }
    }
}
