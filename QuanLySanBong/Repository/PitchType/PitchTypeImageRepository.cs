using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Entities.PitchType.Model;
using QuanLySanBong.Repository.Interface;
using System;

namespace QuanLySanBong.Repository.PitchType
{
    public class PitchTypeImageRepository : GenericRepository<PitchTypeImageModel>, IPitchTypeImageRepository
    {
        private readonly ApplicationDbContext _context;

        public PitchTypeImageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PitchTypeImageModel>> GetByPitchTypeIdAsync(int pitchTypeId)
        {
            return await _context.PitchTypeImages
                .Where(i => i.PitchTypeId == pitchTypeId)
                //.Select(i => i.ImagePath)
                .ToListAsync();
        }

        public void RemoveRange(IEnumerable<PitchTypeImageModel> images)
        {
            _context.PitchTypeImages.RemoveRange(images);
        }
    }

}
