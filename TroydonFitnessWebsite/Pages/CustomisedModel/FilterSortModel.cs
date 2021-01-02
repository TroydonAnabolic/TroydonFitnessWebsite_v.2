using Microsoft.AspNetCore.Identity;
using System.Linq;
using TroydonFitnessWebsite.Areas.Identity.Data;
using TroydonFitnessWebsite.Models;
using TroydonFitnessWebsite.Models.Products;

namespace TroydonFitnessWebsite.Pages.CustomisedModel
{
    public class FilterSortModel : ProductPageModel
    {
        public FilterSortModel(
            UserManager<TroydonFitnessWebsiteUser> userManager) : base(userManager) 
        {
        }

        public string IdSort { get; set; }
        public string PtIdSort { get; set; }
        public string SuppIdSort { get; set; }
        public string DietIdSort { get; set; }
        public string TrainingIdSort { get; set; }
        public string OrderIdSort { get; set; }
        public string CartIdSort { get; set; }
        public string TitleSort { get; set; }
        public string SuppNameSort { get; set; }
        public string PriceSort { get; set; }
        public string LenghtOfRoutineSort { get; set; }
        public string CustomerNameSort { get; set; }

        public string DateSort { get; set; }
        public string QuantitySort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public void AssigningSortOrderValues(string sortOrder)
        {
            IdSort = sortOrder == "id_sort" ? "id_sort_desc" : "id_sort";
            PtIdSort = sortOrder == "ptID" ? "ptID_desc" : "ptID";
            SuppIdSort = sortOrder == "suppID" ? "suppID_desc" : "suppID";
            TrainingIdSort = sortOrder == "trainID" ? "trainID_desc" : "trainID";
            DietIdSort = sortOrder == "dietID" ? "dietID_desc" : "dietID";
            OrderIdSort = sortOrder == "orderID" ? "orderID_desc" : "orderID";
            CartIdSort = sortOrder == "cartID" ? "cartID_desc" : "cartID";
            TitleSort = sortOrder == "Title" ? "title_desc" : "Title";
            SuppNameSort = sortOrder == "SuppName" ? "suppName_desc" : "SuppName";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
            QuantitySort = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            LenghtOfRoutineSort = sortOrder == "Length" ? "length_desc" : "Length";
            CustomerNameSort = sortOrder == "Customer" ? "customer_desc" : "Customer";
        }
    }
}
