using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

/// <summary>
/// предпочтения
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferenceListController : ControllerBase
{
    private IRepository<Preference> _repository;

    public PreferenceListController(IRepository<Preference> repository)
    {
        _repository = repository;
    }
    
    /// <summary>
    /// Получить данные всех Preference
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<PreferenceResponse>> GetPreferencesAsync()
    {
        var preferences = await _repository.GetAllAsync();
        var responses = preferences.Select(x =>
                
            new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

        return responses;
    }
}