using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Maqil
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static List<UserData> user;
        public static int id;
        public static XmlSerializer xml2 = new XmlSerializer(typeof(List<UserData>));
        public static XmlSerializer xml = new XmlSerializer(typeof(int));
        public static string str;
        public static StorageFile file;
        public static StorageFolder folder = ApplicationData.Current.LocalFolder;
        public MainPage()
        {
            InitializeComponent();
            
            
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("edit", typeof(EditPage)),
            ("mail", typeof(MailPage)),
            ("settings", typeof(SettingPage))
        };

        private async void Navigation_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigated += On_Navigated;
            Navigation.SelectedItem = Navigation.MenuItems[2];

            string filename = "";
            IReadOnlyList<StorageFile> storageFiles = await folder.GetFilesAsync();
            foreach (StorageFile eeeeeclass in storageFiles)
            {
                if (eeeeeclass.Name.Contains(".jpg"))
                    filename = eeeeeclass.Name;
            }

            if (filename != "")
                gridbg.ImageSource = new BitmapImage(new Uri($@"{folder.Path}\{filename}"));
            else
                gridbg.ImageSource = new BitmapImage(new Uri("ms-appx:///Background/background.jpg"));

        }

        private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true) Navigation_Navigate("settings", "Настройки", args.RecommendedNavigationTransitionInfo);
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                var navName = args.SelectedItemContainer.Content.ToString();
                Navigation_Navigate(navItemTag, navName, args.RecommendedNavigationTransitionInfo);
                Navigation.PaneTitle = args.SelectedItemContainer.Content.ToString();
            }
        }

        private void Navigation_Navigate(string navItemTag, string navName, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            if (!(_page is null) && navItemTag == "mail") ContentFrame.Navigate(_page, navName, transitionInfo);
            else if (!(_page is null) && !Type.Equals(preNavPageType, _page)) ContentFrame.Navigate(_page, null, transitionInfo);
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
            Navigation.SelectedItem = Navigation.MenuItems
                .OfType<NavigationViewItem>();
        }
    }
}
