# Aster 0.2.0

- Ba mục chính dễ thấy: **Ảnh · Office · Game**.
- Office có Word, Excel, PowerPoint, PDF; phân loại vào `Office/Word`, `Office/Excel`…
- Thư viện game chỉ hiển thị EXE/shortcut khởi chạy, bỏ qua texture, DLL và tài nguyên ingame. Thêm nguồn game riêng để nhận diện rõ hơn.
- **Kho ứng dụng** nhận EXE, chia Ứng dụng / Game, thêm, mở và gỡ khỏi kho mà không xóa file gốc.
- Icon lấy từ Windows, ứng dụng liên kết và file EXE.
- Lịch sử gần đây chống trùng, có đường dẫn, loại, dung lượng và thời gian mở.
- Ảnh xem trước lớn hơn; bấm ảnh hoặc nút Phóng to để xem trong cửa sổ rộng.
- Back / Forward và Alt+Left / Alt+Right.
- Cuộn theo pixel với chuyển động easing nhẹ.
- Chọn bộ cài EXE/MSI, lọc bộ cài trên Desktop, mở trình gỡ ứng dụng đã cài.
- Tác vụ lọc theo trạng thái và loại thao tác, có file hiện tại, nguồn và đích.
- Dung lượng và phân loại có các bước hướng dẫn và thao tác rõ hơn.
- Khởi động cùng Windows; thu nhỏ / đóng xuống khay hệ thống khi bật tùy chọn. Menu khay có Mở và Thoát.

Windows 10/11 x64, .NET Framework 4.8. Một EXE portable; không cần XAML/config ngoài. Startup và tray mặc định tắt. Không tự chạy bộ cài, gỡ phần mềm hay chuyển dữ liệu cho đến khi người dùng chọn thao tác tương ứng.

