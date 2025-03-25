import React, { useEffect, useState } from "react";
import { Table, Button, Alert, Spinner } from "react-bootstrap";
import { BookingAPI } from "../../API";

const BookingHistory = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  useEffect(() => {
    fetchBookingHistory();
  }, []);

  const fetchBookingHistory = async () => {
    try {
      const data = await BookingAPI.GetBookingsByCustomer();
      console.log("Dữ liệu trả về từ API:", data); // Debug dữ liệu API
      setBookings(data);
    } catch (err) {
      console.error("Lỗi khi gọi API:", err);
      setError("Lỗi khi tải lịch sử đặt sân.");
    } finally {
      setLoading(false);
    }
  };
  

  const handleCancelBooking = async (id) => {
    if (!window.confirm("Bạn có chắc chắn muốn hủy đặt sân này?")) return;

    try {
      const result = await BookingAPI.CancelBooking(id); // API mới
      if (result) {
        setSuccessMessage("Hủy đặt sân thành công!");
        fetchBookingHistory(); // Tải lại lịch sử sau khi hủy
      } else {
        setError("Không thể hủy đặt sân. Vui lòng thử lại sau.");
      }
    } catch (err) {
      setError("Không thể hủy đặt sân.");
    }
  };

  return (
    <div className="container mt-4">
      <h2>Lịch sử đặt sân</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      {successMessage && <Alert variant="success">{successMessage}</Alert>}

      {loading ? (
        <Spinner animation="border" />
      ) : (
        <Table striped bordered hover responsive>
          <thead>
            <tr>
              <th>#</th>
              <th>Tên sân</th>
              <th>Loại sân</th>
              <th>Ngày đặt</th>
              <th>Khung giờ</th>
              <th>Giá tiền</th>
              <th>Trạng thái thanh toán</th>
              <th>Trạng thái nhận sân</th>
              <th>Hành động</th>
            </tr>
          </thead>
          <tbody>
            {bookings.length === 0 ? (
              <tr>
                <td colSpan="9" className="text-center">Không có lịch sử đặt sân</td>
              </tr>
            ) : (
              bookings.map((booking, index) => (
                <tr key={booking.id}>
                  <td>{index + 1}</td>
                  <td>{booking.pitchName}</td>
                  <td>{booking.pitchTypeName}</td>
                  <td>{new Date(booking.bookingDate).toLocaleDateString("vi-VN")}</td>
                  <td>
                    {booking.startTime} - {booking.endTime}
                  </td>
                  <td>{booking.price.toLocaleString()} VNĐ</td>
                  <td>
                    <span className={booking.isPaid ? "text-success" : "text-danger"}>
                      {booking.isPaid ? "Đã thanh toán" : "Chưa thanh toán"}
                    </span>
                  </td>
                  <td>
                    <span className={booking.isReceived ? "text-success" : "text-danger"}>
                      {booking.isReceived ? "Đã nhận sân" : "Chưa nhận sân"}
                    </span>
                  </td>
                  <td>
                    {!booking.isReceived && (
                      <Button variant="danger" size="sm" onClick={() => handleCancelBooking(booking.id)}>
                        Hủy
                      </Button>
                    )}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </Table>
      )}
    </div>
  );
};

export default BookingHistory;
