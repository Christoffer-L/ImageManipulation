using ImageManipulationApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace ImageManipulationTests
{
    public class AppControllerTest
    {
        private readonly HomeController _Controller;
        
        public AppControllerTest()
            => _Controller = new HomeController(It.IsAny<ILogger<HomeController>>());

        [Fact]
        public void IndexReturnsValidView()
        {
            // Assemble

            // Act
            var viewResult = _Controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(viewResult);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public void GetImageReturnsValidView()
        {
            // Assemble

            // Act
            var viewResult = _Controller.GetImage() as ViewResult;

            // Assert
            Assert.NotNull(viewResult);
            Assert.Equal("GetImage", viewResult.ViewName);
        }
    }
}
