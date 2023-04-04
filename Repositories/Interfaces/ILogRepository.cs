using Core;

namespace Isite.Repositories.Interfaces
{
    public interface ILogRepository
    {
        public Task<object?> GetItemsBy(UrlRequestBase? requestBase);

        public Task<object?> GetItem(UrlRequestBase? requestBase);



    }
}
