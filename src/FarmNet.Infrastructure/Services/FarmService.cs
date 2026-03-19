using AutoMapper;
using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Services;

public class FarmService(IRepository<Farm> repo, IUnitOfWork uow, IMapper mapper) : IFarmService
{
    public async Task<IEnumerable<FarmDto>> GetAllAsync()
    {
        var farms = await repo.Query().Include(f => f.Batches).OrderByDescending(f => f.NgayTao).ToListAsync();
        return mapper.Map<IEnumerable<FarmDto>>(farms);
    }

    public async Task<FarmDto?> GetByIdAsync(Guid id)
    {
        var farm = await repo.Query().Include(f => f.Batches).FirstOrDefaultAsync(f => f.Id == id);
        return farm == null ? null : mapper.Map<FarmDto>(farm);
    }

    public async Task<Result<FarmDto>> CreateAsync(TaoFarmRequest request)
    {
        var farm = new Farm { Id = Guid.NewGuid(), Ten = request.Ten, DiaChi = request.DiaChi, MoTa = request.MoTa };
        await repo.AddAsync(farm);
        await uow.SaveChangesAsync();
        return Result.Ok(mapper.Map<FarmDto>(farm));
    }

    public async Task<Result<FarmDto>> UpdateAsync(Guid id, TaoFarmRequest request)
    {
        var farm = await repo.GetByIdAsync(id);
        if (farm == null) return Result.Fail("Không tìm thấy trang trại");

        farm.Ten = request.Ten;
        farm.DiaChi = request.DiaChi;
        farm.MoTa = request.MoTa;
        repo.Update(farm);
        await uow.SaveChangesAsync();
        return Result.Ok(mapper.Map<FarmDto>(farm));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var farm = await repo.GetByIdAsync(id);
        if (farm == null) return Result.Fail("Không tìm thấy trang trại");
        repo.Remove(farm);
        await uow.SaveChangesAsync();
        return Result.Ok();
    }
}
