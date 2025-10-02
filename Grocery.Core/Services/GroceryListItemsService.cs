using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            //Haal alle boodschappenlijst-items op.
            var groceryListItems = _groceriesRepository.GetAll();

            //hiermee groepeer je de boodschappenlijst op productId en tel je hoeveel keer het verkocht is per product.
            var productSales = groceryListItems
                .GroupBy(item => item.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    NrOfSells = group.Count()
                })
                .OrderByDescending(g => g.NrOfSells)
                .Take(topX)
                .ToList();

            //Haal alle producten op voor extra info
            var products = _productRepository.GetAll();

            //maak de lijst met BestSellingProducts aan
            var bestSellingProducts = productSales
                .Join(products,
                      sale => sale.ProductId,
                      product => product.Id,
                      (sale, product) => new BestSellingProducts(product.Id, product.Name, product.Stock, sale.NrOfSells, 0))
                .ToList();
            //Rangschik de best verkochte producten
            for (int i = 0; i < bestSellingProducts.Count; i++)
            {
                bestSellingProducts[i].Ranking = i + 1;//rangschikking start op 1
            }
            return bestSellingProducts;


        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
