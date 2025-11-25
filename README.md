# PhotoTranslationTool

📸 PhotoTranslationTool
PhotoTranslationTool là một ứng dụng WPF (Windows Presentation Foundation) giúp bạn dịch văn bản từ hình ảnh một cách nhanh chóng và tiện lợi. Ứng dụng kết hợp OCR (Tesseract) để nhận diện chữ trong ảnh và Google Translate (qua GTranslate) để dịch sang ngôn ngữ mong muốn.
👉 Hiện tại ứng dụng hỗ trợ OCR cho tiếng Anh (English) và tiếng Hàn (Korean).

✨ Tính năng chính
- 🖼️ OCR từ ảnh: Nhận diện văn bản từ ảnh (jpg, png, bmp) hoặc clipboard.
- 🌐 Dịch tự động: Dịch văn bản sang nhiều ngôn ngữ với Google Translate.
- 📋 Clipboard hỗ trợ: Dán ảnh hoặc text trực tiếp từ clipboard để dịch ngay.
- 🖱️ Drag & Drop: Kéo thả ảnh vào cửa sổ để OCR nhanh chóng.
- 🎛️ Hotkey toàn cục: Tùy chỉnh phím tắt để mở/ẩn app hoặc paste từ clipboard.
- 🖥️ Tray icon: Chạy ngầm dưới system tray, double click để mở lại.
- ⚙️ Khởi động cùng Windows: Tùy chọn bật/tắt auto-start khi máy khởi động.
- 🎨 UI hiện đại: Giao diện Metro bo tròn góc, hỗ trợ dark/light theme.

🛠️ Công nghệ sử dụng
- WPF + MahApps.Metro → UI hiện đại, dễ dùng.
- Tesseract OCR → Nhận diện văn bản từ ảnh (hiện hỗ trợ tiếng Anh và tiếng Hàn).
- GTranslate (GoogleTranslator) → Dịch văn bản sang nhiều ngôn ngữ.
- Hardcodet.Wpf.TaskbarNotification → Quản lý icon tray.
- GlobalHotkey (Win32 API) → Đăng ký và xử lý hotkey toàn cục.
