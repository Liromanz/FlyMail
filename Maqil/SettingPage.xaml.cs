using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Maqil
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public static List<UserData> user;
        public static int id;
        public static XmlSerializer xml2 = new XmlSerializer(typeof(List<UserData>));
        public static XmlSerializer xml = new XmlSerializer(typeof(int));
        public static string str;
        public static StorageFile file;
        public static StorageFolder folder = ApplicationData.Current.LocalFolder;
        public static FileOpenPicker backgr;
        public SettingPage()
        {
            this.InitializeComponent();
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e) //Выход
        {
            StorageFile fileA = await ApplicationData.Current.LocalFolder.CreateFileAsync("UserData.xml", CreationCollisionOption.ReplaceExisting);
            await fileA.DeleteAsync();
            var frame = this.Frame as Frame;
            var scroll = frame.Parent as ScrollViewer;
            var nav = scroll.Parent as NavigationView;
            var grid = nav.Parent as Grid;
            var main = grid.Parent as MainPage;
            var parent = main.Parent as Frame;
            parent.Navigate(typeof(LoginPage));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //Логин
        {

            var frame = this.Frame as Frame;
            var scroll = frame.Parent as ScrollViewer;
            var nav = scroll.Parent as NavigationView;
            var grid = nav.Parent as Grid;
            var main = grid.Parent as MainPage;
            var parent = main.Parent as Frame;
            string navig = "navigated";
            parent.Navigate(typeof(LoginPage), navig);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //-------------------------------------------------------------------------------------------------Десериализация юзеров
            var file = await folder.CreateFileAsync("UserData.xml", CreationCollisionOption.OpenIfExists);
            str = await FileIO.ReadTextAsync(file);

            var stream = await file.OpenStreamForReadAsync();
            if (str.Length != 0) user = (List<UserData>)xml2.Deserialize(stream);
            else user = new List<UserData>();

            stream.Close();
            //-------------------------------------------------------------------------------------------------Десериализация айди
            file = await folder.CreateFileAsync("ID.xml", CreationCollisionOption.OpenIfExists);
            str = await FileIO.ReadTextAsync(file);

            var stream1 = await file.OpenStreamForReadAsync();
            if (str.Length != 0) id = (int)xml.Deserialize(stream1);
            else id = 0;

            stream1.Close();
            for (int i = 0; i < user.Count; i++)
            {
                usersNumDropDown.Items.Add(user[i].Login);
            }
            usersNumDropDown.SelectedIndex = id;
        }

        private async void usersNumDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            file = await folder.CreateFileAsync("ID.xml", CreationCollisionOption.ReplaceExisting);
            Stream stream = await file.OpenStreamForWriteAsync();
            id = usersNumDropDown.SelectedIndex;
            xml.Serialize(stream, id);
            stream.Close();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e) //Изменить фон
        {
            backgr = new FileOpenPicker();
            backgr.ViewMode = PickerViewMode.Thumbnail;
            backgr.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            backgr.FileTypeFilter.Add(".jpg");
            backgr.FileTypeFilter.Add(".png");

            var backgroundfile = await backgr.PickSingleFileAsync();
            if (backgroundfile != null)
            {
                await backgroundfile.CopyAsync(folder);
                backgroundfile = await folder.CreateFileAsync(backgroundfile.Name, CreationCollisionOption.OpenIfExists);

                var frame = this.Frame as Frame;
                var scroll = frame.Parent as ScrollViewer;
                var nav = scroll.Parent as NavigationView;
                var grid = nav.Parent as Grid;
                grid.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri($@"{folder.Path}\{backgroundfile.Name}"))};

                IReadOnlyList<StorageFile> storageFiles = await folder.GetFilesAsync();
                foreach (StorageFile eeeeeclass in storageFiles)
                {
                    if (eeeeeclass.Name.Contains(".jpg") && eeeeeclass.Name != backgroundfile.Name)
                        await eeeeeclass.DeleteAsync();
                }
            }
        }
    }
}
