﻿using QuanLySanBong.Helpers;
using QuanLySanBong.Repository.Account;
using QuanLySanBong.Repository.Bill;
using QuanLySanBong.Repository.Booking;
using QuanLySanBong.Repository.Pitch;
using QuanLySanBong.Repository.PitchType;
using QuanLySanBong.Repository.Staff;
using QuanLySanBong.Service.Account;
using QuanLySanBong.Service.Bill;
using QuanLySanBong.Service.Booking;
using QuanLySanBong.Service.File;
using QuanLySanBong.Service.Pitch;
using QuanLySanBong.Service.PitchType;
using QuanLySanBong.Service.Staff;
using QuanLySanBong.UnitOfWork;

namespace QuanLySanBong.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDependencyInjectionExtensions(this IServiceCollection services,
        IConfiguration config)
        {
            services.AddAutoMapper(typeof(Program));

            // Đăng ký các dịch vụ
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IStaffService, StaffService>();

            services.AddScoped<IPitchTypeRepository, PitchTypeRepository>();
            services.AddScoped<IPitchTypeService, PitchTypeService>();

            services.AddScoped<IPitchRepository, PitchRepository>();
            services.AddScoped<IPitchService, PitchService>();
            services.AddLogging();

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingService, BookingService>();

            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IBillService, BillService>();

            return services;
        }
    }
}
