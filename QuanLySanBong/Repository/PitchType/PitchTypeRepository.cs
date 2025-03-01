using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Entities.PitchType.Model;
using QuanLySanBong.Repository.Interface;
using System;

namespace QuanLySanBong.Repository.PitchType
{
    public class PitchTypeRepository : GenericRepository<PitchTypeModel>, IPitchTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public PitchTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PitchTypeModel>> GetAllAsync()
        {
            return await _context.PitchTypes
                .Include(pt=>pt.Images)
                .ToListAsync();
        }

        public async Task<PitchTypeModel> GetByIdAsync(int id)
        {
            return await _context.PitchTypes
             .Include(pt => pt.Images)
             .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task AddAsync(PitchTypeModel pitchType)
        {
            await _context.PitchTypes.AddAsync(pitchType);
            await _context.SaveChangesAsync(); // Chỉ cần gọi 1 lần
        }

        public async Task UpdateAsync(PitchTypeModel pitchType)
        {
            _context.PitchTypes.Update(pitchType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PitchTypeModel pitchType)
        {
            _context.PitchTypes.Remove(pitchType);
            await _context.SaveChangesAsync();
        }
    }
}
