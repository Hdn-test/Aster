# Aster — Không gian dữ liệu của bạn

App Windows portable **0.2**. Ba mục chính **Ảnh · Office · Game**, cùng **Kho ứng dụng** dành riêng cho EXE. Giao diện tối xanh mực, nút gradient xanh–tím, cuộn theo pixel với easing, hỗ trợ giao diện sáng. Xây bằng WPF và .NET Framework 4.8; không cần Node, SDK hay trình duyệt để chạy.

## Mở app

Tải hoặc chép **một file `Aster.exe`** từ GitHub Release rồi mở trực tiếp. Bản tại workspace có tên `Aster-0.2.exe` để không ghi đè phiên cũ đang mở. Giao diện và icon đã nhúng trong EXE; không cần giải nén, XAML hay config cạnh app. Windows 10/11 x64 có .NET Framework 4.8. Cấu hình và lịch sử được app tạo tại `%LOCALAPPDATA%\Aster` khi sử dụng.

## Những phần mới

- **Ảnh:** nguồn cá nhân, Downloads hoặc thư mục bạn thêm; bỏ qua thư mục game/app nhận diện được. Icon ứng dụng liên kết do Windows cung cấp; có thể dùng icon loại file nếu Windows không cung cấp icon app. Ảnh xem trước lớn 320 px, bấm **Phóng to ảnh** hoặc bấm ảnh để xem cửa sổ rộng.
- **Office:** bộ lọc Word, Excel, PowerPoint, PDF. Phân loại tự động giữ thư mục nguồn/ngữ cảnh và tạo `Office/Word`, `Office/Excel`…
- **Game:** chọn **Thêm thư mục** để khai báo nguồn game. Chỉ hiện một EXE/shortcut khởi chạy phù hợp mỗi thư mục, bỏ qua tên executable hỗ trợ như crash handler, updater, setup. Đây là nhận diện bằng quy tắc, có thể cần chọn EXE thủ công với game có nhiều launcher. Không di chuyển EXE game ra khỏi tài nguyên. Nút **Game trong thư mục** cho biết các điểm khởi chạy bên trong.
- **Kho ứng dụng:** hai nhóm Ứng dụng / Game; **Thêm EXE**, mở, chọn nhiều và **Gỡ khỏi kho**. Chỉ nhận `.exe`; gỡ chỉ bỏ liên kết, không xóa file hay gỡ cài đặt.
- **Ứng dụng đã cài:** danh mục Win32, icon, chọn bộ cài EXE/MSI/MSIX; lọc EXE/MSI trên Desktop. **Gỡ ứng dụng** mở trình gỡ của ứng dụng hoặc trang Apps của Windows; việc gỡ do người dùng tiếp tục trong trình đó. Bộ lọc EXE trên Desktop bao gồm cả app portable vì không thể xác định chắc mọi EXE là bộ cài.
- **Startup / tray:** bật trong Settings rồi **Lưu cài đặt**. Startup dùng khóa `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\Aster` trỏ tới vị trí EXE hiện tại. Nếu chuyển EXE sang chỗ khác, lưu lại cài đặt để cập nhật đường dẫn. Khi bật tray, thu nhỏ hoặc đóng cửa sổ sẽ ẩn, tác vụ tiếp tục chạy; nhấp đúp icon khay để mở lại, dùng **Thoát Aster** để đóng hẳn. Startup và tray mặc định tắt.
- **Back / Forward:** nút trong Explorer và Alt+Left / Alt+Right.
- **Gần đây:** chống trùng không phân biệt chữ hoa/thường, giữ đường dẫn, loại, dung lượng và lần mở. Thư mục và file hiện ở hai vùng riêng.
- **Tác vụ:** bộ lọc trạng thái và loại thao tác, file hiện tại, nguồn/đích, thời gian bắt đầu và tổng số đang chạy/hoàn tất/lỗi.
- **Dung lượng / phân loại:** các bước hướng dẫn, nhóm loại file, thư mục lớn, file lớn, file lâu không sửa; quét nặng theo yêu cầu.

Các phép kiểm tra chỉ dùng sandbox tạo dưới `artifacts`, không chạy bộ cài, không gỡ ứng dụng và không thay đổi startup thật của người dùng.

## Các màn hình

- **Home:** thư mục nhanh, ổ đĩa và dung lượng thực, thư mục ghim, thư mục gần đây và file mở bằng Aster.
- **Explorer:** nhập đường dẫn, điều hướng, danh sách/lưới, chọn nhiều, kéo thả để sao chép vào thư mục hiện tại, tạo thư mục, sao chép, di chuyển, đổi tên đơn/hàng loạt, Thùng rác, ZIP và giải nén ZIP. Bảng phải hiển thị thông tin và xem trước ảnh mà bộ giải mã Windows hỗ trợ.
- **Search:** chỉ mục trong bộ nhớ của nguồn do bạn chọn, lọc tên/phần mở rộng, loại, dung lượng, ngày sửa đổi và đường dẫn. Lưu truy vấn kèm các bộ lọc. Thêm nguồn rồi bấm **Lập chỉ mục**; chỉ mục cần làm mới sau khi dữ liệu thay đổi.
- **Storage:** chọn thư mục để phân tích tổng dung lượng, loại file, thư mục lớn và file lớn. Tìm file trùng bằng SHA-256 khi yêu cầu. Dữ liệu lâu không sửa dựa trên ngày sửa đổi, không phải lịch sử sử dụng thực tế.
- **Transfers:** tối đa hai tác vụ đồng thời, tiến độ theo file hiện tại, tạm dừng, tiếp tục, hủy, lỗi và lịch sử. Hoàn tác toàn bộ các lần di chuyển chưa hoàn tác theo thứ tự ngược.
- **Phân loại:** sửa nhóm đích và phần mở rộng, xem trước, phân loại ngay, bật/tắt theo dõi một nguồn trong phiên mở app.
- **Ứng dụng & game:** danh mục Win32 từ Windows, tìm kiếm, sắp xếp bằng tiêu đề cột, chỉnh tên nhóm bằng nhấp đúp ô nhóm. Thêm `.lnk` hoặc `.exe` để mở ứng dụng từ Aster; đây cũng là cách thêm game hoặc ứng dụng Store vào thư viện.
- **Settings:** chủ đề, font, kích thước hàng, hiệu ứng và nguồn lập chỉ mục. Phiên bản này dùng tiếng Việt và mở file bằng ứng dụng mặc định của Windows.

