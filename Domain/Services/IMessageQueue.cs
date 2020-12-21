using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IMessageQueue
    {
         Task Consumer(int Id);
         Task Publisher(int Id);
    }
}
