using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvxForms.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {

        public override Task Initialize()
        {
            //TODO: Add starting logic here

            return base.Initialize();
        }

        public ICommand GoCommand
        {
            get
            {
                return new MvxCommand(() => ShowViewModel<TesteViewModel>());
            }
        }
    }
}
