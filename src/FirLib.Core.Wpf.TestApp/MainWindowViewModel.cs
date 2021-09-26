using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Tests.Wpf
{
    public class MainWindowViewModel : ViewModelBase
    {
        public DelegateCommand Command_TriggerException { get; }

        public DelegateCommand Command_TriggerWithInnerException { get; }

        public DelegateCommand Command_TriggerFileNotFoundException { get; }

        public DelegateCommand Command_TriggerAggregateException { get; }

        public MainWindowViewModel()
        {
            this.Command_TriggerException =
                new DelegateCommand(() => throw new ApplicationException("Dummy exception"));
            this.Command_TriggerWithInnerException =
                new DelegateCommand(() =>
                {
                    try
                    {
                        throw new ApplicationException("Inner exception message");
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException("Outer exception message", e);
                    }
                });
            this.Command_TriggerFileNotFoundException =
                new DelegateCommand(() => File.OpenRead("ThisFileDoesNotExist.txt"));
            this.Command_TriggerAggregateException =
                new DelegateCommand(async () =>
                {
                    await Task.Delay(2);

                    Task.Factory.StartNew(() => throw new ApplicationException("Exception out of AggregateException")).Wait();
                });
        }
    }
}
