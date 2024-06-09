﻿using Application.Dtos;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Services.Resources.Impl;

public class SkillService(
    IServiceProvider serviceProvider, 
    ILogger<SkillService> logger) : ResourceService<SkillDto>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<SkillService> _logger = logger;

    public override async Task<IEnumerable<SkillDto>> GetResourcesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CvContext>();

        var skills = await context.Skills.ToListAsync();
        var skillDtos = skills.Select(ConvertToSkillDto);
        return skillDtos;
    }

    public override async Task AddResourceAsync(SkillDto resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CvContext>();

        var skillModel = new Skill
        {
            Name = resource.Name,
            Level = resource.Level
        };

        context.Skills.Add(skillModel);
        await context.SaveChangesAsync();
        _logger.LogInformation("Added skill '{}'", resource.Name);
    }

    public override async Task UpdateResourceAsync(SkillDto resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CvContext>();

        var existingSkill = await context.Skills.FindAsync(resource.Id)
            ?? throw new InvalidOperationException($"No skill with id '{resource.Id}' found");

        if (!resource.IsEqualToModel(existingSkill))
        {
            existingSkill.Name = resource.Name;
            existingSkill.Level = resource.Level;

            await context.SaveChangesAsync();
            _logger.LogInformation("Updated skill '{}'", resource.Id);
        }
    }

    public override async Task DeleteResourceAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentException($"Provided id '{id}' is invalid.");
        }

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CvContext>();

        var skill = await context.Skills.FindAsync(id);
        if (skill is null)
        {
            _logger.LogError("Couldn't find skill with id '{}'", id);
            throw new InvalidOperationException($"Couldn't find skill with id '{id}'.");
        }

        context.Skills.Remove(skill);
        await context.SaveChangesAsync();
        _logger.LogInformation("Removed skill '{}'", id);
    }

    private static SkillDto ConvertToSkillDto(Skill skill)
    {
        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Level = skill.Level
        };
    }
}