## Phân loại mà không trộn dữ liệu

Ví dụ:

```text
Downloads/hinh.jpg               → Kho/Downloads/Ảnh/hinh.jpg
Downloads/Dự án A/cover.png      → Kho/Downloads/Dự án A/Ảnh/cover.png
Downloads/Dự án B/cover.png      → Kho/Downloads/Dự án B/Ảnh/cover.png
Downloads/báo cáo.docx           → Kho/Downloads/Office/Word/báo cáo.docx
Downloads/Game/texture.png       → giữ nguyên khi nhận diện thư mục game/app
```

Hai nguồn khác nhau có cùng tên được cấp thư mục riêng, ví dụ `Downloads` và `Downloads (2)`. Không ghi đè khi file trùng tên; thêm hậu tố số. Phần mở rộng chưa có quy tắc được giữ nguyên.

Nguồn và đích phải nằm ngoài nhau. App bỏ qua thư mục hệ thống, AppData, Program Files, tên thư mục game phổ biến, thư mục có DLL, tài nguyên Unity và các gói chứa executable. Thư mục con chứa executable kèm dữ liệu được giữ nguyên; nguồn có executable và tài nguyên cũng được giữ nguyên, ngoại trừ nguồn có tên `Downloads` dành cho bộ cài rời. Việc nhận diện này dùng quy tắc bảo thủ, không phải một cơ sở dữ liệu nhận diện mọi game. Một bộ tài nguyên không có dấu hiệu cài đặt vẫn có thể được coi là dữ liệu cá nhân: hãy kiểm tra **Xem trước** trước khi bật tự động.

Tự động mặc định **tắt**, không bật lại khi khởi động. Khi bạn bật, app kiểm tra mỗi 8 giây; file phải ổn định qua hai lần kiểm tra, không mới sửa trong 10 giây, và mở được độc quyền. Nguồn/đích tự động không được đi qua junction hoặc symbolic link. Mỗi lần di chuyển hoàn tất được ghi lịch sử.

## Hành vi tác vụ

- Sao chép theo khối 1 MB, chạy nền, file đang sao chép có đuôi tạm và được dọn nếu hủy.
- Di chuyển cùng ổ dùng thao tác đổi vị trí của hệ thống, thường hoàn tất ngay; không có bước dài để tạm dừng.
- Di chuyển khác ổ sao chép trước khi xóa nguồn. Nếu tác vụ thư mục bị hủy/lỗi, phần đích đã sao chép có thể còn lại, nguồn được giữ khi chưa hoàn tất.
- Giải nén bị hủy/lỗi có thể để lại thư mục kết quả một phần. App chặn đường dẫn ZIP vượt khỏi thư mục đích.
- Hoàn tác không ghi đè vị trí cũ và dừng nếu gặp xung đột. Hoàn tác thư mục chuyển khác ổ chưa được hỗ trợ; file khác ổ được hỗ trợ.
- App dùng Thùng rác qua Windows; khả năng khôi phục phụ thuộc ổ đĩa và cấu hình Thùng rác.
- Danh mục ứng dụng đọc registry Win32; chưa tự lập danh mục đầy đủ ứng dụng Store hoặc mọi game trong launcher. Thêm shortcut để quản lí chúng.
- Chỉ mục giới hạn 50.000 file mỗi phiên; kết quả hiển thị tối đa 5.000. Chưa có chỉ mục nền USN, thumbnail lưới, đồng bộ đám mây, hỗ trợ RAR/7z, tùy chỉnh phím tắt hoặc chứng chỉ ký ứng dụng.

Cấu hình và lịch sử nằm ở `%LOCALAPPDATA%\Aster`. App không gửi dữ liệu lên mạng. Ảnh xem trước được đọc xong rồi đóng file, tránh giữ khóa file. Chưa đưa ra cam kết tốc độ so với Windows Explorer vì chưa benchmark trên dữ liệu thực của bạn.

## Phát triển và kiểm tra

```powershell
.\build.ps1
.\test.ps1
.\verify-ui.ps1
```

`build.ps1` dùng trình biên dịch .NET Framework có sẵn trên máy Windows. Các kiểm tra tạo dữ liệu riêng dưới `artifacts`, không dùng file cá nhân. `verify-ui.ps1` render tám màn hình WPF thật, kiểm tra điều hướng, Explorer, chỉ mục, bộ lọc và cài đặt. Ảnh trong `artifacts` dùng dữ liệu thử, không phải thư viện file của bạn.

Tham khảo thiết kế: [Fluent 2 typography](https://fluent2.microsoft.design/typography), [Fluent 2 motion](https://fluent2.microsoft.design/motion), [Files](https://www.files.community/). Hai ảnh tham khảo của bạn định hướng bố cục ba vùng, bảng file và màu tối.

