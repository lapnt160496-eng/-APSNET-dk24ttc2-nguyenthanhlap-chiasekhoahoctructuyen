# -APSNET-dk24ttc2-nguyenthanhlap-chiasekhoahoctructuyen
# Đề tài: Xây dựng website chia sẻ khóa học trực tuyến

## Thông tin sinh viên
- Họ tên: Nguyễn Thành Lập  
- Mã số sinh viên:  170124161
- Email : Lapnt160496@tvu-onschool.edu.vn
- Phone : 035 9664 596 
- Lớp: DK24TTC2
- Giảng viên hướng dẫn: Thầy Đoàn Phước Miền

## Mô tả đồ án
Website chia sẻ khóa học trực tuyến cho phép người dùng đăng ký tài khoản, tìm kiếm và tham gia các khóa học theo chủ đề.  
Người quản trị có thể thêm, chỉnh sửa, xóa khóa học và quản lý người dùng.

## Tiến Trình thực hiện 
# Tuần 1 
- Chuẩn bị xây dựng form bài báo cáo, định dạng file Word đúng yêu cầu.
- Nghiên cứu, cài đặt Phần mềm APS.NET và các công cụ hổ trợ lập trình khác .
- Phát họa sơ bộ khung sườn của website chia sẻ khóa học trực tuyến


# fill test code python
from flask import Flask, render_template_string, request

app = Flask(__name__)

# Danh sách khóa học mẫu
courses = [
    {"id": 1, "title": "Python cơ bản", "description": "Học cú pháp và cấu trúc cơ bản của Python."},
    {"id": 2, "title": "Phân tích dữ liệu", "description": "Sử dụng Pandas và Matplotlib để phân tích dữ liệu."},
    {"id": 3, "title": "Lập trình web với Flask", "description": "Xây dựng ứng dụng web đơn giản bằng Flask."}
]

# Giao diện HTML đơn giản
html_template = """
<!DOCTYPE html>
<html>
<head>
    <title>Khóa học trực tuyến</title>
</head>
<body>
    <h1>Danh sách khóa học</h1>
    <ul>
        {% for course in courses %}
        <li>
            <strong>{{ course.title }}</strong><br>
            {{ course.description }}<br>
            <form method="POST" action="/register">
                <input type="hidden" name="course_id" value="{{ course.id }}">
                <input type="text" name="name" placeholder="Tên của bạn" required>
                <button type="submit">Đăng ký</button>
            </form>
        </li>
        {% endfor %}
    </ul>
</body>
</html>
"""

@app.route("/", methods=["GET"])
def home():
    return render_template_string(html_template, courses=courses)

@app.route("/register", methods=["POST"])
def register():
    name = request.form.get("name")
    course_id = int(request.form.get("course_id"))
    course = next((c for c in courses if c["id"] == course_id), None)
    if course:
        return f"Cảm ơn {name} đã đăng ký khóa học: {course['title']}!"
    else:
        return "Khóa học không tồn tại."

if __name__ == "__main__":
    app.run(debug=True)
