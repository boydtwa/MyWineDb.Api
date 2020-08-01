using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using TableContinuationToken = Microsoft.Azure.Cosmos.Table.TableContinuationToken;

namespace Microsoft.Azure.Cosmos.Table
{
    /// <summary>
    /// Thank you: https://stackoverflow.com/users/2795999/jose-ch
    /// see: https://stackoverflow.com/questions/24234350/how-to-execute-an-azure-table-storage-query-async-client-version-4-0-1
    /// </summary>
    public static class TableQueryExtensions
    {
        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query,
            CancellationToken ct = default(CancellationToken), System.Action<IList<T>> onProgress = null)
                where T : ITableEntity, new()
        {

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (onProgress != null) onProgress(items);

            } while (token != null && !ct.IsCancellationRequested);

            return items;
        }
    }
}