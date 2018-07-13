using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;

namespace ScriptScripter.Processor.Services.Tests
{
    [TestClass()]
    public class ScriptContainerWatcherServiceTests
    {

    }

    public abstract class ScriptContainerWatcherServiceTestBase
    {
        protected ScriptContainerWatcherService _service;
        protected Mock<Data.Contracts.IScriptContainerRepository> _mockScriptContainerRepo;
        protected Mock<Services.Contracts.IEventNotificationService> _mockEventNotificationService;
        protected Mock<IFileSystemWatcherFactory> _mockFileSystemWatcherFactory;
        protected FakeFileSystemWatcher _fakeFileSystemWatcher;

        protected List<Data.Models.ScriptContainer> _existingContainers;
        protected Moq.Language.Flow.ISetup<Contracts.IEventNotificationService> _contentsChangedSetup;

        protected Data.Models.ScriptContainer _container;

        [TestInitialize]
        public void BaseInit()
        {
            _mockScriptContainerRepo = new Mock<Data.Contracts.IScriptContainerRepository>();
            _mockEventNotificationService = new Mock<Contracts.IEventNotificationService>();
            //_fakeFileSystemWatcher = new FakeFileSystemWatcher();
            _mockFileSystemWatcherFactory = new Mock<IFileSystemWatcherFactory>();
            _existingContainers = new List<Data.Models.ScriptContainer>();
            _container = new Data.Models.ScriptContainer()
            {
                ContainerUid = new Guid("9A92D3F5-144E-4554-9E4D-596216BC8471"),
                DatabaseName = "TestDb",
                ScriptFilePath = @"c:\temp\testdb.xml"
            };

            _service = new ScriptContainerWatcherService(_mockScriptContainerRepo.Object, _mockEventNotificationService.Object, _mockFileSystemWatcherFactory.Object);

            _mockScriptContainerRepo.Setup(m => m.GetAll())
                .Returns(() => _existingContainers);
            _mockFileSystemWatcherFactory.Setup(m => m.Create())
                .Returns(() =>
                {
                    _fakeFileSystemWatcher = new FakeFileSystemWatcher();
                    return _fakeFileSystemWatcher;
                });

            _contentsChangedSetup = _mockEventNotificationService.Setup(m => m.NotifyScriptContainerContentsChanged(_container));
            _contentsChangedSetup.Verifiable();
        }
    }

    [TestClass]
    public class ScriptContainerWatcherService_given_container_exists_when_Begin : ScriptContainerWatcherServiceTestBase
    {

        [TestInitialize]
        public void Init()
        {
            //put the container in BEFORE begin
            _existingContainers.Add(_container);

            _service.BeginWatchingAllContainers();
        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_changed()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();

        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_created()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseCreated();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();


        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_deleted()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();
        }

        [TestMethod]
        public void when_removed_does_not_raise_events()
        {
            /*************  arrange  ******************/
            //say that we removed the container
            _mockEventNotificationService.Raise(m => m.ScriptContainerRemoved += null, new EventArgs<Data.Models.ScriptContainer>(_container));

            _contentsChangedSetup.Callback(() => Assert.Fail("The contents changed should NOT have fired.  now you are fired."));

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();
            _fakeFileSystemWatcher.RaiseCreated();
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            //nothing to assert, if the callback we setup doesn't fire then world is happy
        }

        [TestMethod]
        public void when_endwatching_does_not_raise_events()
        {
            /*************  arrange  ******************/
            _service.EndWatchingAllcontainers();

            _contentsChangedSetup.Callback(() => Assert.Fail("The contents changed should NOT have fired.  now you are fired."));

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();
            _fakeFileSystemWatcher.RaiseCreated();
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            //nothing to assert, if the callback we setup doesn't fire then world is happy
        }
    }

    [TestClass]
    public class ScriptContainerWatcherService_given_container_added_after_Begin : ScriptContainerWatcherServiceTestBase
    {

        [TestInitialize]
        public void Init()
        {
            _service.BeginWatchingAllContainers();

            //here we are saying we added the container after begin was called 
            _mockEventNotificationService.Raise(m => m.ScriptContainerAdded += null, new EventArgs<Data.Models.ScriptContainer>(_container));
        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_changed()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();

        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_created()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseCreated();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();


        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_deleted()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();

        }

