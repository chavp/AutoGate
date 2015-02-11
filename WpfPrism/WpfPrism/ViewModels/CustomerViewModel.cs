using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPrism.ViewModels
{
    public class CustomerViewModel
        : BindableBase
    {
        public string Name { get; set; }
    }
}
