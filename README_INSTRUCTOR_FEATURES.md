# Online Course Management - Instructor Features

Đã bổ sung đầy đủ code cho 4 chức năng của Instructor trên nền project WPF/MVVM + EF Core có sẵn:
**quản lý course, quản lý lesson, quản lý student, và đề xuất course.**

## Kiến trúc

```
DataAccess    -> models + DbContext (giữ nguyên, không đổi)
Repositories  -> DataAccess       (mỗi method tự mở/đóng 1 DbContext ngắn hạn)
Services      -> Repositories     (validate + business rule)
Presentation  -> Services         (WPF, MVVM tự viết tay - không dùng thư viện ngoài)
```

Đã thêm `ProjectReference` giữa 4 project (trước đó chưa có), nên nhớ **Rebuild Solution**
sau khi mở lại project.

### MVVM tự viết tay, không cần NuGet thêm

`Presentation/Helpers/RelayCommand.cs` và `AsyncRelayCommand.cs` là 2 class ICommand viết tay
(không dùng CommunityToolkit.Mvvm), để tránh phải restore thêm package. `AsyncRelayCommand`
tự bắt exception và hiện `MessageBox` thay vì làm crash app - rất hữu ích vì mọi thao tác đều
gọi thẳng SQL Server.

### Vì sao mỗi Repository tự mở `DbContext` riêng

Toàn bộ Repository dùng pattern `using var context = new OnlineCourseManagementDbContext();`
trong từng method thay vì giữ 1 DbContext dùng chung. Lý do: tránh lỗi kinh điển của EF Core khi
`Update()` một entity đã tách rời (detached) mà navigation property (Category, Lessons,
Enrollments...) vẫn còn dữ liệu từ lần Include() trước - EF sẽ cố gắn cả navigation đó vào
luôn, gây insert/update thừa. Các method Update (Course, Lesson, Enrollment status) đều
fetch lại entity "sạch" trong context mới rồi chỉ set field cần đổi, an toàn tuyệt đối.

## Cách chạy

1. Cập nhật connection string trong `Presentation/appsettings.json` nếu SQL Server của bạn khác
   `Server=(local)`.
2. Chạy `OnlineCourseDB.sql` để tạo DB + seed data (2 Instructor, 2 Course, 2 Student, 5 Lesson).
3. Mở solution trong Visual Studio, **Rebuild Solution** (lần đầu cần restore NuGet:
   `Microsoft.EntityFrameworkCore.SqlServer`).
4. Set `Presentation` làm Startup Project, F5.
5. Đăng nhập bằng 1 trong 2 email seed sẵn: `vana@example.com` hoặc `thib@example.com`
   (chưa có mật khẩu - hệ thống chưa có bảng Account, xem phần "Đăng nhập" bên dưới).

## Chi tiết từng chức năng

### 1. Đăng nhập (Login)
Vì schema chưa có bảng Account, `LoginViewModel` chỉ tra `Instructors` theo Email
(`InstructorRepository.GetByEmailAsync`). Instructor tìm được lưu vào
`Presentation/Helpers/InstructorSession.Current` (static, dùng cho cả session).
Đây là điểm bạn có thể thay thế dễ dàng sau này nếu thêm bảng Account/mật khẩu thật -
chỉ cần sửa `InstructorService.LoginAsync`.

### 2. Quản lý Course (`CourseListView` + `CourseEditWindow`)
- Danh sách course của Instructor đang đăng nhập, tìm theo tên/danh mục.
- Thêm/Sửa qua modal `CourseEditWindow`, validate: tên bắt buộc, giá >= 0, số giờ > 0,
  phải chọn danh mục.
- Xóa: chặn nếu course đã có học viên đăng ký (`CourseService.DeleteCourseAsync`) - tránh
  xóa nhầm dữ liệu học viên. Về mặt DB, FK đã cấu hình `ON DELETE CASCADE` nên nếu cần xóa
  cứng, có thể bỏ check này.
- Nút "Quản lý bài học" mở `LessonListView` ngay trong khung nội dung chính (không mở
  window riêng), có nút "Quay lại".

