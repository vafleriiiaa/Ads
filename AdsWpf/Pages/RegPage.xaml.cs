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
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            TextBoxLogin.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            TextBoxFIO.Text = string.Empty;
            PasswordBoxAgain.Password = string.Empty;
            NavigationService.Navigate(new Authorization());
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password) || string.IsNullOrEmpty(PasswordBoxAgain.Password) || string.IsNullOrEmpty(TextBoxFIO.Text))
            {
                MessageBox.Show("Заполните все вышеуказанные поля!");
                return;
            }
            using (var database = new Entities())
            {
                var user = database.User
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Login == TextBoxLogin.Text);
                if (user != null)
                {
                    MessageBox.Show("Такой логин уже существует");
                    return;
                }
            }
            if (PasswordBox.Password.Length >= 6)
            {
                bool en = true; // английская раскладка
                bool number = false; // цифра

                for (int i = 0; i < PasswordBox.Password.Length; i++) // перебираем символы
                {
                    if ((PasswordBox.Password[i] >= 'А' && PasswordBox.Password[i] <= 'Я') || (PasswordBox.Password[i] >= 'а' && PasswordBox.Password[i] <= 'я')) en = false; // если русская раскладка
                    if (PasswordBox.Password[i] >= '0' && PasswordBox.Password[i] <= '9') number = true; // если цифры
                }

                if (en == false)
                {
                    MessageBox.Show("Доступна только английская раскладка");
                    return;
                } // выводим сообщение

                if (number == false)
                {
                    MessageBox.Show("Добавьте хотя бы одну цифру");
                    return;
                } // выводим сообщение


            }
            else
            {
                MessageBox.Show("пароль слишком короткий, минимум 6 символов");
                return;
            }

            if (PasswordBox.Password != PasswordBoxAgain.Password)
            {
                MessageBox.Show("Пароли не соответствуют");

                return;
            }
            Entities db = new Entities();
            User userObject = new User
            {
                Username = TextBoxFIO.Text,
                Login = TextBoxLogin.Text,
                Password = PasswordBox.Password,
                Id_Role=1,
                TotalProfit = 0
                
            };
            db.User.Add(userObject);
            db.SaveChanges();
            MessageBox.Show("Пользователь Добавлен");


        }
    }
}
