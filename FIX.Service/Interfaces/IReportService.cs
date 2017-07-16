using System.Linq;
using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IReportService
    {
        IQueryable<DailyTrading> GetAllDailyTrading();
        DailyTrading GetDailyTrading(int tradeId);
        void InsertDailyTrading(DailyTrading dailyTrade);
        bool SaveChanges(int userId);
        void UpdateDailyTrading(DailyTrading dailyTrade);
    }
}