using Core;
using Core.Repositories;
using Idata.Entities.Isite;
using Isite.Repositories.Interfaces;

namespace Isite.Repositories
{
    public class RevisionRepository : RepositoryBase<Revision>, IRevisionRepository
    {
        public RevisionRepository()
        {

        }               
    }
}
