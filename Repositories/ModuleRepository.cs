using Core;
using Core.Repositories;
using Idata.Data;
using Idata.Data.Entities;
using Idata.Data.Entities.Isite;
using Isite.Repositories.Interfaces;

namespace Isite.Repositories
{
    public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
    {

        private const string controllerName = "Module";

        public ModuleRepository()
        {
        }

        public override void CustomFilters(ref IQueryable<Module> query, ref UrlRequestBase? requestBase)
        {
            query = query.Where(m => m.enabled == true);
        }

    }
}
