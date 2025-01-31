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
    /// Логика взаимодействия для AdsAddPage.xaml
    /// </summary>
    public partial class AdsAddPage : Page
    {
        private Ads _currentAds = new Ads();
        private User _currentUsers = new User();
        private Entities context;
        bool _addAds;
        public AdsAddPage(Ads selectedPost, int isMy, bool addPost)
        {
            InitializeComponent();
            context = new Entities();
            _addAds = addPost;
            if (selectedPost != null) //перенос выбранного поста
            {
                _currentAds = selectedPost;
                if (isMy != -1 && selectedPost.Id_Status != 2) // если пост мой редачим
                {
                    setEdit();
                    SaveBut.Visibility = Visibility.Visible;
                    SoldBut.Visibility = Visibility.Visible;
                }
            }

            if (addPost) //если нужно добавить пост
            {
                _currentAds.Id_Status = 1;
                _currentAds.UserID = isMy;
                boxStatus.Visibility = Visibility.Hidden;
                stat.Visibility = Visibility.Hidden;
                _currentAds.PublishDate = DateTime.Now;
                AddBut.Visibility = Visibility.Visible;
                setEdit();
            }

            DataContext = _currentAds;


        }
        private void setEdit()
        {
            box2.IsReadOnly = false;
            box3.IsReadOnly = false;
            box4.IsReadOnly = false;
            box5.IsReadOnly = false;
            box7.IsReadOnly = false;
            box8.IsReadOnly = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentAds.Category)) errors.AppendLine("Укажите категорию!");
            if (string.IsNullOrWhiteSpace(_currentAds.Type)) errors.AppendLine("Укажите тип товара!");
            if (string.IsNullOrWhiteSpace(_currentAds.Title)) errors.AppendLine("Напишите название поста!");
            if (string.IsNullOrWhiteSpace(_currentAds.City)) errors.AppendLine("Укажите город!");
            if (string.IsNullOrWhiteSpace((_currentAds.Price).ToString())) errors.AppendLine("Укажите цену!");
            if (string.IsNullOrWhiteSpace(_currentAds.Description)) errors.AppendLine("Напишите описание!");

            // Проверяем переменную errors на наличие ошибок
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            // Добавляем в объект Post новую запись
            if (_currentAds.AdID == 0)
                Entities.GetContext().Ads.Add(_currentAds);
            // Делаем попытку записи данных в БД о новом пользователе
            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void SoldBut_Click(object sender, RoutedEventArgs e)
        {
            _currentUsers = Entities.GetContext().User.FirstOrDefault(x => x.UserID == _currentAds.UserID);
            _currentUsers.TotalProfit += _currentAds.Price;

            // .Where(x => x.ID_user == _currentPost.ID_user).

            //закрываем пост
            _currentAds.Id_Status = 2;
            // Добавляем в объект Post новую запись
            if (_currentAds.AdID == 0)
                Entities.GetContext().Ads.Add(_currentAds);

            // Делаем попытку записи данных в БД о новом пользователе
            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Пост закрыт!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }



    }
}
