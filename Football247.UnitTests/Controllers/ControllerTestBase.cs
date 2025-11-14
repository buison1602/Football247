using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.UnitTests.Controllers
{
    public abstract class ControllerTestBase<TController>
    {
        protected readonly Mock<ILogger<TController>> Logger = new();

        protected T Inject<T>(T instance) => instance; 
    }
}
