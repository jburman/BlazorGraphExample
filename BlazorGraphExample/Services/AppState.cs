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
        public string Path { get; private set; }
        public bool InProgress { get; private set; }

        public event Action LoginStatusChanged;
        public event Action UserChanged;
        public event Action DriveItemsChanged;
        public event Action PathChanged;
        public event Action InProgressChanged;

        public event Action<int> LoadProgressChanged;

        public void SetLoginStatus(LoginStatus loginStatus) =>
            _Set<LoginStatus>(LoginStatus, loginStatus, LoginStatusChanged, val => LoginStatus = val);

        public void SetUser(GraphUser user) =>
            _Set<GraphUser>(User, user, UserChanged, val => User = val);

        public void SetDriveItems(List<DriveItem> driveItems) =>
            _Set<List<DriveItem>>(DriveItems, driveItems, DriveItemsChanged, val => DriveItems = val);

        public void SetInProgress(bool inProgress) =>
            _Set<bool>(InProgress, inProgress, InProgressChanged, val => InProgress = val);

        public void SetPath(string path) =>
            _Set<string>(Path, path, PathChanged, val => Path = val);

        public void FireLoadProgressChanged(int count) =>
            LoadProgressChanged?.Invoke(count);

        public void PushFolder(string folder)
        {
            string newPath = Path;

            if (string.IsNullOrEmpty(folder) || folder == "/")
                newPath = string.Empty;
            else
                newPath += "/" + folder;

            SetPath(newPath);
        }

        public void PopFolder()
        {
            if(!string.IsNullOrEmpty(Path) && Path != "/")
            {
                int index = Path.LastIndexOf('/');
                if (index == 0)
                    SetPath(string.Empty);
                if (index != -1)
                    SetPath(Path.Substring(0, index));
            }
        }

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