### 3. Quản lý Lesson (`LessonListView` + `LessonEditWindow`)
- Lesson thuộc 1 course, sắp xếp theo `OrderIndex`.
- Thêm bài học mới tự động lấy `OrderIndex` kế tiếp (`GetNextOrderIndexAsync`).
- Nút Lên/Xuống hoán đổi `OrderIndex` giữa 2 bài liền kề (`LessonService.MoveLessonUpAsync` /
  `MoveLessonDownAsync`).

### 4. Quản lý Student (`StudentListView` + `StudentDetailWindow`)
- Bảng hiển thị tất cả **lượt đăng ký** (Enrollment) của các course thuộc Instructor này -
  1 học viên có thể xuất hiện nhiều dòng nếu học nhiều course của bạn.
- Lọc theo khóa học, tìm theo tên/email.
- Chọn 1 dòng -> đổi trạng thái (Đã đăng ký / Đang học / Đã hoàn thành / Đã hủy) hoặc xóa
  hẳn lượt đăng ký.
- "Xem lịch sử" mở cửa sổ hiển thị **toàn bộ** lịch sử học của học viên đó (kể cả course
  của Instructor khác) - hữu ích để tư vấn đề xuất khóa học tiếp theo.
- Form "Ghi danh học viên vào khóa học": nhập email - nếu email đã tồn tại thì ghi danh
  học viên cũ, chưa có thì tạo học viên mới.

### 5. Đề xuất Course (`RecommendationView`)
Theo đúng lựa chọn của bạn: **gợi ý course cho 1 student cụ thể, dựa trên category học
viên đã học, loại trừ course đã đăng ký.**

Thuật toán (`RecommendationService.RecommendForStudentAsync`):
1. Lấy lịch sử đăng ký của học viên (loại trừ trạng thái "Đã hủy").
2. Lấy danh sách `CategoryId` học viên đã từng học.
3. Tìm course cùng category, loại trừ course đã đăng ký (kể cả đã hủy - không gợi ý lại).
4. Xếp hạng theo số lượt đăng ký (độ phổ biến) rồi đến ngày tạo mới nhất.
5. **Cold start**: nếu học viên chưa có lịch sử phù hợp, tự động fallback sang course phổ
   biến nhất toàn hệ thống, có ghi rõ lý do trong UI.
- Checkbox "Chỉ khóa học của tôi" giới hạn đề xuất trong course của Instructor đang đăng
  nhập, thay vì toàn hệ thống.
- Mỗi đề xuất có nút "Ghi danh học viên" để ghi danh trực tiếp từ màn hình gợi ý.

## Lưu ý quan trọng - vui lòng test kỹ

Môi trường mình dùng để viết code này **không có .NET SDK và không có mạng**, nên mình
**không build/run thử được**. Mình đã:
- Đối chiếu kỹ tên property với đúng các model trong `DataAccess/Models` của bạn.
- Kiểm tra cân bằng dấu ngoặc `{}` trong toàn bộ file `.cs` và parse-validate toàn bộ XML
  trong file `.xaml`.
- Tránh các API EF Core ít phổ biến, ưu tiên `ToListAsync/FirstOrDefaultAsync/FindAsync/
  SaveChangesAsync` v.v. đã chắc chắn tồn tại.

Nhưng đây vẫn nên được xem là **bản nháp đầu tiên chất lượng cao**, không phải code đã
được đảm bảo build xanh 100%. Khi mở trong Visual Studio, nếu có lỗi build (thường sẽ chỉ
là lỗi nhỏ, ví dụ tên using hoặc dấu ngoặc), hãy sửa theo thông báo lỗi - kiến trúc và
logic tổng thể đã được thiết kế chắc chắn.

## Gợi ý mở rộng sau này
- Thêm bảng `Accounts` với mật khẩu hash thật (BCrypt/PBKDF2) thay vì đăng nhập bằng Email.
- Thêm phân trang cho danh sách Course/Student khi dữ liệu lớn.
- Thêm chức năng cho Student/Admin (hiện tại chỉ có Instructor theo đúng yêu cầu).
