﻿namespace Domain.Models;

public class Settings
{
    public long Id { get; set; }

    public required string PortraitImageFileName { get; set; }

    public required string BackgroundImageFileName { get; set; }
}
