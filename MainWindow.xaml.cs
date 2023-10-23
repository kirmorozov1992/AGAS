using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
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

namespace AGAS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Путь к файлу со списком товаров
        string path_goods = "goods.txt";

        // Переменная с текущей датой
        DateTime dateTime = DateTime.Today;

        // Метод добавления информации о товаре в список товаров
        private async void Button_add_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(path_goods, true))
            {
                if (
                        TextBox_add_code.Text == "" || TextBox_add_price.Text == ""
                        || TextBox_keep_place.Text == "" || TextBox_add_date.Text == ""
                        || TextBox_add_date.Text.Length < 10
                        || TextBox_add_date.Text.Length > 10
                   )
                {
                    MessageBox.Show("ЗАПОЛНИТЕ ВСЕ ПОЛЯ");
                }
                else
                {
                    await writer.WriteLineAsync(
                        TextBox_add_code.Text.Trim() + " "
                        + TextBox_add_price.Text.Trim() + " "
                        + TextBox_keep_place.Text.Trim() + " "
                        + TextBox_add_date.Text.Trim()
                    );

                    TextBox_add_code.Clear();
                    TextBox_add_price.Clear();
                    TextBox_keep_place.Clear();
                    TextBox_add_date.Clear();
                    MessageBox.Show("ДОБАВЛЕНО");
                }


            }
        }


        // Метод, выводящий список товаров со СГ, истекающим через n-дней
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string ld_text = TextBox_last_days.Text.Replace(" ", "");
            if (ld_text != "")
            {
                // Список, содержащий список товаров
                List<string> goods = new List<string>();

                using (StreamReader reader = new StreamReader(path_goods))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        goods.Add(line);
                    }
                }

                string text = "";
                if (goods.Count != 0)
                {
                    for (int i = 0; i < goods.Count; i++)
                    {
                        string[] goods_data = goods[i].Split(' ');
                        if (DateTime.Parse(goods_data[3]) < dateTime)
                        {
                            continue;
                        }
                        else if (dateTime.AddDays(int.Parse(ld_text)) == DateTime.Parse(goods_data[3]))
                        {
                            text += goods[i] + "\n";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
                }


                if (text == "")
                {
                    MessageBox.Show("НЕТ ТОВАРА");
                }
                else
                {
                    TextBox_info.Text = text.Trim();
                    //Button_show_fresh.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
            }
            
        }

        // Метод для очистки текстовых полей
        private void Button_clear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_last_days.Clear();
            TextBox_info.Clear();
            TextBox_discount.Clear();
            TextBox_pd.Clear();
            TextBox_gcode.Clear();
        }

        // Метод добавления скидки для отсортированного товара по сроку годности
        private async void Button_set_discount_Click(object sender, RoutedEventArgs e)
        {
            string dc_text = TextBox_discount.Text.Replace(" ", "");
            if (dc_text != "")
            {
                // Список, содержащий список товаров
                List<string> goods = new List<string>();
                List<string> new_list = new List<string>();

                using (StreamReader reader = new StreamReader(path_goods))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        goods.Add(line);
                    }
                }

                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    if (dateTime.AddDays(int.Parse(TextBox_last_days.Text)) == DateTime.Parse(goods_data[3]) && goods_data.Count() == 4)
                    {
                        double actual_price = int.Parse(goods_data[1]);
                        int discount = int.Parse(dc_text);
                        double new_price = actual_price - ((actual_price / 100) * discount);
                        List<string> new_goods_data = goods_data.ToList();
                        new_goods_data.Add(new_price.ToString());
                        string goods_line = String.Join(" ", new_goods_data);
                        new_list.Add(goods_line);
                    }
                    else
                    {
                        new_list.Add(goods[i].ToString());
                    }
                }
                string new_list_2 = String.Join("\n", new_list);

                using (StreamWriter writer = new StreamWriter(path_goods, false))
                {
                    await writer.WriteLineAsync(new_list_2);
                    MessageBox.Show("ГОТОВО");
                }
            }
            else
            {
                MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
            }
            
        }

        // Метод удаления всего списка товаров
        private void Button_delete_all_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(path_goods);
            File.Create(path_goods);
            MessageBox.Show("ГОТОВО");
        }

        // Метод, выводящий список товаров с истекшим сроком годности на определенную дату
        private async void Button_pd_show_Click(object sender, RoutedEventArgs e)
        {
            string pd_text = TextBox_pd.Text.Replace(" ", "");
            if (pd_text != "")
            {
                // Список, содержащий список товаров
                List<string> goods = new List<string>();

                using (StreamReader reader = new StreamReader(path_goods))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        goods.Add(line);
                    }
                }

                string text = "";
                if (goods.Count != 0 && DateTime.Parse(pd_text) < dateTime)
                {
                    for (int i = 0; i < goods.Count; i++)
                    {
                        string[] goods_data = goods[i].Split(' ');
                        if (DateTime.Parse(goods_data[3]) == DateTime.Parse(pd_text))
                        {
                            text += goods[i] + "\n";
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else if (DateTime.Parse(pd_text) > dateTime)
                {
                    MessageBox.Show("ТОВАР НЕ ПРОСРОЧЕН");
                }
                else
                {
                    MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
                }


                if (text == "")
                {
                    MessageBox.Show("НЕТ ТОВАРА");
                }
                else
                {
                    TextBox_info.Text = text.Trim();
                }
            }
            else
            {
                MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
            }
        }

        // Метод для удаления товаров с истекшим сроком годности на определенную дату
        private async void Button_pd_sort_delete_Click(object sender, RoutedEventArgs e)
        {
            string pd_text = TextBox_pd.Text.Replace(" ", "");

            if (pd_text != "")
            {
                // Список, содержащий список товаров
                List<string> goods = new List<string>();

                using (StreamReader reader = new StreamReader(path_goods))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        goods.Add(line);
                    }
                }

                string text = "";
                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    if (DateTime.Parse(pd_text) != DateTime.Parse(goods_data[3]))
                    {
                        text += goods[i].ToString() + "\n";
                    }
                    else
                    {
                        continue;
                    }
                }

                using (StreamWriter writer = new StreamWriter(path_goods, false))
                {
                    await writer.WriteLineAsync(text.Trim());
                    MessageBox.Show("ГОТОВО");
                }
            }
            else
            {
                MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
            }
        }

        // Метод для поиска товара с определенным кодом в списке товаров 
        private async void Button_search_Click(object sender, RoutedEventArgs e)
        {
            string gcode_text = TextBox_gcode.Text.Replace(" ", "");
            
            if (gcode_text != "")
            {
                List<string> goods = new List<string>();

                using (StreamReader reader = new StreamReader(path_goods))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        goods.Add(line);
                    }
                }

                string text = "";
                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    if (goods_data[0].ToString() == gcode_text)
                    {
                        text += goods[i].ToString() + "\n";
                    }
                    else
                    {
                        continue;
                    }
                }

                if (text == "")
                {
                    MessageBox.Show("НЕТ ТОВАРА");
                }
                else
                {
                    TextBox_info.Text = text.Trim();
                }
            }
            else
            {
                MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
            }
        }

        // Метод для удаления товара с определенным кодом из списка товаров
        private async void Button_del_one_Click(object sender, RoutedEventArgs e)
        {
            string gcode_text = TextBox_gcode.Text.Replace(" ", "");

            if (gcode_text != "")
            {
                List<string> goods = new List<string>();

                using (StreamReader reader = new StreamReader(path_goods))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        goods.Add(line);
                    }
                }

                string text = "";
                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    if (goods_data[0].ToString() != gcode_text)
                    {
                        text += goods[i].ToString() + "\n";
                    }
                    else
                    {
                        continue;
                    }
                }

                using (StreamWriter writer = new StreamWriter(path_goods, false))
                {
                    await writer.WriteLineAsync(text.Trim());
                    MessageBox.Show("ГОТОВО");
                }
            }
            else
            {
                MessageBox.Show("ВВЕДИТЕ ДАННЫЕ");
            }
        }

        // Метод для вывода общей стоимости товаров с учетом скидки
        private async void Button_show_all_dc_Click(object sender, RoutedEventArgs e)
        {
            // Список, содержащий список товаров
            List<string> goods = new List<string>();

            using (StreamReader reader = new StreamReader(path_goods))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    goods.Add(line);
                }
            }

            // Переменная для общей стоимости товаров со скидкой
            double sum_1 = 0;
            // Переменная для общей стоимости товаров без скидки
            double sum_2 = 0;

            if (goods.Count != 0)
            {
                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    if (goods_data.Length != 5)
                    {
                        sum_2 += double.Parse(goods_data[1]);
                    }
                    else
                    {
                        sum_1 += double.Parse(goods_data[4]);
                    }
                }
            }
            else
            {
                MessageBox.Show("СПИСОК ТОВАРОВ ПУСТ");
            }


            if (sum_1 + sum_2 == 0)
            {
                MessageBox.Show("НЕТ ТОВАРА");
            }
            else
            {

                TextBox_info.Text = (sum_1 + sum_2).ToString();
            }
        }

        // Метод для вывода общей стоимости товаров без учета скидки
        private async void Button_show_all_wdc_Click(object sender, RoutedEventArgs e)
        {
            // Список, содержащий список товаров
            List<string> goods = new List<string>();

            using (StreamReader reader = new StreamReader(path_goods))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    goods.Add(line);
                }
            }

            // Переменная для ОСТ с УС
            double sum = 0;

            if (goods.Count != 0)
            {
                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    sum += double.Parse(goods_data[1]);
                }
            }
            else
            {
                MessageBox.Show("СПИСОК ТОВАРОВ ПУСТ");
            }


            if (sum == 0)
            {
                MessageBox.Show("НЕТ ТОВАРА");
            }
            else
            {

                TextBox_info.Text = sum.ToString();
            }
        }

        // Метод для вывода разницы между общей стоимостью и общей стоимостью с учетом скидки
        private async void Button_diff_Click(object sender, RoutedEventArgs e)
        {
            // Список, содержащий список товаров
            List<string> goods = new List<string>();

            using (StreamReader reader = new StreamReader(path_goods))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    goods.Add(line);
                }
            }

            // Переменная для ОСТ без УС
            double sum = 0;
            // Переменная для общей стоимости товаров со скидкой
            double sum_1 = 0;
            // Переменная для общей стоимости товаров без скидки
            double sum_2 = 0;

            if (goods.Count != 0)
            {
                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    sum += double.Parse(goods_data[1]);
                }

                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    if (goods_data.Length != 5)
                    {
                        sum_2 += double.Parse(goods_data[1]);
                    }
                    else
                    {
                        sum_1 += double.Parse(goods_data[4]);
                    }
                }
            }
            else
            {
                MessageBox.Show("СПИСОК ТОВАРОВ ПУСТ");
            }


            if (sum == 0)
            {
                MessageBox.Show("НЕТ ТОВАРА");
            }
            else
            {
                // Переменная для вывода разницы
                double diff = sum - (sum_1 + sum_2);
                TextBox_info.Text = diff.ToString();
            }
        }

        // Метод для вывода общей стоимости товаров с истёкшим сроком годности
        private async void Button_show_all_pd_Click(object sender, RoutedEventArgs e)
        {
            // Список, содержащий список товаров
            List<string> goods = new List<string>();

            using (StreamReader reader = new StreamReader(path_goods))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    goods.Add(line);
                }
            }

            // Переменная для ОСТ c УС
            double sum_dc = 0;
            // Переменная для ОСТ без УС
            double sum_wdc = 0;

            if (goods.Count != 0)
            {
                for (int i = 0; i < goods.Count; i++)
                {
                    string[] goods_data = goods[i].Split(' ');
                    
                    if (DateTime.Parse(goods_data[3]) < dateTime)
                    {
                        if (goods_data.Length != 5)
                        {
                            sum_wdc += double.Parse(goods_data[1]);
                        }
                        else
                        {
                            sum_dc += double.Parse(goods_data[4]);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                MessageBox.Show("СПИСОК ТОВАРОВ ПУСТ");
            }


            if (sum_dc + sum_wdc == 0)
            {
                MessageBox.Show("НЕТ ТОВАРА");
            }
            else
            {
                TextBox_info.Text = (sum_dc + sum_wdc).ToString();
            }
        }

        // Метод для удаления товаров с истёкшим сроком годности
        private async void Button_del_all_pd_Click(object sender, RoutedEventArgs e)
        {
            // Список, содержащий список товаров
            List<string> goods = new List<string>();

            using (StreamReader reader = new StreamReader(path_goods))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    goods.Add(line);
                }
            }

            string text = "";
            for (int i = 0; i < goods.Count; i++)
            {
                string[] goods_data = goods[i].Split(' ');
                if (DateTime.Parse(goods_data[3]) >= dateTime)
                {
                    text += goods[i].ToString() + "\n";
                }
                else
                {
                    continue;
                }
            }

            if (text == "")
            {
                File.Delete(path_goods);
                File.Create(path_goods);
                MessageBox.Show("ГОТОВО");
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(path_goods, false))
                {
                    await writer.WriteLineAsync(text.Trim());
                    MessageBox.Show("ГОТОВО");
                }
            }
        }
        //---------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ КОДА ТОВАРА
        // Метод не позволяет вводить другие символы, кроме цифр
        private void TextBox_add_code_val(object sender, TextCompositionEventArgs e)
        {
            if (!Int32.TryParse(e.Text, out int val))
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        // Метод не позволяет вводить пробел
        private void TextBox_add_code_val1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        //--------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ ЦЕНЫ ТОВАРА
        // Метод не позволяет вводить другие символы, кроме цифр
        private void TextBox_add_price_val(object sender, TextCompositionEventArgs e)
        {
            if (!Int32.TryParse(e.Text, out int val))
            {
                e.Handled = true; // отклоняем ввод
            }
        }

        // Метод не позволяет вводить пробел
        private void TextBox_add_price_val1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        //--------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ МЕСТА ХРАНЕНИЯ
        // Метод не позволяет вводить пробел
        private void TextBox_keep_place_val(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        //--------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ ГОДЕН ДО
        // Метод не позволяет вводить другие символы, кроме цифр и точки
        private void TextBox_add_date_val(object sender, TextCompositionEventArgs e)
        {
            if (!Int32.TryParse(e.Text, out int val) && e.Text != ".")
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        // Метод не позволяет вводить пробел
        private void TextBox_add_date_val1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }
        //--------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ ИСТЕКАЕТ ЧЕРЕЗ
        // Метод не позволяет вводить другие символы, кроме цифр
        private void TextBox_last_days_val(object sender, TextCompositionEventArgs e)
        {
            if (!Int32.TryParse(e.Text, out int val))
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        // Метод не позволяет вводить пробел
        private void TextBox_last_days_val1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        //--------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ СКИДКИ
        // Метод не позволяет вводить другие символы, кроме цифр
        private void TextBox_discount_val(object sender, TextCompositionEventArgs e)
        {
            if (!Int32.TryParse(e.Text, out int val))
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        // Метод не позволяет вводить пробел
        private void TextBox_discount_val1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        //--------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ СРОК ИСТЁК
        // Метод не позволяет вводить другие символы, кроме цифр и точки
        private void TextBox_pd_val(object sender, TextCompositionEventArgs e)
        {
            if (!Int32.TryParse(e.Text, out int val) && e.Text != ".")
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        // Метод не позволяет вводить пробел
        private void TextBox_pd_val1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        //--------- ОГРАНИЧЕНИЕ ВВОДА ДЛЯ КОД ТОВАРА (ПОИСК)
        // Метод не позволяет вводить другие символы, кроме цифр
        private void TextBox_gcode_val(object sender, TextCompositionEventArgs e)
        {
            if (!Int32.TryParse(e.Text, out int val))
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        // Метод не позволяет вводить пробел
        private void TextBox_gcode_val1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }
    }
}
