using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TelegramApiProject.Search;
using TelegramApiProject.Send;
using TelegramApiProject.User;
using TLSharp.Core;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace TelegramApiProject.Wpf.Pages
{
    public partial class SendPage : Page
    {
        //todo IDisposable
        private readonly SendService _sendService;
        private readonly MessageServise _messageServise;
        private readonly UserSearchResult _userSearchResult;
        private readonly SendModel _sendModel;
        private readonly BlacklistService _blacklistService;

        public SendPage(UserSearchResult userSearchResult)
        {
            _messageServise = new MessageServise();
            _blacklistService = new BlacklistService(new PathService());
            _sendService = new SendService(_messageServise, new UserService(), _blacklistService);

            InitializeComponent();
            InitTimer();

            _userSearchResult = userSearchResult;
            _sendModel = new SendModel();
        }

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            _sendModel.Message = UserTextToSend.Text;
            _sendModel.Interval = TimePicker.Value.GetValueOrDefault().TimeOfDay;
            _sendModel.IsNameIncluded = CheckBoxName.IsChecked.Value;

            if (!string.IsNullOrEmpty(UserTextToSend.Text) || UserFilesToSend.Items.Count > 0)
            {
                Send();
            }
            else
            {
                MessageBox.Show("Нечего отправлять", MessageBoxConstants.Information, MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }

        private async void Send()
        {
            TelegramClient client = await Client.GetClient();
            int usersCount = _userSearchResult.TlUsers.Count;

            if (_sendModel.Interval != TimeSpan.Zero)
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                string promptMessage = $"Сообщения будут отправлены {usersCount} пользователям";

                if (MakeSureToSendMessages(promptMessage))
                {
                    ClearSendForm();
                    NavidateHome();
                    await _sendService.RunPeriodically(client, _sendModel, _userSearchResult, tokenSource.Token);
                }
            }
            else
            {
                string promptMessage = $"Отправить сейчас сообщение {usersCount} пользователям?";
                if (MakeSureToSendMessages(promptMessage))
                {
                    List<UserModel> users = await _sendService.SendMessage(client, _sendModel, _userSearchResult);
                    if (users.Count > 0)
                    {
                        MessageBox.Show($"Отправлено", MessageBoxConstants.Information,
                            MessageBoxButton.OK,
                            MessageBoxImage.Question);
                        ClearSendForm();
                    }
                    else
                    {
                        MessageBox.Show("Сообщение не отправлено ни одному пользователю", MessageBoxConstants.Information,
                            MessageBoxButton.OK,
                            MessageBoxImage.Question);
                    }
                    ClearSendForm();
                    NavidateHome();
                }
            }
        }

        private bool MakeSureToSendMessages(string promptMessage)
        {
            var dialogResult = MessageBox.Show(promptMessage, MessageBoxConstants.Information, MessageBoxButton.OKCancel, 
                MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.OK)
            {
                return true;
            }

            return false;
        }

        private void NavidateHome()
        {
            HomePage home = new HomePage();
            NavigationService.Navigate(home);
        }

        private void ButtonLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            //opens dialog for load files
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    var file = Path.GetFileName(filename);
                    UserFilesToSend.Items.Add(file);
                    DetermineFileType(filename);
                }
            }
        }

        private void DetermineFileType(string filename)
        {
            _sendModel.Photos = _sendModel.Photos ?? new List<string>();
            _sendModel.Documents = _sendModel.Documents ?? new List<string>();

            if (filename.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase) ||
                filename.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase) ||
                filename.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                filename.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase))
            {
                _sendModel.Photos.Add(filename);
            }
            else
            {
                _sendModel.Documents.Add(filename);
            }
        }

        private void ClearSendForm()
        {
            UserFilesToSend?.Items.Clear();
            UserTextToSend.Text = string.Empty;
        }

        private void InitTimer()
        {
            TimePicker.Format = DateTimeFormat.Custom;
            TimePicker.FormatString = "HH'ч 'mm'м 'ss'с'";
            TimePicker.Value = DateTime.Today;
        }
    }
}
