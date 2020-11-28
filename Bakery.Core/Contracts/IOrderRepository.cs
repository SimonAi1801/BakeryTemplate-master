using Bakery.Core.Entities;
using System.Threading.Tasks;

namespace Bakery.Core.Contracts
{
    public interface IOrderRepository
    {
        Task<int> GetCountAsync();

        Task<Order[]> GetAllAsync();
    }
}