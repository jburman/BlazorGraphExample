using System.Collections.Generic;

namespace BlazorGraphExample.Components
{
    public class DriveItemListViewModel
    {
        public DriveItemListViewModel(List<DriveItem> driveItems,
            int pageSize,
            int totalItems,
            int currentPage)
        {
            DriveItems = driveItems;
            PageSize = pageSize;
            TotalItems = totalItems;
            CurrentPage = currentPage;
        }

        public IReadOnlyList<DriveItem> DriveItems { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }

        public void NextPage()
        {

        }

        public void PreviousPage()
        {

        }
    }
}
