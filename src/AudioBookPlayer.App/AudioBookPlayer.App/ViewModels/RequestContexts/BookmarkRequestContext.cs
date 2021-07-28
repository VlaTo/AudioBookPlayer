﻿using AudioBookPlayer.App.Domain;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.ViewModels.RequestContexts
{
    public sealed class BookmarkRequestContext : InteractionRequestContext
    {
        public AudioBookPosition Bookmark
        {
            get;
        }

        public BookmarkRequestContext(AudioBookPosition bookmark)
        {
            Bookmark = bookmark;
        }
    }
}