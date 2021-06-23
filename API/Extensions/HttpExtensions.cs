using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new 
            {
                currentPage,
                itemsPerPage,
                totalItems,
                totalPages
            };
            //Add new header property as JSON
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
            //Show new header property on client's browser
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}