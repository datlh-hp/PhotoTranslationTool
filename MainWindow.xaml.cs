using GTranslate.Translators;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Tesseract;

namespace PhotoTranslationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private const string _tessdataLanguage = "eng+kor";
        public MainWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

            GlobalHotkey.Register(handle);
        }

        private async void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PasteFromClipboard();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("!!!ERROR!!!", ex.Message);
            }
        }

        private async void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.V &&
                (Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
                {
                    PasteFromClipboard();
                }
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    Translate();
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("!!!ERROR!!!", ex.Message);
            }
        }

        private async void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog { Filter = "Image Files|*.jpg;*.png;*.bmp" };
                if (dlg.ShowDialog() == true)
                {
                    SelectedImage.Source = new BitmapImage(new Uri(dlg.FileName));
                    string text = RunOcr(dlg.FileName);
                    OcrTextBox.Text = text;

                    // TODO: gọi API dịch
                    string translated = await TranslateTextAsync(text);
                    TranslatedTextBox.Text = translated;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("!!!ERROR!!!", ex.Message);
            }
        }

        private async void SelectedImage_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0)
                    {
                        string path = files[0];
                        SelectedImage.Source = new BitmapImage(new Uri(path));

                        string text = RunOcr(path);
                        OcrTextBox.Text = text;

                        // TODO: gọi dịch
                        string translated = await TranslateTextAsync(text);
                        TranslatedTextBox.Text = translated;
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("!!!ERROR!!!", ex.Message);
            }
        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            Translate();
        }

        private async void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OcrTextBox != null && !string.IsNullOrWhiteSpace(OcrTextBox.Text))
                {
                    TranslatedTextBox.Text = await TranslateTextAsync(OcrTextBox.Text);
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("!!!ERROR!!!", ex.Message);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // hủy việc đóng

            this.Hide();     // chỉ ẩn cửa sổ
        }

        #region Private Methods

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (GlobalHotkey.HandleHotkey(hwnd, msg, wParam, lParam))
            {

                this.Dispatcher.Invoke(() =>
                {
                    bool isVisible = this.IsVisible;
                    if (!isVisible)
                    {
                        // Nếu cửa sổ đang ẩn, hiển thị nó
                        this.Show();
                        this.WindowState = WindowState.Normal;
                        this.Activate();

                        PasteFromClipboard(false);
                    }
                    else
                    {
                        // Nếu cửa sổ đang hiện, ẩn nó
                        this.Hide();
                    }

                });

                handled = true;
            }
            return IntPtr.Zero;
        }

        private async void Translate()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OcrTextBox.Text))
                {
                    await this.ShowMessageAsync("Thông báo", "Chưa có văn bản để dịch!");
                    return;
                }

                string translated = await TranslateTextAsync(OcrTextBox.Text);
                TranslatedTextBox.Text = translated;
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("!!!ERROR!!!", ex.Message);
            }
        }

        private async void PasteFromClipboard(bool showMessage = true)
        {
            if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();
                SelectedImage.Source = img;

                // OCR từ ảnh
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                using var ms = new MemoryStream();
                encoder.Save(ms);

                string text = RunOcr(ms.ToArray());
                OcrTextBox.Text = text;

                // TODO: dịch text
                string translated = await TranslateTextAsync(text);
                TranslatedTextBox.Text = translated;
            }
            else if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                OcrTextBox.Text = text;

                // TODO: dịch text
                string translated = await TranslateTextAsync(text);
                TranslatedTextBox.Text = translated;
            }
            else
            {
                if (showMessage)
                {
                    await this.ShowMessageAsync("Thông báo", "Clipboard không có ảnh hoặc text!");
                }
            }
        }

        public async Task<string> TranslateTextAsync(string text)
        {
            string sourceText = OcrTextBox.Text;
            if (!string.IsNullOrWhiteSpace(sourceText))
            {
                var translator = new GoogleTranslator();
                var selectedItem = (ComboBoxItem)LangComboBox.SelectedItem;
                string targetLang = selectedItem.Tag.ToString();

                var result = await translator.TranslateAsync(text, targetLang);

                return result.Translation;
            }

            return string.Empty;
        }

        private string RunOcr(string imagePath)
        {
            string tessDataPath = System.IO.Path.Combine(AppContext.BaseDirectory, "tessdata");
            using var engine = new TesseractEngine(tessDataPath, _tessdataLanguage, EngineMode.Default);
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            return page.GetText();
        }

        private string RunOcr(byte[] imageBytes)
        {
            string tessDataPath = System.IO.Path.Combine(AppContext.BaseDirectory, "tessdata");
            using var engine = new Tesseract.TesseractEngine(tessDataPath, _tessdataLanguage, Tesseract.EngineMode.Default);
            using var img = Tesseract.Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(img);
            return page.GetText();
        }

        #endregion Private Methods


    }
}