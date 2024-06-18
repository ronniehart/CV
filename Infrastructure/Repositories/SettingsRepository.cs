﻿using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SettingsRepository(IDbContextFactory<CvContext> contextFactory) : ISettingsRepository
{
    private readonly IDbContextFactory<CvContext> _contextFactory = contextFactory;

    public async Task<Settings> GetSettingsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.Settings.FirstOrDefaultAsync();

        if (settings is null)
        {
            settings = new Settings
            {
                BackgroundImageFileName = "profile-background.jpg",
                PortraitImageFileName = "portrait.jpg"
            };

            await SetSettingsAsync(settings);
        }

        return settings;
    }

    public async Task SetSettingsAsync(Settings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.PortraitImageFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.BackgroundImageFileName);

        using var context = await _contextFactory.CreateDbContextAsync();
        var existingSettings = await context.Settings.FirstOrDefaultAsync();

        if (existingSettings is null)
        {
            context.Settings.Add(settings);
        }
        else
        {
            existingSettings.PortraitImageFileName = settings.PortraitImageFileName;
            existingSettings.BackgroundImageFileName = settings.BackgroundImageFileName;
        }

        await context.SaveChangesAsync();
    }
}
