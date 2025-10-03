using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;


namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    OnPropertyChanged();
                    OnSelectedProductChanged();
                }
            }
        }

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
            BoughtProductsList = new ObservableCollection<BoughtProducts>();
        }

        // Deze methode vult de lijst bij een andere selectie
        public void OnSelectedProductChanged()
        {
            BoughtProductsList.Clear();
            if (SelectedProduct != null)
            {
                var boughtProducts = _boughtProductsService.Get(SelectedProduct.Id);
                foreach (var bp in boughtProducts)
                {
                    BoughtProductsList.Add(bp);
                }
            }
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
