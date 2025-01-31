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
    /// Логика взаимодействия для AdsPage.xaml
    /// </summary>
    public partial class AdsPage : Page
    {
        int selectId;
        public AdsPage(int id)
        {
            InitializeComponent();
            SortComboBox.SelectedIndex = 0;

            var currentAds = Entities.GetContext().Ads.ToList();

            DataGridAds.ItemsSource = currentAds;
            selectId = id;
            UpdateAds();
        }
        private void UpdateAds()
        {
            //загружаем всех пользователей в список
            var currentPosts = Entities.GetContext().Ads.ToList();
            //осуществляем поиск по названию поста без учета регистра букв
            currentPosts = currentPosts.Where(x => x.Title.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
            if (RoleCheckBox.IsChecked.Value)
            { currentPosts = currentPosts.Where(x => (int)x.UserID == selectId).ToList(); }
            else { currentPosts = currentPosts.Where(x => x.Title.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList(); }
            //осуществляем сортировку в зависимости от выбора пользователя
            if (SortComboBox.SelectedIndex == 0)
                DataGridAds.ItemsSource = currentPosts.OrderBy(x => x.Title).ToList();
            else
                DataGridAds.ItemsSource = currentPosts.OrderByDescending(x => x.Title).ToList();
            var profit = Entities.GetContext().User
                        .Where(x => (int)x.UserID == selectId)
                        .Select(x => (int)x.TotalProfit)
                        .FirstOrDefault();
            prof.Text = profit.ToString();



        }

        private void RoleCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            DelButton.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Visible;
            UpdateAds();
        }
        private void RoleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DelButton.Visibility = Visibility.Hidden;
            AddButton.Visibility = Visibility.Hidden;
            UpdateAds();
        }


        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }




        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            SortComboBox.SelectedIndex = 0;
            RoleCheckBox.IsChecked = false;
            UpdateAds();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdsAddPage(null, selectId, true));
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            var postsForRemoving = DataGridAds.SelectedItems.Cast<Ads>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {postsForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                try
                {
                    Entities.GetContext().Ads.RemoveRange(postsForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены");

                    UpdateAds();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridAds.SelectedItem != null && DataGridAds.SelectedItem is Ads selectedAds)
            {
                if (RoleCheckBox.IsChecked.Value)
                {
                    NavigationService?.Navigate(new AdsAddPage(selectedAds, selectId, false));
                }
                else NavigationService?.Navigate(new AdsAddPage(selectedAds, -1, false));
                
            }
        }

        private void DataGridAds_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateAds();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAds();

        }
    }
}
