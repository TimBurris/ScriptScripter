using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ScriptScripter.Processor.Services.Contracts;

namespace ScriptScripter.DesktopApp.ViewModels.Tests
{
    [TestClass()]
    public class DatabaseConnectionViewModelTests : VMTestBase
    {
        public Mock<NinjaMvvm.Wpf.Abstractions.INavigator> _mockNavigator = new Mock<NinjaMvvm.Wpf.Abstractions.INavigator>();
        public Mock<IScriptingService> _mockScriptingService = new Mock<IScriptingService>();
        public DatabaseConnectionViewModel CreateViewModel()
        {

            var vm = new DatabaseConnectionViewModel(MockNavigator.Object, RepoMocks.MockConfigurationRepo.Object, ServiceMocks.MockScriptingService.Object, new DatabaseConnectionControlViewModel(_mockNavigator.Object, _mockScriptingService.Object, logger: null), logger: null);
            return vm;
        }

        [TestMethod]
        public void ConnectAsync_should_not_save_or_close_when_test_fails()
        {
            //arrange
            var vm = CreateViewModel();

            ServiceMocks.MockScriptingService.Setup(m => m.TestServerConnectionAsync(It.IsAny<Processor.Data.Models.ServerConnectionParameters>()))
                .Returns(Task.FromResult(new Processor.Dto.ActionResult() { WasSuccessful = false, Message = "all bus' up" }));

            MockNavigator.Setup(m => m.ShowDialog<MessageBoxViewModel>(It.IsAny<Action<MessageBoxViewModel>>()))
                .Callback<Action<MessageBoxViewModel>>(a =>
               {
                   var messageVM = new MessageBoxViewModel(navigator: null, logger: null);
                   a(messageVM);

                   messageVM.Message.Should().Be("all bus' up");
               })
               .Returns(new MessageBoxViewModel(navigator: null, logger: null));
            //act
            vm.ConnectAsync().Wait();

            //assert
            VerifyAllMocks();
        }

        [TestMethod]
        public void ConnectAsync_should_save_and_close_when_test_passes()
        {
            //arranget
            var vm = CreateViewModel();

            ServiceMocks.MockScriptingService.Setup(m => m.TestServerConnectionAsync(It.IsAny<Processor.Data.Models.ServerConnectionParameters>()))
                .Returns(Task.FromResult(new Processor.Dto.ActionResult() { WasSuccessful = true }));

            //should fire set
            RepoMocks.MockConfigurationRepo.Setup(m => m.SetServerConnectionParameters(It.IsAny<Processor.Data.Models.ServerConnectionParameters>()));
            //should close dialog
            MockNavigator.Setup(m => m.CloseDialog(vm));

            //act
            vm.ConnectAsync().Wait();

            //assert
            VerifyAllMocks();
        }
    }
}