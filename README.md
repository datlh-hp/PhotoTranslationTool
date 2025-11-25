# PhotoTranslationTool

📸 PhotoTranslationTool
PhotoTranslationTool is a WPF (Windows Presentation Foundation) application that helps you translate text from images quickly and conveniently. It combines OCR (Tesseract) to recognize text in images and Google Translate (via GTranslate) to translate into your desired language.
👉 Currently, the application supports OCR for English and Korean.

✨ Key Features
- 🖼️ OCR from images: Recognize text from images (jpg, png, bmp) or directly from the clipboard.
- 🌐 Automatic translation: Translate recognized text into multiple languages using Google Translate.
- 📋 Clipboard support: Paste images or text directly from the clipboard for instant translation.
- 🖱️ Drag & Drop: Drag and drop images into the window for quick OCR.
- 🎛️ Global hotkey: Customize a system-wide hotkey to show/hide the app or paste from clipboard.
- 🖥️ Tray icon: Runs in the system tray, double-click to reopen the app.

🚀 How to Use
- Select an image using the Select Image button or drag & drop it into the app.
- The app will automatically perform OCR and display the text in the OCR Text box.
- Choose your target language from the LangComboBox.
- Click Translate to translate the text.
- Use Ctrl+V to paste images/text from the clipboard or trigger the global hotkey to open the app instantly.

🛠️ Technologies Used
- WPF + MahApps.Metro → Modern, user-friendly UI.
- Tesseract OCR → Text recognition from images (currently supports English and Korean).
- GTranslate (GoogleTranslator) → Translate text into multiple languages.
- Hardcodet.Wpf.TaskbarNotification → System tray icon management.
- GlobalHotkey (Win32 API) → Register and handle global hotkeys.
