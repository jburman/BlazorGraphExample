using System;

namespace BlazorGraphExample.Services
{
    public interface IPagingState
    {
        event Action PageCountChanged;
        event Action<(int oldPage, int newPage)> CurrentPageChanged;

        bool HasPages();
        int CurrentPage { get; }
        int PageCount { get; }
        void NextPage();
        void PreviousPage();
    }
}
