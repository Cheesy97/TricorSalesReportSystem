using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using SalesReportSystem.Model;
using SalesReportSystem.ViewModel;

namespace SalesReportSystem.Pages.Report.Model
{
    public interface IPageListService
    {
        Task<List<SaleJoinUserModel>> GetPaginatedResult(int currentPage, int pageSize = 10);
        Task<int> GetCount();
    }

    public class PageListService : IPageListService
    {
        private AuthDbContext _context { get; }
        private readonly IAccessService _accessService;

        public PageListService(AuthDbContext _context, IAccessService accessService)
        {
            this._context = _context;
            this._accessService = accessService;
        }

        public async Task<List<SaleJoinUserModel>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            var data = await GetData();
            return data.OrderBy(d => d.SaleId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<int> GetCount()
        {
            var data = await GetData();
            return data.Count;
        }

        private async Task<List<SaleJoinUserModel>> GetData()
        {
           UserAccess user = await _accessService.GetCurrentUser();
           var sList = await (from s in _context.sales
                        join u in _context.users on s.UserId equals u.UserId
                        where s.UserId == user.UserId
                              orderby s.SaleId descending
                        select new SaleJoinUserModel()
                        {
                            SaleId = s.SaleId,
                            SaleDate = s.SaleDate,
                            Amount = s.Amount,
                            UpdateDate = s.UpdateDate,
                            SalePersonName = u.Name
                        }).ToListAsync();

            return sList;
        }
    }
}
