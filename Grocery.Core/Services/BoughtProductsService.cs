using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            var result = new List<BoughtProducts>();

            if (productId == null)
                return result;

            // Stap 1: Haal alle items op
            var items = _groceryListItemsRepository.GetAll()
                .Where(item => item.ProductId == productId.Value);

            foreach (var item in items)
            {
                // Stap 2: Haal boodschappenlijst op
                var groceryList = _groceryListRepository.Get(item.GroceryListId);
                if (groceryList == null) continue;

                // Stap 3: Haal client op
                var client = _clientRepository.Get(groceryList.ClientId);
                if (client == null) continue;

                // Stap 4: Haal product op
                var product = _productRepository.Get(item.ProductId);
                if (product == null) continue;

                // Stap 5: Voeg toe aan resultaat
                result.Add(new BoughtProducts(client, groceryList, product));
            }

            return result;
        }
    }
}
