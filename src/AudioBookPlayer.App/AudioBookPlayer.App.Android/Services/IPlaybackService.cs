using System;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Android.Services
{
    internal interface IPlaybackService
    {
        //const string ActionPlay = "com.libraprogramming.audiobookreader.action.play";

        void SetBook(AudioBook audioBook);

        void Play(int chapterIndex, TimeSpan position);
    }
}