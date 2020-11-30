using AE.Net.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Html;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Review : Page
    {
        static MailMessage mail;
        public Review()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            mail = (MailMessage)e.Parameter;
            editor.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, HtmlUtilities.ConvertToText(mail.Body));
            SenderTextBox.Text = mail.From.ToString();
            SubjectTextBox.Text = mail.Subject;

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Frame as Frame;
            frame.Navigate(typeof(MailPage));
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Frame as Frame;
            frame.Navigate(typeof(EditPage), mail.From.Address.ToString());
        }
    }
}
