using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Bakery.Persistence;
using Bakery.Wpf.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bakery.Wpf.ViewModels
{
    public class EditCreateProductViewModel : BaseViewModel
    {
        private Product _productTmp = new Product();
        private Product _product;
        private bool _isCreate = false;
        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        private string _productNr;

        public string ProductNr
        {
            get => _productNr;
            set
            {
                _productNr = value;
                OnPropertyChanged(nameof(ProductNr));
                ValidateViewModelProperties();
            }
        }

        private string _productName;

        [MaxLength(20)]
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
                ValidateViewModelProperties();
            }
        }

        private string _price;

        public string Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
                ValidateViewModelProperties();
            }
        }


        public EditCreateProductViewModel(IWindowController controller, ProductDto product) : base(controller)
        {
            if (product != null)
            {
                Product = new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ProductNr = product.ProductNr
                };
                ProductNr = product.ProductNr;
                ProductName = product.Name;
                Price = product.Price.ToString();
                _productTmp = Product;
            }
            else
            {
                _isCreate = true;
            }
        }

        private ICommand _cmdSaveCommand;

        public ICommand CmdSaveCommand
        {
            get
            {
                if (_cmdSaveCommand == null)
                {
                    _cmdSaveCommand = new RelayCommand(
                        execute: async _ => await SaveProductAsync(),
                        canExecute: _ => !HasErrors);
                }
                return _cmdSaveCommand;
            }
        }

        private ICommand _cmdUndoCommand;

        public ICommand CmdUndoCommand
        {
            get 
            {
                if (_cmdUndoCommand == null)
                {
                    _cmdUndoCommand = new RelayCommand(
                        execute: _ => 
                        {
                            Product = _productTmp;
                            ProductNr = _productTmp.ProductNr;
                            ProductName = _productTmp.Name;
                            Price = "";
                        },
                        canExecute: _ => ProductNr != null || ProductName != null || Price != null);
                }
                return _cmdUndoCommand;
            }
        }


        private async Task SaveProductAsync()
        {
            try
            {
                await using IUnitOfWork uow = new UnitOfWork();

                if (_isCreate)
                {
                    Product = new Product
                    {
                        Name = ProductName,
                        ProductNr = ProductNr,
                        Price = Convert.ToDouble(Price)
                    };
                    await uow.Products.AddAsync(Product);
                }
                else
                {
                    var productInDb = await uow.Products.GetByIdAsync(Product.Id);
                    productInDb.Name = ProductName;
                    productInDb.ProductNr = ProductNr;
                    productInDb.Price = Convert.ToDouble(Price);
                    uow.Products.Update(productInDb);
                }

                await uow.SaveChangesAsync();
                Controller.CloseWindow(this);
            }
            catch (ValidationException ex)
            {
                if (ex.Value is IEnumerable<string> properties)
                {
                    foreach (var property in properties)
                    {
                        Errors.Add(property, new List<string> { ex.ValidationResult.ErrorMessage });
                    }
                }
                else
                {
                    DbError = ex.ValidationResult.ToString();
                }
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult> { ValidationResult.Success };
        }
    }
}
