using Bakery.Core.DTOs;
using Bakery.Wpf.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bakery.Wpf.ViewModels
{
    public class EditCreateProductViewModel : BaseViewModel
    {
        public EditCreateProductViewModel(IWindowController controller, ProductDto product) : base(controller)
        {

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
