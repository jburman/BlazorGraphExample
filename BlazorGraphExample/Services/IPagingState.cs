using System;

namespace BlazorGraphExample.Services
{
    public interface IPagingState
    {
        event Action PageCountChanged;
        event Action CurrentPageChanged;

        void PushPageToken(int pageNumber, string skipToken);
        (int pageNumber, string skipToken) PopPageToken();
        bool HasPages();
        int CurrentPage { get; }
        int PageCount { get; }
        void NextPage();
        void PreviousPage();
    }
}
