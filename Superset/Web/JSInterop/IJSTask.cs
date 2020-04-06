using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Superset.Web.JSInterop
{
    public interface IJSTask<T>
    {
        ValueTask<T> Execute(IJSRuntime runtime);
    }

    public interface IJSTask
    {
        Task Execute(IJSRuntime runtime);
    }
}