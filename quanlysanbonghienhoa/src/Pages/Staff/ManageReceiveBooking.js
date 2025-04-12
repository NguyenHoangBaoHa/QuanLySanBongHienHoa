import React, { useEffect, useState } from "react";
import { Table, Container, Badge, Form } from "react-bootstrap";
import moment from "moment";
import { BookingAPI } from "../../API";

const ManageReceiveBooking = () => {
  const [bookings, setBookings] = useState([]);
  const [searchText, setSearchText] = useState("");

  useEffect(() => {
    fetchBookings();
  }, []);

  const fetchBookings = async () => {
    try {
      const data = await BookingAPI.GetAllBookings();
      setBookings(data);
    } catch (error) {
      console.error("Lỗi khi tải danh sách booking: ", error);
    }
  };

  const filteredBookings = bookings
    .filter((booking) => booking.isReceived) // ✅ CHỈ hiện những đơn ĐÃ nhận sân
    .filter(
      (booking) =>
        booking.displayName.toLowerCase().includes(searchText.toLowerCase()) ||
        booking.phoneNumber.includes(searchText)
    );

  return (
    <Container className="mt-4">
      <h2 className="mb-4">Quản Lý Nhận Sân</h2>

      <Form.Control
        type="text"
        placeholder="Tìm kiếm theo tên hoặc số điện thoại..."
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
        className="mb-3"
      />

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>STT</th>
            <th>Họ Tên</th>
            <th>Số Điện Thoại</th>
            <th>Chi Tiết Sân</th>
            <th>Ngày Đặt Sân</th>
            <th>Trạng Thái Thanh Toán</th>
          </tr>
        </thead>
        <tbody>
          {filteredBookings.map((booking, index) => (
            <tr key={booking.id}>
              <td>{index + 1}</td>
              <td>{booking.displayName}</td>
              <td>{booking.phoneNumber}</td>
              <td>{booking.pitchName} - {booking.pitchTypeName}</td>
              <td>{moment(booking.bookingDate).format("DD/MM/YYYY HH:mm")}</td>
              <td>
                <Badge bg={booking.paymentStatus === "Paid" ? "success" : "danger"}>
                  {booking.paymentStatus === "Paid" ? "Đã Thanh Toán" : "Chưa Thanh Toán"}
                </Badge>
              </td>
            </tr>
          ))}
          {filteredBookings.length === 0 && (
            <tr>
              <td colSpan="7" className="text-center text-muted">
                Không có khách hàng nào đã nhận sân.
              </td>
            </tr>
          )}
        </tbody>
      </Table>
    </Container>
  );
};

export default ManageReceiveBooking;
