using BlazorGraphExample.Services.GraphAPI;
using System;
using System.Collections.Generic;

namespace BlazorGraphExample.Services
{
    public class AppState
    {
        public LoginStatus LoginStatus { get; private set; } = LoginStatus.Undetermined;
        public GraphUser User { get; private set; }
        public IReadOnlyList<DriveItem> DriveItems { get; private set; }

        public event Action LoginStatusChanged;
        public event Action UserChanged;
        public event Action DriveItemsChanged;

        public void SetLoginStatus(LoginStatus loginStatus) =>
            _Set<LoginStatus>(LoginStatus, loginStatus, LoginStatusChanged, val => LoginStatus = val);

        public void SetUser(GraphUser user) =>
            _Set<GraphUser>(User, user, UserChanged, val => User = val);

        public void SetDriveItems(DriveItem[] driveItems) =>
            _Set<DriveItem[]>(DriveItems, driveItems, DriveItemsChanged, val => DriveItems = val);

        private bool _Set<T>(object existing, object updated, Action changeEvent, Action<T> setter)
        {
            if (existing != updated)
            {
                setter((T)updated);
                changeEvent?.Invoke();
                return true;
            }
            return false;
        }
    }
}
