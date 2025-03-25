using Microsoft.Data.SqlClient;
using QuanLySanBong.Repository.Account;
using QuanLySanBong.Repository.Bill;
using QuanLySanBong.Repository.Booking;
using QuanLySanBong.Repository.Pitch;
using QuanLySanBong.Repository.PitchType;
using QuanLySanBong.Repository.Staff;

namespace QuanLySanBong.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }

        IStaffRepository Staffs { get; }

        IPitchTypeImageRepository PitchTypeImages { get; }

        IPitchTypeRepository PitchTypes { get; }

        IPitchRepository Pitches { get; }

        IBookingRepository Bookings { get; }

        IBillRepository Bills { get; }

        Task<int> CompleteAsync();
    }
}
