using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Application.Services.Interfaces
{
    public interface IUpdateTimeStampService
    {
        //update time of the cart update time in data base
        Task<string> updateDateTime(string userId);
    }
}
