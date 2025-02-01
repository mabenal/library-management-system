using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using lms.Abstractions.CustomActionFilters;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace lms.Tests.AbstractionsTests.CustomActionFilters
{
    public class ValidateModelTests
    {
        [Fact]
        public void OnActionExecuting_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Error", "Test error");

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor(),
                modelState
            );

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );

            var filter = new ValidateModel();

            // Act
            filter.OnActionExecuting(context);

            // Assert
            Assert.IsNotType<BadRequestObjectResult>(context.Result);
        }

        [Fact]
        public void OnActionExecuting_ShouldNotReturnBadRequest_WhenModelStateIsValid()
        {
            // Arrange
            var modelState = new ModelStateDictionary();

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor(),
                modelState
            );

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );

            var filter = new ValidateModel();

            // Act
            filter.OnActionExecuting(context);

            // Assert
            Assert.NotNull(context.Result);
        }
    }
}