# Aster 0.3 — Thư viện của bạn

App Windows portable cho **Ảnh · Office · Game**, quản lí file và phân loại file mới theo thư mục đích do bạn chọn.

**[Tải Aster.exe](https://github.com/Hdn-test/Aster/releases/latest/download/Aster.exe)** · [Các bản phát hành](https://github.com/Hdn-test/Aster/releases)

Một EXE, không cần file phụ. Windows 10/11 x64 có .NET Framework 4.8. Bản build tại workspace là `Aster-0.3.exe`; bản phát hành tên `Aster.exe`.

## Thư viện được lưu lâu dài

Mỗi mục Ảnh, Office và Game có hai chế độ riêng:

- **Đã lưu:** file bạn chủ động thêm, tồn tại qua đổi tab và mở lại app, cho đến khi bạn gỡ liên kết.
- **Kết quả quét:** kết quả tạm thời trong phiên hiện tại. Quét không tự lưu. Chọn file và bấm **Lưu mục đã chọn** để đưa vào thư viện.

**Thêm file** hoặc kéo thả để lưu liên kết. Có tên hiển thị, bộ sưu tập, yêu thích và tìm kiếm theo tên/đường dẫn/nhóm. Gỡ khỏi thư viện không xóa file gốc. File đã chuyển/xóa bên ngoài vẫn có liên kết kèm trạng thái **Không tìm thấy file**; không tự mất khỏi danh sách. Khi Aster di chuyển file, liên kết được cập nhật theo lịch sử di chuyển.

Office có bộ lọc Word, Excel, PowerPoint, PDF cùng icon của ứng dụng liên kết. Game chỉ nhận EXE hoặc shortcut khởi chạy; nhấp đúp để mở, không cần nút Mở game riêng. Nguồn quét Game bỏ qua executable hỗ trợ như setup/crash handler và không liệt kê DLL, texture, âm thanh ingame. Game có nhiều launcher có thể cần chọn EXE thủ công.

## Phân loại tự động làm gì?

Tính năng này **chuyển file thực tế**, khác với lưu liên kết trong thư viện.

1. Chọn nguồn, ví dụ Downloads.
2. Chọn hoặc tạo nơi lưu cho **5 nhóm**: Ảnh, Office, File nén, Video & âm thanh, Bộ cài. Bỏ chọn nhóm không cần dùng.
3. **Xem trước** để thấy từng đường dẫn đích.
4. **Chuyển file đã xem trước** để chạy một lần, hoặc **Bật theo dõi nguồn** để xử lí file mới ổn định trong phiên app.

Bạn có thể chọn riêng từng nơi lưu hoặc đặt một thư mục gốc để tạo 5 nhóm bên dưới. Các thư mục đích phải riêng biệt; không trùng nguồn, không thuộc hệ thống/game/app và không qua symbolic link/junction. Thư mục đích nằm trong nguồn được phép vì Aster không quét thư mục con.

Ví dụ khi giữ riêng nguồn:

```text
Downloads/hình.png       → Nơi lưu Ảnh/Downloads/hình.png
Downloads/báo cáo.docx   → Nơi lưu Office/Downloads/Word/báo cáo.docx
Downloads/dữ liệu.zip   → Nơi lưu File nén/Downloads/dữ liệu.zip
Downloads/Game/...      → giữ nguyên toàn bộ
Downloads/Dự án/...     → giữ nguyên toàn bộ
```

**Chỉ file nằm trực tiếp trong nguồn được phân loại. Không đi vào bất kỳ thư mục con nào.** Nguồn được khai báo là game hoặc có dấu hiệu gói game/app sẽ bị từ chối. File EXE đã lưu vào thư viện Game cũng được loại khỏi phân loại. Mã nguồn hoặc định dạng chưa có trong 5 nhóm được giữ nguyên.

Theo dõi mặc định tắt, không bật lại khi khởi động. Kiểm tra mỗi 8 giây; file phải ổn định qua hai lần kiểm tra, không mới sửa trong 10 giây và mở được độc quyền. File trùng tên được thêm hậu tố, không ghi đè. Có lịch sử và hoàn tác di chuyển.

## Các phần còn lại

- **Home:** ba thư viện chính, truy cập nhanh, ổ đĩa, thư mục ghim và lịch sử gần đây theo hàng căn trái có đường dẫn, loại, dung lượng, thời gian. Bỏ nội dung trang trí thừa ở góc chi tiết.
- **Explorer:** danh sách/lưới, chọn nhiều, tạo thư mục, sao chép, di chuyển, đổi tên, Thùng rác, ZIP và giải nén ZIP. Back/Forward, phím tắt và kéo thả để sao chép.
- **Chi tiết:** thu gọn khi chưa chọn mục; chọn file/thư mục để mở thông tin. Ảnh xem trước lớn và có cửa sổ phóng to. Bộ giải mã Windows quyết định những định dạng xem được.
- **Search:** chỉ mục của nguồn bạn chọn, bộ lọc có icon, lọc tên/phần mở rộng, dung lượng, ngày, vị trí; lưu truy vấn cùng bộ lọc. Làm mới chỉ mục theo yêu cầu.
- **Storage:** phân tích theo yêu cầu, loại file, thư mục lớn, file lớn, file lâu không sửa; tìm trùng nội dung bằng SHA-256 khi yêu cầu.
- **Transfers:** nhóm theo trạng thái và loại thao tác, nguồn/đích, file hiện tại, tạm dừng/tiếp tục/hủy. Tối đa hai tác vụ đồng thời.
- **Kho ứng dụng:** nhận EXE, chia Ứng dụng/Game, thêm/gỡ liên kết, đổi tên hiển thị, đổi nhóm, tìm kiếm và mở vị trí. Mục “Ứng dụng đã cài” đã được bỏ.
- **Settings:** chủ đề tối/sáng, font, mật độ hàng, hiệu ứng, nguồn chỉ mục, khởi động cùng Windows và ẩn xuống khay hệ thống.

Startup/tray mặc định tắt. Startup trỏ tới vị trí EXE hiện tại; sau khi chuyển EXE sang nơi khác, lưu lại cài đặt. Khi bật tray, thu nhỏ/đóng cửa sổ chỉ ẩn app, tác vụ vẫn chạy; nhấp đúp icon khay để mở lại, dùng **Thoát Aster** để đóng hẳn.

## Lưu dữ liệu và giới hạn

`%LOCALAPPDATA%\Aster\library.json` lưu thư viện riêng để cấu hình từ phiên bản cũ không xóa danh sách. `settings.json` lưu giao diện, nguồn và nơi lưu; `history.json` lưu lịch sử di chuyển. App không tải dữ liệu của bạn lên mạng.

Chưa hỗ trợ RAR/7z, thumbnail lưới đầy đủ, cloud sync, USN index hoặc ký ứng dụng. Chỉ mục tối đa 50.000 file, hiển thị tối đa 5.000 kết quả. Dữ liệu lâu không sửa dựa trên ngày sửa đổi, không đo mức sử dụng. Nhận diện game dùng quy tắc; nếu không chắc nguồn thuộc game, xem trước danh sách trước khi chạy.

Sao chép có file tạm được dọn khi hủy. Di chuyển/giải nén thư mục bị hủy hoặc lỗi có thể giữ lại phần đích đã hoàn tất; nguồn thư mục chuyển khác ổ chỉ bị xóa sau khi sao chép hoàn tất. Hoàn tác dừng khi vị trí cũ đã có dữ liệu; hoàn tác thư mục khác ổ chưa hỗ trợ. Thùng rác phụ thuộc cấu hình Windows/ổ đĩa.

## Build và kiểm tra

```powershell
.\build.ps1
.\test.ps1
.\verify-ui.ps1
```

Dùng trình biên dịch .NET Framework có sẵn trên Windows. Kiểm tra tạo sandbox dưới `artifacts`; không dùng file cá nhân, không chạy game/bộ cài, không gỡ phần mềm hay sửa startup thật. Bản 0.3 qua 52 kiểm tra xử lí dữ liệu và 49 kiểm tra giao diện, gồm lưu thư viện qua đổi tab/mở lại, không tự lưu kết quả quét, căn trái lịch sử và bảo vệ game/dự án.

Tham khảo bố cục: [Files](https://files.community/), [Fluent 2 layout](https://fluent2.microsoft.design/layout). Giao diện dùng Segoe UI, icon ứng dụng liên kết từ Windows và Segoe MDL2 Assets cho thao tác.