        [TestMethod]
        public void when_removed_does_not_raise_events()
        {
            /*************  arrange  ******************/
            //say that we removed the container
            _mockEventNotificationService.Raise(m => m.ScriptContainerRemoved += null, new EventArgs<Data.Models.ScriptContainer>(_container));

            _contentsChangedSetup.Callback(() => Assert.Fail("The contents changed should NOT have fired.  now you are fired."));

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();
            _fakeFileSystemWatcher.RaiseCreated();
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            //nothing to assert, if the callback we setup doesn't fire then world is happy
        }

        [TestMethod]
        public void when_endwatching_does_not_raise_events()
        {
            /*************  arrange  ******************/
            _service.EndWatchingAllcontainers();

            _contentsChangedSetup.Callback(() => Assert.Fail("The contents changed should NOT have fired.  now you are fired."));

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();
            _fakeFileSystemWatcher.RaiseCreated();
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            //nothing to assert, if the callback we setup doesn't fire then world is happy
        }
    }

    [TestClass]
    public class ScriptContainerWatcherService_given_container_Updated_after_Begin : ScriptContainerWatcherServiceTestBase
    {

        [TestInitialize]
        public void Init()
        {
            //put the container in begin with the same Uid
            _existingContainers.Add(new Data.Models.ScriptContainer()
            {
                ContainerUid = _container.ContainerUid,
                DatabaseName = "origina",
                ScriptFilePath = @"c:\temp\originaldb.xml"
            });

            _service.BeginWatchingAllContainers();

            //now we are saying the container has changed (same UID), so all events should be with new NEW container
            _mockEventNotificationService.Raise(m => m.ScriptContainerUpdated += null, new EventArgs<Data.Models.ScriptContainer>(_container));
        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_changed()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();

        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_created()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseCreated();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();


        }

        [TestMethod]
        public void Raises_NotifyScriptContainerContentsChanged_when_file_deleted()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            _mockEventNotificationService.Verify();

        }

        [TestMethod]
        public void when_removed_does_not_raise_events()
        {
            /*************  arrange  ******************/
            //say that we removed the container
            _mockEventNotificationService.Raise(m => m.ScriptContainerRemoved += null, new EventArgs<Data.Models.ScriptContainer>(_container));

            _contentsChangedSetup.Callback(() => Assert.Fail("The contents changed should NOT have fired.  now you are fired."));

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();
            _fakeFileSystemWatcher.RaiseCreated();
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            //nothing to assert, if the callback we setup doesn't fire then world is happy
        }

        [TestMethod]
        public void when_endwatching_does_not_raise_events()
        {
            /*************  arrange  ******************/
            _service.EndWatchingAllcontainers();

            _contentsChangedSetup.Callback(() => Assert.Fail("The contents changed should NOT have fired.  now you are fired."));

            /*************    act    ******************/
            _fakeFileSystemWatcher.RaiseChanged();
            _fakeFileSystemWatcher.RaiseCreated();
            _fakeFileSystemWatcher.RaiseDeleted();

            /*************  assert   ******************/
            //nothing to assert, if the callback we setup doesn't fire then world is happy
        }
    }

    public class FakeFileSystemWatcher : System.IO.FileSystemWatcher
    {
        public void RaiseChanged()
        {
            this.RaiseChanged(new System.IO.FileSystemEventArgs(changeType: System.IO.WatcherChangeTypes.Changed, directory: "", name: ""));
        }

        public void RaiseChanged(System.IO.FileSystemEventArgs e)
        {
            base.OnChanged(e);
        }

        public void RaiseCreated()
        {
            this.RaiseCreated(new System.IO.FileSystemEventArgs(changeType: System.IO.WatcherChangeTypes.Created, directory: "", name: ""));
        }

        public void RaiseCreated(System.IO.FileSystemEventArgs e)
        {
            base.OnCreated(e);
        }


        public void RaiseDeleted()
        {
            this.RaiseDeleted(new System.IO.FileSystemEventArgs(changeType: System.IO.WatcherChangeTypes.Deleted, directory: "", name: ""));
        }

        public void RaiseDeleted(System.IO.FileSystemEventArgs e)
        {
            base.OnDeleted(e);
        }
    }

}