using AutoMapper;
using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.DTOs.Search;
using RecambiosWOW.Application.Interfaces;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Application.Services;

public class PartService : IPartService
{
    private readonly IPartRepository _partRepository;
    private readonly IMapper _mapper;

    public PartService(IPartRepository partRepository, IMapper mapper)
    {
        _partRepository = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PartDto> GetByIdAsync(int id)
    {
        var part = await _partRepository.GetByIdAsync(id);
        return _mapper.Map<PartDto>(part);
    }

    public async Task<PartDto> GetBySerialNumberAsync(string serialNumber)
    {
        var part = await _partRepository.GetBySerialNumberAsync(serialNumber);
        return _mapper.Map<PartDto>(part);
    }

    public async Task<IEnumerable<PartDto>> SearchAsync(PartSearchCriteriaDto criteriaDto)
    {
        var criteria = _mapper.Map<PartSearchCriteria>(criteriaDto);
        var parts = await _partRepository.SearchAsync(criteria);
        return _mapper.Map<IEnumerable<PartDto>>(parts);
    }

    public async Task<PartDto> CreateAsync(PartDto partDto)
    {
        var part = _mapper.Map<Part>(partDto);
        var created = await _partRepository.AddAsync(part);
        return _mapper.Map<PartDto>(created);
    }

    public async Task<PartDto> UpdateAsync(PartDto partDto)
    {
        var part = _mapper.Map<Part>(partDto);
        var updated = await _partRepository.UpdateAsync(part);
        return _mapper.Map<PartDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _partRepository.DeleteAsync(id);
    }
}
