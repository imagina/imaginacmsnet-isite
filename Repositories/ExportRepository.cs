using Core;
using Core.Exceptions;
using Core.Storage.Interfaces;
using Isite.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Isite.Repositories
{
    public class ExportRepository : IExportRepository
    {
        IStorageBase _storageBase;
        public ExportRepository(IStorageBase storageBase)
        {
            _storageBase = storageBase;
        }
        public async Task<byte[]?> DownloadExport(UrlRequestBase urlRequestBase)
        {
            try
            {


                // Incoming File Stream
                var fileStream = await _storageBase.DownloadFile(urlRequestBase.filename) as Stream;

                // Create a new instance of memorystream
                var memoryStream = new MemoryStream();

                // Use the .CopyTo() method and write current filestream to memory stream
                fileStream.CopyTo(memoryStream);

                // Convert Stream To Array
                byte[] byteArray = memoryStream.ToArray();

                return byteArray;

            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, "Error consulting storage for file");
                return null;
            }



        }
        public virtual async Task<Dictionary<string, object>?[]> GetExport(UrlRequestBase? requestBase)
        {

            try
            {
                var reportFormats = Ihelpers.Helpers.ConfigurationHelper.GetConfig<string[]>("DefaultConfigs:ReportFormats");

                string? downloadEndpoint = Ihelpers.Helpers.ConfigurationHelper.GetConfig("DefaultConfigs:ReportsEndpoint");

                if (downloadEndpoint == null) throw new Exception("Reports Endpoint configuration not found on app settings file");

                Dictionary<string, object>?[] response;

                List<Dictionary<string, object>?> repoResponses = new();

                for (var i = 0; i < reportFormats.Count(); i++)
                {
                    var repoResponse = await _storageBase.ReadFile($"{requestBase.filename}.{reportFormats[i]}");



                    if (repoResponse != null)
                    {
                        //get the timezone of user;
                        var rawUserTimezone = (TimeSpan.Parse(requestBase.getRequestTimezone()));

                        //get the response from the storage
                        DateTime date = (DateTime)repoResponse["lastModified"];

                        //convert to user timezone
                        var userTimezoneDate = date.Add(rawUserTimezone);
                            
                        //re-set the value to dictionary
                        repoResponse["lastModified"] = userTimezoneDate;

                        //convert to UTC
                        repoResponses.Add(repoResponse);
                    }

                }


                return repoResponses.ToArray();

            }
            catch (Exception ex)
            {
                ExceptionBase.HandleException(ex, $"Error export of {requestBase.filename}", " trace received: " + JsonConvert.SerializeObject(requestBase));
                return null;
            }


        }

    }
}
