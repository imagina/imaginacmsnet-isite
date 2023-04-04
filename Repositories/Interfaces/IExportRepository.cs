using Core;
namespace Isite.Repositories.Interfaces
{
    public interface IExportRepository
    {
        Task<byte[]?> DownloadExport(UrlRequestBase urlRequestBase);
        Task<Dictionary<string, object>?[]> GetExport(UrlRequestBase? requestBase);
    }
}
