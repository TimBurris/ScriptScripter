using System.Windows;
using CommandLine;
using Ninject;

namespace ScriptScripter.DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            var kernel = new Ninject.StandardKernel(new Ninject.Modules.INinjectModule[] { new Ninjector(), new ScriptScripter.Processor.Ninjector() });

            Ninjector.Container = kernel;
            ScriptScripter.Processor.Ninjector.Container = kernel;

            var vm = this.InitMainWindow(Ninjector.Container);

            this.InitFromInputParams(vm, e.Args);
        }


        private ViewModels.MainViewModel InitMainWindow(IKernel container)
        {
            var x = new MainWindow();
            x.Show();

            //NOTE: you cannot create a vm until after mainwindow is started because the VM might need navigator, which can be ready until the window is available
            var vm = container.Get<ViewModels.MainViewModel>();
            x.DataContext = vm;
            var o = vm.ViewBound;

            return vm;
        }


        private void InitFromInputParams(ViewModels.MainViewModel vm, string[] args)
        {
            string addScriptContainerPath = null;
            var parseResult = Parser.Default.ParseArguments<IncomingOptions>(args)
                         .WithParsed<IncomingOptions>(o =>
                         {
                             addScriptContainerPath = o.AddScriptContainerPath;
                         })
                         ;
            if (!string.IsNullOrWhiteSpace(addScriptContainerPath))
            {
                vm.AddNewScriptForContainer(addScriptContainerPath);
            }
        }
    }
}
