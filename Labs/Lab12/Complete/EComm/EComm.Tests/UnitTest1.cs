using EComm.API.Controllers;
using EComm.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace EComm.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TwoPlusTwo()
        {
            Assert.Equal(4, (2 + 2));
        }

        [Fact]
        public void ProductDetails()
        {
            // Arrange
            var repository = new StubRepository();
            var pc = new ProductController(repository);

            // Act
            var result = pc.GetProduct(1).Result;

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result);
            var r = result as OkObjectResult;
            Assert.IsAssignableFrom<Product>(r!.Value); 
            var model = r!.Value as Product;
            Assert.Equal("Bread", model!.ProductName);
        }
    }
}