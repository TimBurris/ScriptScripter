using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;

namespace ScriptScripter.DesktopApp.ViewModels.Tests
{
    [TestClass()]
    public class AddDatabaseViewModelTests
    {
    }

    [TestClass()]
    public class AddDatabaseViewModel_SelectFileTestTests
    {
        private AddDatabaseViewModel _viewModel;
        private Mock<FileAndFolderDialog.Abstractions.IFileDialogService> _mockFileDialogService = new Mock<FileAndFolderDialog.Abstractions.IFileDialogService>();
        private FileAndFolderDialog.Abstractions.OpenFileOptions _optionsSentIn;

        [TestInitialize]
        public void Init()
        {
            _viewModel = new AddDatabaseViewModel(scriptContainerRepository: null, navigator: null, fileDialogService: _mockFileDialogService.Object, databaseConnectionControlVM: null, logger:null);
            _viewModel.DefaultFileNamePattern = "DBScripts_{DatabaseName}.xml";

            _mockFileDialogService.Setup(m => m.ShowSelectFileDialog(It.IsAny<FileAndFolderDialog.Abstractions.OpenFileOptions>()))
                .Callback<FileAndFolderDialog.Abstractions.OpenFileOptions>(options => _optionsSentIn = options);
        }


        [TestMethod]
        public void case_insensitive_matches_databasename_replacement_string()
        {
            //*************  arrange  ******************
            _viewModel.DefaultFileNamePattern = "DBScripts_{DATABASEname}.xml";
            _viewModel.DatabaseName = "Olaf";

            //*************    act    ******************
            _viewModel.SelectFile();

            //*************  assert   ******************
            _optionsSentIn.DefaultFileName.Should().Be("DBScripts_Olaf.xml");

        }

        [TestMethod]
        public void does_not_set_defaultfilename_when_databasename_is_null()
        {
            //*************  arrange  ******************
            _viewModel.DatabaseName = null;

            //*************    act    ******************
            _viewModel.SelectFile();

            //*************  assert   ******************
            _optionsSentIn.DefaultFileName.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void does_not_set_defaultfilename_when_databasename_is_empty()
        {
            //*************  arrange  ******************
            _viewModel.DatabaseName = string.Empty;

            //*************    act    ******************
            _viewModel.SelectFile();

            //*************  assert   ******************
            _optionsSentIn.DefaultFileName.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void sets_defaultfilename_when_databasename_is_not_empty()
        {
            //*************  arrange  ******************
            _viewModel.DatabaseName = "Olaf";

            //*************    act    ******************
            _viewModel.SelectFile();

            //*************  assert   ******************
            _optionsSentIn.DefaultFileName.Should().Be("DBScripts_Olaf.xml");

        }
    }
}