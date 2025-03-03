using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuanLySanBong.Entities.Account.Model;
using QuanLySanBong.Entities.Booking.Model;
using QuanLySanBong.Entities.Customer.Model;
using QuanLySanBong.Entities.Pitch.Model;
using QuanLySanBong.Entities.PitchType.Model;
using QuanLySanBong.Entities.Staff.Model;

namespace QuanLySanBong.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<StaffModel> Staffs { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }

        public DbSet<PitchTypeModel> PitchTypes { get; set; }

        public DbSet<PitchTypeImageModel> PitchTypeImages { get; set; }

        public DbSet<PitchModel> Pitches { get; set; }

        public DbSet<BookingModel> Bookings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Định nghĩa cấu trúc cho bảng Account
            modelBuilder.Entity<AccountModel>()
                .ToTable("Account")
                .HasKey(a => a.Id);
            modelBuilder.Entity<AccountModel>()
                .Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(256);
            modelBuilder.Entity<AccountModel>()
                .Property(a => a.Password)
                .IsRequired()
                .HasMaxLength(256);
            modelBuilder.Entity<AccountModel>()
                .Property(a => a.Role)
                .IsRequired();

            // Seed dữ liệu cho tài khoản Admin
            modelBuilder.Entity<AccountModel>().HasData(new AccountModel
            {
                Id = 1,
                Email = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Mã hóa mật khẩu
                Role = "Admin",
                IdCustomer = null,
                IdStaff = null
            });

            // Định nghĩa cấu trúc cho bảng Staff
            modelBuilder.Entity<StaffModel>()
                .ToTable("Staff")
                .HasKey(s => s.Id);

            modelBuilder.Entity<StaffModel>()
                .Property(s => s.DisplayName)
                .IsRequired();

            modelBuilder.Entity<StaffModel>()
                .Property(s => s.StartDate)
                .IsRequired();

            // Định nghĩa cấu trúc cho bảng Customer
            modelBuilder.Entity<CustomerModel>()
                .ToTable("Customer")
                .HasKey(c => c.Id);

            modelBuilder.Entity<CustomerModel>()
                .Property(c => c.DisplayName)
                .IsRequired();

            // Thiết lập mối quan hệ 1-1 giữa Staff và Account nếu cần
            modelBuilder.Entity<AccountModel>()
                .HasOne(a => a.Staff)
                .WithOne(s => s.Account)
                .HasForeignKey<AccountModel>(a => a.IdStaff);

            // Thiết lập mối quan hệ 1-1 giữa Customer và Account nếu cần
            modelBuilder.Entity<AccountModel>()
                .HasOne(a => a.Customer)
                .WithOne(c => c.Account)
                .HasForeignKey<AccountModel>(a => a.IdCustomer);

            // Cấu hình bảng Pitch (Sân)
            modelBuilder.Entity<PitchModel>(entity =>
            {
                entity.ToTable("Pitch");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.CreateAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.UpdateAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                // Cấu hình quan hệ với PitchType
                entity.HasOne(p => p.PitchType)
                      .WithMany(pt => pt.Pitches)
                      .HasForeignKey(p => p.IdPitchType)
                      .OnDelete(DeleteBehavior.SetNull); // Xóa loại sân không xóa sân
            });

            // Cấu hình bảng PitchType (Loại sân)
            modelBuilder.Entity<PitchTypeModel>(entity =>
            {
                entity.ToTable("PitchType");
                entity.HasKey(pt => pt.Id);

                entity.Property(pt => pt.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(pt => pt.Price)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(pt => pt.LimitPerson)
                      .IsRequired();
            });

            modelBuilder.Entity<PitchTypeImageModel>(entity =>
            {
                entity.ToTable("PitchTypeImage");
                entity.HasKey(pi => pi.Id);

                entity.Property(pi => pi.ImagePath)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasOne(pi => pi.PitchType)
                      .WithMany(pt => pt.Images) // Một PitchType có nhiều ảnh
                      .HasForeignKey(pi => pi.PitchTypeId)
                      .OnDelete(DeleteBehavior.Cascade); // Nếu xóa PitchType thì xóa luôn ảnh
            });

            modelBuilder.Entity<BookingModel>(entity =>
            {
                entity.ToTable("Booking");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.BookingDate)
                      .IsRequired();

                entity.Property(b => b.PaymentStatus)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(b => b.IsReceived)
                      .IsRequired()
                      .HasDefaultValue(false);

                entity.Property(b => b.CreateAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(b => b.UpdateAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                // Quan hệ với Customer
                entity.HasOne(b => b.Customer)
                      .WithMany(c => c.Bookings)
                      .HasForeignKey(b => b.IdCustomer)
                      .OnDelete(DeleteBehavior.Restrict); // Không xóa Customer khi xóa Booking

                // Quan hệ với Pitch
                entity.HasOne(b => b.Pitch)
                      .WithMany(p => p.Bookings)
                      .HasForeignKey(b => b.IdPitch)
                      .OnDelete(DeleteBehavior.Restrict); // Không xóa Pitch khi xóa Booking
            });
        }
    }
}
