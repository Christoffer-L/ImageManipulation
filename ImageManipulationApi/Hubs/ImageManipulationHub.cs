using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageManipulationApi.Entities;
using ImageManipulationApi.Repository;
using Microsoft.AspNetCore.SignalR;

namespace ImageManipulationApi.Hubs
{
    public class ImageManipulationHub : Hub
    {
        public ImageManipulationHub() {}

        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceivedNotification", message);
        }
    }
}
