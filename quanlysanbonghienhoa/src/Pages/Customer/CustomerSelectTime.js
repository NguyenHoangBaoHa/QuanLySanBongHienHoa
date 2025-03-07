import React, { useEffect, useState } from "react";
import { Table, Button, Spinner, Alert } from "react-bootstrap";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { BookingAPI } from "../../API"; // Import API đã cập nhật
const CustomerSelectTime = ({ pitchId }) => {
  const [schedule, setSchedule] = useState([]);
  const [startDate, setStartDate] = useState(new Date()); // Mặc định ngày hiện tại
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchSchedule();
  }, [startDate]); // Tự động gọi API khi thay đổi startDate

  // Gọi API lấy lịch đặt sân
  const fetchSchedule = async () => {
    if (!pitchId) return;
    setLoading(true);
    setError("");

    try {
      const formattedDate = startDate.toISOString().split("T")[0]; // Định dạng yyyy-MM-dd
      const response = await BookingAPI.get(`/api/booking/pitch/${pitchId}/week?startDate=${formattedDate}`);
      setSchedule(response.data);
    } catch (err) {
      setError("Không thể tải lịch đặt sân. Vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };

  // Chuyển đổi ngày trong tuần thành tên thứ
  const getDayOfWeek = (dateString) => {
    const days = ["Chủ Nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7"];
    const date = new Date(dateString);
    return days[date.getDay()];
  };

  return (
    <div className="container mt-4">
      <h2 className="text-center mb-3">Lịch đặt sân</h2>

      {/* Chọn ngày bắt đầu */}
      <div className="d-flex justify-content-between mb-3">
        <DatePicker
          selected={startDate}
          onChange={(date) => setStartDate(date)}
          dateFormat="dd/MM/yyyy"
          className="form-control"
          minDate={new Date()} // Không cho chọn ngày trong quá khứ
        />
        <Button variant="primary" onClick={fetchSchedule} disabled={loading}>
          {loading ? <Spinner animation="border" size="sm" /> : "Làm mới"}
        </Button>
      </div>

      {/* Hiển thị lỗi nếu có */}
      {error && <Alert variant="danger">{error}</Alert>}

      {/* Hiển thị bảng lịch đặt sân */}
      <Table bordered hover>
        <thead>
          <tr>
            <th>Ngày</th>
            <th>Giờ</th>
            <th>Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          {schedule.length > 0 ? (
            schedule.map((booking, index) => (
              <tr key={index}>
                <td>{getDayOfWeek(booking.bookingDate)}</td>
                <td>{new Date(booking.bookingDate).toLocaleTimeString()}</td>
                <td>
                  <span className={`badge ${booking.isReceived ? "bg-danger" : "bg-success"}`}>
                    {booking.isReceived ? "Đã đặt" : "Còn trống"}
                  </span>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="3" className="text-center">
                Không có lịch đặt sân trong tuần này.
              </td>
            </tr>
          )}
        </tbody>
      </Table>
    </div>
  );
};

export default CustomerSelectTime;
