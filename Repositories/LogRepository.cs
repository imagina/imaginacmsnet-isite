
using Core;
using Core.Exceptions;
using Core.Logger;
using Idata.Data;
using Idata.Entities.Core;
using Ihelpers.Helpers;
using Isite.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace Isite.Repositories
{
    public class LogRepository : ILogRepository
    {
        protected readonly IdataContext _dataContext;

        protected readonly dynamic? entity = Activator.CreateInstance(typeof(Log));

        protected readonly IQueryable<Log> _dbSet;

        public LogRepository(IdataContext dataContext)
        {
            _dataContext = dataContext;
            _dbSet = _dataContext.Logs;
        }


        public virtual async Task<object?> GetItem(UrlRequestBase? requestBase)
        {
            Log? model = new(); //object to be returned
            //set the current context user 
            //requestBase.setCurrentContextUser(_contextUser);

            var _dbSet = _dataContext.Logs as IQueryable<Log>;
            try
            {

                //Try get the search filter
                string field = requestBase.GetFilter("field");

                //Create base query based on criteria and field
                var query = _dbSet.Where($"obj => obj.{field} == @0", requestBase.criteria);

                // Custom filters from the Repository child
                this.CustomFilters(ref query, ref requestBase);

                // adding dynamic filters 
                //This doesnt apply for those objects that are different from EntityBase
                query = requestBase.SetDynamicFilters(query, entity);

                //Include the linked user
                query = query.Include(ob => ob.user);
                //TODO inyect the include parameters as given in front
                //This doesnt apply for those objects that are different from EntityBase
                //query = requestBase.GetIncludes(query, entity);

                //get the model with given criteria
                model = await query.FirstOrDefaultAsync();

                //if model is null (not found) throw exception
                if (model == null) throw new ExceptionBase($"{typeof(Log).Name} with {field} {requestBase.criteria} not found ", 404);
                //try get the user
                dynamic? user = (requestBase.currentContextUser != null) ? requestBase.currentContextUser : null;
                //try get the email of the actual user
                string userEmail = (user != null) ? (string)user.email : "Anonymous";
                //try get the userId of the actual user
                long? userId = (user != null) ? (long?)user.id : null;

                Task.Factory.StartNew(() => CoreLogger.LogMessage($"{userEmail} has seen log: {model.id}", logType: Ihelpers.Helpers.LogType.Information, userId: userId));


                //if the model is valid return the item
                return model;

            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, $"Error obtaining {typeof(Log).Name}", " trace received: " + JsonConvert.SerializeObject(requestBase) + "\r\n");

            }
            return model;
        }

        public async Task<object?> GetItemsBy(UrlRequestBase? requestBase)
        {

            PaginatedList<Log>? resultList = null;




            try
            {


                IQueryable<Log> query = _dbSet;

                // Custom filters from the Repository child
                this.CustomFilters(ref query, ref requestBase);

                //Verify the filters that are present in requestBase
                // adding dynamic filters 
                //This doesnt apply for those objects that are different from EntityBase


                query = requestBase.SetDynamicFilters(query, entity);

                //Include the linked user
                query = query.Include(ob => ob.user);

                //Get includes and apply them
                //This doesnt apply for those objects that are different from EntityBase
                //query = requestBase.GetIncludes(query, entity);
                //try get the actual user from urlRequestbase
                dynamic? user = (requestBase.currentContextUser != null) ? requestBase.currentContextUser : null;
                //try get the email of the actual user
                string userEmail = (user != null) ? (string)user.email : "Anonymous";
                //try get the userId of the actual user
                long? userId = (user != null) ? (long?)user.id : null;

                Task.Factory.StartNew(() => CoreLogger.LogMessage($"{userEmail} has listed logs", logType: Ihelpers.Helpers.LogType.Information, userId: userId));


                //Use of PaginatedList to get the items and the meta of them
                resultList = await PaginatedList<Log>.CreateAsync(query, requestBase.page, requestBase.take);



            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, $"Error obtaining list of {typeof(Log).Name}", " trace received: " + JsonConvert.SerializeObject(requestBase));
            }

            return resultList;
        }


        public virtual void CustomFilters(ref IQueryable<Log> query, ref UrlRequestBase? requestBase)
        {
            return;
        }




    }
}
