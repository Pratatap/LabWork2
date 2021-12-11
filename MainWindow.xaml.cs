using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace LabWork2._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        static int mode = 1; //mode for button ChangedThreats/AllThreats
        public MainWindow()
        {

            InitializeComponent();
            CheckOnStart();
            StartUp();
            //ExcelReader.OpenExcelTable("");
            // ExcelReader.ReadAllLines(ExcelReader.TableContentSet, ref ExcelReader.threat);
            //DataSet threatTable = ExcelReader.TableContentSet;
            //ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent();
            // ExcelReader.TakeHeaders(ExcelReader.TableContentSet, ref ExcelReader.headers);
            //ExcelReader.fullThreatInfo = ExcelReader.GetFullContent();
            //DescriptionGrid.ItemsSource = fullThreats[10].list;
            //TempButton.Text = fullThreats[6].list.Count.ToString();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void ThreatDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update_DescriptionGrid((DataGrid)sender);
        }
        private void Update_DescriptionGrid(DataGrid sender)
        {
            if (sender.SelectedItem != null)
            {

                DescriptionGrid.ItemsSource = null;
                Threat.ShortThreatInfo si = (Threat.ShortThreatInfo)ThreatDataGrid.SelectedItem;
                int index = int.Parse(si.ID.Substring(4));
                if (mode == 1)
                {
                    DescriptionGrid.ItemsSource = GetFullContentById(index)?.list;
                }
                else
                {
                    DescriptionGrid.ItemsSource = GetChFullContentById(index)?.list;
                }
            }
        }
        public static Threat.FullThreatInfo GetFullContentById(int id)
        {
            foreach (var item in ExcelReader.fullThreatInfo)
            {
                if (item.list[0].Item2 == id.ToString())
                {
                    return item;
                }
            }
            return null;
        }
        public static Threat.FullThreatInfo GetChFullContentById(int id)
        {
            foreach (var item in ExcelReader.chfullThreatInfo)
            {
                if (item.list[0].Item2 == id.ToString())
                {
                    return item;
                }
            }
            return null;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void StartUp()
        {
            ExcelReader.OpenExcelTable("");
            ExcelReader.ReadAllLines(ExcelReader.TableContentSet, ref ExcelReader.threat);
            ExcelReader.TakeHeaders(ExcelReader.TableContentSet, ref ExcelReader.headers);
            ExcelReader.GetFullContent();
            Paginator.Paginate(ExcelReader.threat);
            ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(Paginator.pagedThreatList[0]);
            UpdatePageButtons();
            //ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(ExcelReader.threat);
        }
        private void GetNewTable()
        {
            ExcelReader.OpenExcelTable("");
            ExcelReader.chfullThreatInfo = null;
            ExcelReader.chthreat = null;
            ExcelReader.newthreat = null;
            ExcelReader.ReadAllLines(ExcelReader.TableContentSet, ref ExcelReader.newthreat);
            ExcelReader.chthreat = ExcelReader.GetChangedThreats(ExcelReader.newthreat, ExcelReader.threat);
            ExcelReader.GetChFullContent();
            ExcelReader.threat = ExcelReader.newthreat;
            UpdatePageButtons();
            SaveMode();
            //ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(ExcelReader.threat);
        }
        private static void CheckOnStart()
        {
            ExcelReader.CheckOnStart();
        }
        private void DownloadFromWeb(object sender, RoutedEventArgs e)
        {
            if (ExcelReader.CheckInternet())
            {
                ExcelReader.LoadFromWeb();
                MessageBox.Show("Скачивание успешно!", "Initialization");
                GetNewTable();
            }
            else
            {
                MessageBox.Show("Нет подключения к интернету ;c", "Error");
            }

        }
        public static void MessageBoxOnStart()
        {
            MessageBox.Show("Таблицы в локальной директории не найдено... скачиваем из интернета", "Initialization");

        }
        public static void MessageBoxFileFound()
        {
            MessageBox.Show("Найдена таблица в локальной директории, используем её!", "Initialization");
        }
        public void UpdatePageButtons()
        {
            PrevButton.IsEnabled = Paginator.page == 0 ? false : true;
            NextButton.IsEnabled = Paginator.page == Paginator.pagedThreatList.Count-1 ? false : true;
        }

        private void LoadFromLocal(object sender, RoutedEventArgs e)
        {
            string path = FileDialogOpenShow();
            if (path == null)
            {
                MessageBox.Show("Ошибка, путь выбран неверно", "Error");
            }
            else if (!(path.Contains(".xlsx") || path.Contains("*.xls")))
            {
                MessageBox.Show("Файл должен быть типа .xlsx или .xls", "Error");
            }
            else
            {
                ExcelReader.LoadLocal(path);
                GetNewTable();
            }

        }
        private string FileDialogOpenShow()
        {
            Microsoft.Win32.OpenFileDialog dialogue = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Document",
                DefaultExt = ".xlsx",
                Filter = "Excel files|*.xls;*.xlsx;|All|*.*"
            };
            var result = dialogue.ShowDialog();
            if (result.HasValue && result.Value)
            {
                return dialogue.FileName;
            }
            return null;
        }
        private void SaveMode()
        {
            if (mode == 2)
            {
                ThreatDataGrid.IsEnabled = true;
                try
                {
                    Paginator.Paginate(ExcelReader.chthreat);
                    UpdatePageButtons();
                    ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(Paginator.pagedThreatList[Paginator.page]);
                }
                catch (Exception)
                {
                    ThreatDataGrid.IsEnabled = false;
                    PrevButton.IsEnabled = false;
                    NextButton.IsEnabled = false;
                }

            }
            else if (mode == 1)
            {
                ThreatDataGrid.IsEnabled = true;
                Changer.Content = "Show changed threats";
                try
                {
                    Paginator.Paginate(ExcelReader.threat);
                    UpdatePageButtons();
                    ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(Paginator.pagedThreatList[Paginator.page]);

                }
                catch (Exception)
                {
                    ThreatDataGrid.IsEnabled = false;
                }

            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mode == 1)
            {
                ThreatDataGrid.IsEnabled = true;
                Changer.Content = "Show all threats";
                mode = 2;
                try
                {
                    Paginator.Paginate(ExcelReader.chthreat);
                    UpdatePageButtons();
                    ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(Paginator.pagedThreatList[Paginator.page]);
                }
                catch (Exception)
                {
                    ThreatDataGrid.IsEnabled = false;
                    PrevButton.IsEnabled = false;
                    NextButton.IsEnabled = false;
                }

            }
            else if (mode == 2)
            {
                ThreatDataGrid.IsEnabled = true;
                Changer.Content = "Show changed threats";
                mode = 1;
                try
                {
                    Paginator.Paginate(ExcelReader.threat);
                    UpdatePageButtons();
                    ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(Paginator.pagedThreatList[Paginator.page]);

                }
                catch (Exception)
                {
                    ThreatDataGrid.IsEnabled = false;
                }

            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialogue = new Microsoft.Win32.SaveFileDialog();
            dialogue.FileName = "newThreatList";
            dialogue.DefaultExt = ".xlsx";
            dialogue.Filter = "Excel files|*.xlsx;*.xls;|All|*.*";
            var result = dialogue.ShowDialog();
            string path;
            if (result.HasValue && result.Value)
            {
                path = dialogue.FileName;
            }
            else
            {
                return;
            }
            if (ExcelReader.SaveAs(path))
            {
                MessageBox.Show("Успешно сохранено!", "Saving");
            }
            else
            {
                MessageBox.Show("Так не надо делать", "Saving");
            }


        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (Paginator.Prev())
            {
                ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(Paginator.pagedThreatList[Paginator.page]);
            }
            UpdatePageButtons();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Paginator.Next())
            {
                ThreatDataGrid.ItemsSource = ExcelReader.GetShortContent(Paginator.pagedThreatList[Paginator.page]);
            }
            UpdatePageButtons();
        }

        //PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(ThreatDataGrid)));

    }
}
