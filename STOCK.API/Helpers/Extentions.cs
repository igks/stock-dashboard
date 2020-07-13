using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPagination(this HttpResponse response,
            int currentPage, int pageSize, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, pageSize, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination",
                JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, BaseParams baseParams, Dictionary<string, Expression<Func<T, object>>> columnsMap)
        {
            if (String.IsNullOrWhiteSpace(baseParams.OrderBy) || !columnsMap.ContainsKey(baseParams.OrderBy))
                return query;

            if (baseParams.isDescending)
            {
                return query.OrderByDescending(columnsMap[baseParams.OrderBy]);
            }
            else
            {
                return query.OrderBy(columnsMap[baseParams.OrderBy]);
            }
        }
    }
}