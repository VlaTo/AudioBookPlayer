﻿using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Data
{
    public interface IBooksRepository : IRepository<AudioBook>, IBooksProvider
    {
    }
}