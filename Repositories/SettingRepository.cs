using Core;
using Isite.Repositories.Interfaces;

namespace Isite.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        public Task<object?> Create(UrlRequestBase? requestBase, BodyRequestBase? bodyRequestBase)
        {
            throw new NotImplementedException();
        }

        public Task<object?> DeleteBy(UrlRequestBase? requestBase, dynamic? modelToRemove = null)
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetItem(UrlRequestBase? requestBase)
        {
            throw new NotImplementedException();
        }

        public Task<List<object>> GetItemsBy(UrlRequestBase? requestBase)
        {
            throw new NotImplementedException();
        }

        public Task<object> UpdateBy(UrlRequestBase? requestBase, BodyRequestBase? bodyRequestBase)
        {
            throw new NotImplementedException();
        }
    }
}
