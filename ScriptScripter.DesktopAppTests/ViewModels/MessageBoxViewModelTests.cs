using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using static ScriptScripter.DesktopApp.ViewModels.MessageBoxViewModel;
using Moq;

namespace ScriptScripter.DesktopApp.ViewModels.Tests
{
    [TestClass()]
    public class MessageBoxViewModelTests
    {
        [TestMethod()]
        public void InitTest()
        {
            var vm = new MessageBoxViewModel();
            vm.Init(title: "title", message: "message", buttons: MessageBoxViewModel.MessageBoxButton.YesNo);

            Assert.AreEqual(expected: "title", actual: vm.ViewTitle);
            Assert.AreEqual(expected: "message", actual: vm.Message);
            Assert.IsFalse(vm.CanOK, message: "CanOK is wrong");
            Assert.IsTrue(vm.CanYes, message: "CanYes is wrong");
            Assert.IsTrue(vm.CanNo, message: "CanNo is wrong");
            Assert.IsFalse(vm.CanCancel, message: "CanCancel is wrong");
            Assert.IsFalse(vm.CanShowMoreDetails, message: "CanShowMoreDetails is wrong");
        }

        [TestMethod()]
        public void Init1()
        {
            var vm = new MessageBoxViewModel();
            vm.Init(title: "title", message: "message", buttons: MessageBoxViewModel.MessageBoxButton.YesNo, icon: MessageBoxViewModel.MessageBoxImage.Warning);

            Assert.AreEqual(expected: "title", actual: vm.ViewTitle);
            Assert.AreEqual(expected: "message", actual: vm.Message);
            Assert.IsFalse(vm.CanOK, message: "CanOK is wrong");
            Assert.IsTrue(vm.CanYes, message: "CanYes is wrong");
            Assert.IsTrue(vm.CanNo, message: "CanNo is wrong");
            Assert.IsFalse(vm.CanCancel, message: "CanCancel is wrong");
            Assert.IsFalse(vm.CanShowMoreDetails, message: "CanShowMoreDetails is wrong");
            Assert.AreEqual(expected: MessageBoxViewModel.MessageBoxImage.Warning, actual: vm.Icon);
        }

        [TestMethod()]
        public void Init2()
        {
            var vm = new MessageBoxViewModel();
            vm.Init(title: "title", message: "message",
                buttons: MessageBoxViewModel.MessageBoxButton.YesNo,
                icon: MessageBoxViewModel.MessageBoxImage.Warning,
                defaultButton: MessageBoxViewModel.MessageBoxDefaultButton.OK);

            Assert.AreEqual(expected: "title", actual: vm.ViewTitle);
            Assert.AreEqual(expected: "message", actual: vm.Message);
            Assert.IsFalse(vm.CanOK, message: "CanOK is wrong");
            Assert.IsTrue(vm.CanYes, message: "CanYes is wrong");
            Assert.IsTrue(vm.CanNo, message: "CanNo is wrong");
            Assert.IsFalse(vm.CanCancel, message: "CanCancel is wrong");
            Assert.IsFalse(vm.CanShowMoreDetails, message: "CanShowMoreDetails is wrong");
            Assert.AreEqual(expected: MessageBoxViewModel.MessageBoxImage.Warning, actual: vm.Icon);
            Assert.AreEqual(expected: MessageBoxViewModel.MessageBoxDefaultButton.OK, actual: vm.DefaultButton);
        }

        [TestMethod()]
        public void MessageBoxViewModelDefaultsTest()
        {
            var vm = new MessageBoxViewModel();

            Assert.AreEqual(expected: null, actual: vm.ViewTitle);
            Assert.AreEqual(expected: null, actual: vm.Message);
            Assert.IsFalse(vm.CanOK, message: "CanOK is wrong");
            Assert.IsFalse(vm.CanYes, message: "CanYes is wrong");
            Assert.IsFalse(vm.CanNo, message: "CanNo is wrong");
            Assert.IsFalse(vm.CanCancel, message: "CanCancel is wrong");
            Assert.IsFalse(vm.CanShowMoreDetails, message: "CanShowMoreDetails is wrong");
            Assert.AreEqual(expected: MessageBoxViewModel.MessageBoxImage.None, actual: vm.Icon);
            Assert.AreEqual(expected: MessageBoxViewModel.MessageBoxDefaultButton.NoDefault, actual: vm.DefaultButton);
        }


        [TestMethod()]
        public void YesTest()
        {
            bool closeCalled = false;
            var mockNavigator = new Mock<NinjaMvvm.Wpf.Abstractions.INavigator>();

            var vm = new MessageBoxViewModel(mockNavigator.Object);
            mockNavigator.Setup(mck => mck.CloseDialog(vm))
                .Callback(() => closeCalled = true);

            vm.Yes();

            Assert.AreEqual(expected: MessageBoxResult.Yes, actual: vm.ViewResult);
            Assert.IsTrue(closeCalled, message: "Close event should have fired");
        }

        [TestMethod()]
        public void NoTest()
        {
            bool closeCalled = false;
            var mockNavigator = new Mock<NinjaMvvm.Wpf.Abstractions.INavigator>();

            var vm = new MessageBoxViewModel(mockNavigator.Object);
            mockNavigator.Setup(mck => mck.CloseDialog(vm))
                .Callback(() => closeCalled = true);

            vm.No();

            Assert.AreEqual(expected: MessageBoxResult.No, actual: vm.ViewResult);
            Assert.IsTrue(closeCalled, message: "Close event should have fired");
        }

        [TestMethod()]
        public void OKTest()
        {
            bool closeCalled = false;
            var mockNavigator = new Mock<NinjaMvvm.Wpf.Abstractions.INavigator>();

            var vm = new MessageBoxViewModel(mockNavigator.Object);
            mockNavigator.Setup(mck => mck.CloseDialog(vm))
                .Callback(() => closeCalled = true);

            vm.OK();

            Assert.AreEqual(expected: MessageBoxResult.OK, actual: vm.ViewResult);
            Assert.IsTrue(closeCalled, message: "Close event should have fired");
        }

        [TestMethod()]
        public void CancelTest()
        {
            bool closeCalled = false;
            var mockNavigator = new Mock<NinjaMvvm.Wpf.Abstractions.INavigator>();

            var vm = new MessageBoxViewModel(mockNavigator.Object);
            mockNavigator.Setup(mck => mck.CloseDialog(vm))
                .Callback(() => closeCalled = true);

            vm.Cancel();

            Assert.AreEqual(expected: MessageBoxResult.Cancel, actual: vm.ViewResult);
            Assert.IsTrue(closeCalled, message: "Close event should have fired");
        }

        [TestMethod()]
        public void ShowMoreDetailsTest()
        {
            var vm = new MessageBoxViewModel();

            Assert.IsFalse(vm.DisplayMoreDetails, "DisplayMoreDetails should be false until ShowMoreDetails is fired");
            vm.ShowMoreDetails();
            Assert.IsTrue(vm.DisplayMoreDetails, "DisplayMoreDetails should be true after ShowMoreDetails is fired");

        }

        [TestMethod()]
        public void IconSourceIsSetTest()
        {
            var vm = new MessageBoxViewModel();

            Assert.IsNull(vm.IconSource, "IconSource should be null until Icon is set");
            vm.Icon = MessageBoxImage.Exclamation;
            Assert.IsNotNull(vm.IconSource, "IconSource should be not null after Icon is set");
        }
    }

}
