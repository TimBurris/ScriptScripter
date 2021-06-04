using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace ScriptScripter.DesktopAppTests
{
    public class ServiceMockery
    {
        public Mock<Processor.Services.Contracts.IScriptingService> MockScriptingService { get; set; }
        public Mock<Processor.Services.Contracts.IEventNotificationService> MockEventNotification { get; set; }

        public void SetupMocks(MockBehavior mockBehavior)
        {
            MockScriptingService = new Mock<Processor.Services.Contracts.IScriptingService>(mockBehavior);
            MockEventNotification = new Mock<Processor.Services.Contracts.IEventNotificationService>(mockBehavior);

            //****** DON'T FORGET TO ADD TO VERIFY ALL!!!! *******
        }

        public void VerifyAllMocks()
        {
            //Repos
            MockScriptingService.VerifyAll();
            MockEventNotification.VerifyAll();
        }

    }
}
