using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.FrontEnd.Models
{
    public class PaymentHistoryModel
    {

            public int order_id { get; set; }
            public double total { get; set; }

            public DateTime order_date { get; set; }

            public string order_status { get; set; }

    }
}
