using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdsWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserAddPage.xaml
    /// </summary>
    public partial class UserAddPage : Page
    {
        private User _currentUser = new User();
        private bool redact;
        public UserAddPage(User currentUser, bool flag)
        {
            InitializeComponent();
            if (currentUser != null)
            {
                _currentUser = currentUser;
                cmbRole.SelectedIndex = currentUser.Id_Role - 1;
            }
            DataContext = _currentUser;
            var context = Entities.GetContext();
            this.redact = flag;
            cmbRole.ItemsSource = context.Role.Distinct().ToList();
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (redact == false)
            {
                if (string.IsNullOrEmpty(txLogin.Text) || string.IsNullOrEmpty(txPassword.Text) || string.IsNullOrEmpty(txFIO.Text) || string.IsNullOrEmpty(cmbRole.Text))
                {
                    MessageBox.Show("Заполните все вышеуказанные поля!");
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Вы уверены что хотите Добавить эти данные?", "Подтвержение закрытия", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Entities db = new Entities();
                    User questionObject = new User
                    {
                        Login = txLogin.Text,
                        Password = txPassword.Text,
                        Username = txFIO.Text,
                        Id_Role = cmbRole.SelectedIndex + 1,
                        TotalProfit = 0,

                    };
                    db.User.Add(questionObject);
                    db.SaveChanges();
                    MessageBox.Show("Пользователь Добавлен");
                    NavigationService.GoBack();

                }
            }
            else
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrWhiteSpace(_currentUser.Login))
                    errors.AppendLine("Укажите логин!");
                if (string.IsNullOrWhiteSpace(_currentUser.Username))
                    errors.AppendLine("Укажите ФИО!");
                if (string.IsNullOrWhiteSpace(_currentUser.Password))
                    errors.AppendLine("Укажите Пароль!");
                if ((_currentUser.Id_Role == 0) || (cmbRole.Text == ""))
                    errors.AppendLine("Выберите Роль!");
                else
                    _currentUser.Id_Role = cmbRole.SelectedIndex + 1;

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                //Добавляем в объект students новую запись
                if (_currentUser.UserID == 0)
                {
                    Entities.GetContext().User.Add(_currentUser);
                }
                try
                {
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}