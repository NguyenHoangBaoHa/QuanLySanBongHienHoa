using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Entities.Enums;
using QuanLySanBong.Entities.Pitch.Model;

namespace QuanLySanBong.Repository.Pitch
{
    public class PitchRepository : IPitchRepository
    {
        private readonly ApplicationDbContext _context;

        public PitchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PitchModel>> GetAllAsync()
        {
            var pitches = await _context.Pitches
                .Include(p => p.PitchType)
                .ThenInclude(pt => pt.Images)
                .AsNoTracking()
                .ToListAsync();

            return pitches;
        }

        public async Task<PitchModel> GetByIdAsync(int id)
        {
            return await _context.Pitches
                .Include(p => p.PitchType)
                .ThenInclude(pt => pt.Images)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(PitchModel pitch)
        {
            _context.Pitches.Add(pitch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PitchModel pitch)
        {
            _context.Pitches.Update(pitch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pitch = await _context.Pitches.FindAsync(id);
            if (pitch == null) return;

           _context.Pitches.Remove(pitch);
            await _context.SaveChangesAsync();
        }

        public IQueryable<PitchModel> GetQueryable()
        {
            return _context.Pitches
                .Include(p => p.PitchType)
                    .ThenInclude(pt => pt.Images)
                .AsNoTracking();
        }
    }
}
