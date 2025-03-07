﻿using Microsoft.Data.SqlClient;
using QuanLySanBong.Repository.Account;
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

        Task<int> CompleteAsync();

        // 📌 Thêm phương thức để gọi Stored Procedure
        Task<List<T>> ExecuteStoredProcedureAsync<T>(string storedProcedureName, Func<SqlDataReader, T> mapFunction);
    }
}
