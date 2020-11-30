using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Maqil
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class EditPage : Page
    {
        public List<UserData> user;
        public int id;
        public XmlSerializer xml2 = new XmlSerializer(typeof(List<UserData>));
        public XmlSerializer xml = new XmlSerializer(typeof(int));
        public static string str;
        public StorageFolder folder = ApplicationData.Current.LocalFolder;
        public EditPage()
        {
            this.InitializeComponent();
            editor.Document.Selection.CharacterFormat.ForegroundColor = Colors.Black;

        }
        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            editor.Focus(FocusState.Pointer);
            editor.Document.Selection.SetRange(0, editor.Document.Selection.EndPosition);
            editor.Document.Selection.SetText(TextSetOptions.None, string.Empty);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string textStr = "";
            editor.Document.GetText(TextGetOptions.None, out textStr);

            if (textStr != "" && SenderTextBox.Text != "")
            {
                //----------------------------------------------------------------------------------------------Десериализация юзера 
                var file = await folder.CreateFileAsync("UserData.xml", CreationCollisionOption.OpenIfExists);
                str = await FileIO.ReadTextAsync(file);

                var stream = await file.OpenStreamForReadAsync();
                if (str.Length != 0) user = (List<UserData>)xml2.Deserialize(stream);
                else user = new List<UserData>();

                stream.Close();
                //----------------------------------------------------------------------------------------------Десериализация айдишника 

                file = await folder.CreateFileAsync("ID.xml", CreationCollisionOption.OpenIfExists);
                str = await FileIO.ReadTextAsync(file);

                var stream1 = await file.OpenStreamForReadAsync();
                if (str.Length != 0) id = (int)xml.Deserialize(stream1);
                else id = 0;

                stream1.Close();
                //----------------------------------------------------------------------------------------------Почта
                try
                {
                    MailAddress from = new MailAddress(user[id].Login);
                    MailAddress to = new MailAddress(SenderTextBox.Text);
                    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(from, to);
                    if (SubjectTextBox.Text != "")
                        m.Subject = SubjectTextBox.Text;
                    m.Body = textStr;
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(user[id].smtp, 587);
                    smtp.Credentials = new NetworkCredential(user[id].Login.ToString(), user[id].Pass.ToString());
                    smtp.EnableSsl = true;
                    smtp.Send(m);
                    Dialog_Sended();
                }
                catch (Exception ex)
                {
                    Dialog_NotSended(ex.Message);
                }
            }
        }
        private async void Dialog_Sended()
        {
            ContentDialog spravka = new ContentDialog()
            {
                Title = "Письмо отправлено",
                Content = "Ваше письмо успешно отправлено!",
                CloseButtonText = "OK"
            };
            await spravka.ShowAsync();
        }
        private async void Dialog_NotSended(String e)
        {
            ContentDialog spravka = new ContentDialog()
            {
                Title = "Письмо не отправлено",
                Content = $"Произошла ошибка: {e}",
                CloseButtonText = "OK"
            };
            await spravka.ShowAsync();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string address = "";
            if (e.Parameter != null)
            {
                address = (string)e.Parameter;
                SenderTextBox.Text = address;
            }
        }
    }
}
