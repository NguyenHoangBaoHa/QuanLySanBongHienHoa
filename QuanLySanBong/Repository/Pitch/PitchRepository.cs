using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
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
            var pitches =  await _context.Pitches
                .Include(p => p.PitchType)
                .AsNoTracking()
                .ToListAsync();

            foreach (var pitch in pitches)
            {
                Console.WriteLine($"Pitch ID: {pitch.Id}, Name: {pitch.Name}, PitchType: {(pitch.PitchType != null ? pitch.PitchType.Name : "NULL")}");
            }

            return pitches;
        }

        public async Task<PitchModel> GetByIdAsync(int id)
        {
            return await _context.Pitches
                .Include(p => p.PitchType)
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
            if (pitch != null)
            {
                _context.Pitches.Remove(pitch);
                await _context.SaveChangesAsync();
            }
        }
    }
}
