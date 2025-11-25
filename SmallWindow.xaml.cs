using ControlzEx.Standard;
using GTranslate.Translators;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tesseract;

namespace PhotoTranslationTool
{
    /// <summary>
    /// Interaction logic for SmallWindow.xaml
    /// </summary>
    public partial class SmallWindow : MetroWindow
    {
        private bool _isLocked = false;
        private const string _tessdataLanguage = "eng+kor";
        public string _targetLang { get; set; } = string.Empty;
        public SmallWindow()
        {
            InitializeComponent();
            this.SourceInitialized += (s, e) =>
            {
                var hwndSource = (System.Windows.Interop.HwndSource)PresentationSource.FromVisual(this);
                hwndSource.AddHook(WndProcAsync);

                NativeMethods.AddClipboardFormatListener(hwndSource.Handle);
            };

            this.ShowCloseButton = false;       // ẩn nút đóng
            this.ShowMinButton = false;         // ẩn nút thu nhỏ
            this.ShowMaxRestoreButton = false;  // ẩn nút phóng to/khôi phục
            this.Opacity = 0.7;  
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWin = Application.Current.MainWindow;
            mainWin.Show();   // hiện lại nếu đã ẩn
            mainWin.Activate(); // đưa lên foreground

            this.Hide();
        }

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            _isLocked = !_isLocked;

            if (_isLocked)
            {
                this.ShowTitleBar = false;          
                this.ResizeMode = ResizeMode.NoResize;
                this.Topmost = true;  // luôn ở trên cùng
                //MessageBox.Show("App đã bị khóa cố định.");
            }
            else
            {
                this.ShowTitleBar = true;
                this.ResizeMode = ResizeMode.CanResize;
                this.Topmost = false;  // luôn ở trên cùng
                //MessageBox.Show("App đã được mở khóa, có thể di chuyển/phóng to.");
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Opacity = e.NewValue;

        }

        #region Private Methods
        private nint WndProcAsync(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCLBUTTONDOWN = 0xA1;
            const int HTCAPTION = 0x2;
            const int WM_CLIPBOARDUPDATE = 0x031D;

            if (_isLocked && msg == WM_NCLBUTTONDOWN && wParam.ToInt32() == HTCAPTION)
            {
                handled = true; // chặn kéo cửa sổ
            }

            if (msg == WM_CLIPBOARDUPDATE)
            {
                _ = HandleClipboardUpdateAsync();// Xử lý cập nhật clipboard bất đồng bộ
            }

            return IntPtr.Zero;
        }
        private async Task HandleClipboardUpdateAsync()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                await TranslateTextAsync(text);
            }
            else if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                using var ms = new MemoryStream();
                encoder.Save(ms);

                string text = RunOcr(ms.ToArray());
                await TranslateTextAsync(text);
            }
        }

        public async Task TranslateTextAsync(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var translator = new GoogleTranslator();
                if (string.IsNullOrWhiteSpace(_targetLang))
                {
                    _targetLang = "vi";
                }
                var result = await translator.TranslateAsync(text, _targetLang);

                TranslatedTextBox.Text = result.Translation;
            }
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

internal static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    public static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
}