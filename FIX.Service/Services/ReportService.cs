using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIX.Service
{
    public class ReportService : IReportService
    {
        private IUnitOfWork _uow;

        public ReportService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IQueryable<DailyTrading> GetAllDailyTrading()
        {
            return _uow.Repository<DailyTrading>().GetAsQueryable();
        }

        public DailyTrading GetDailyTrading(int tradeId)
        {
            return _uow.Repository<DailyTrading>().GetByKey(tradeId);
        }

        public void InsertDailyTrading(DailyTrading dailyTrade)
        {
            _uow.Repository<DailyTrading>().Insert(dailyTrade);
        }

        public void UpdateDailyTrading(DailyTrading dailyTrade)
        {
            _uow.Repository<DailyTrading>().Update(dailyTrade);
        }

        public bool SaveChanges(int userId)
        {
            return _uow.Save(userId);
        }
    }
}
