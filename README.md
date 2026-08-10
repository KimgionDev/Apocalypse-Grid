# Apocalypse Grid - 2D Top-Down Shooter

Đây là mã nguồn của đồ án Niên luận Cơ sở ngành Kỹ thuật phần mềm. 
Đề tài: XÂY DỰNG MINI-GAME 2D TOP-DOWN SHOOTER TÍCH HỢP THUẬT TOÁN SINH MÀN CHƠI TỰ ĐỘNG BSP.

## Thông tin tác giả
- Sinh viên thực hiện: Nguyễn Ngọc Đức Phát (MSSV: B2303840)
- Lớp: DI2396A1
- Cán bộ hướng dẫn: Gv. Trương Thị Thanh Tuyền (Đại học Cần Thơ)

## Giới thiệu
Apocalypse Grid là một trò chơi Offline Mini-game thể loại 2D Top-Down Shooter mang phong cách Rogue-lite. Trò chơi tập trung vào khả năng "chơi lại" vô tận thông qua việc tự động sinh các bản đồ hoàn toàn mới mỗi khi bắt đầu màn chơi, kết hợp với nhịp độ chiến đấu dồn dập và hệ thống nâng cấp chỉ số.

## Các tính năng nổi bật
- Sinh bản đồ tự động (Procedural Dungeon Generation): Áp dụng thuật toán Phân vùng không gian nhị phân (BSP) bằng vòng lặp kết hợp cấu trúc Queue để sinh các phòng, đảm bảo không tràn bộ nhớ (Stack Overflow). Kết hợp Random Walk và Bitmasking để tạo vách tường gồ ghề tự nhiên.
- Tối ưu hóa hiệu năng (Object Pooling): Toàn bộ đạn và quái vật được quản lý thông qua Object Pool, giúp duy trì ổn định trên 60 FPS khi có hàng trăm thực thể xuất hiện trên màn hình.
- Hệ thống chiến đấu và AI: Cơ chế ngắm bắn 360 độ, nạp đạn, và hệ thống quái vật có khả năng bám đuổi, né vật cản và tái sinh theo đợt.
- Giao dịch và Nâng cấp (Dashboard): Sử dụng vàng thu thập được để nâng cấp các chỉ số sinh tồn (Máu, Sát thương, Tốc độ) và mua đạn.
- Hệ thống Rớt đồ thông minh (Smart Loot): Quái vật có tỉ lệ độc lập rớt Vàng, Vật phẩm nhiệm vụ, Hộp tiếp tế Đạn và Vật phẩm hồi Máu.

## Công nghệ sử dụng
- Nền tảng: Unity Engine (2D Core)
- Ngôn ngữ lập trình: C#
- Design Patterns: Singleton, Object Pooling
- Cấu trúc dữ liệu: Queue, List, HashSet, Dictionary (Vector2Int, BoundsInt)
- Thành phần Unity: Tilemap, Rigidbody2D, Collider2D, ScriptableObject

## Hướng dẫn cài đặt và sử dụng
1. Giải nén thư mục trò chơi hoặc Clone Repository này về máy.
2. Mở tệp ApocalypseGrid.exe (trên hệ điều hành Windows) hoặc khởi chạy Scene trực tiếp từ Editor của Unity.

## Cách chơi
- Sảnh chờ (Dashboard): Sử dụng Chuột trái để nâng cấp chỉ số và mua đạn bằng Vàng. Bấm nút START để bắt đầu màn chơi.
- Di chuyển: Sử dụng các phím W - A - S - D.
- Chiến đấu: Dùng chuột để điều hướng tâm ngắm, click Chuột trái để bắn. Nhấn phím R để nạp đạn.
- Mục tiêu: Khám phá bản đồ, tiêu diệt quái vật để thu thập Vàng và Vật phẩm nhiệm vụ. Khi đủ vật phẩm, Cổng dịch chuyển sẽ tự động mở khóa. Tìm đường đến Cổng để nhận thưởng và bước sang cấp độ tiếp theo với độ khó tăng dần.
