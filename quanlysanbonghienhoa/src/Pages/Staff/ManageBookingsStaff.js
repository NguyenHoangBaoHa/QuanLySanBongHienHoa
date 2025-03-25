import { useEffect, useState } from "react";
import { Table, Container, Badge, Form } from 'react-bootstrap';
import moment from 'moment';
import { BookingAPI } from "../../API";

const ManageBookingsStaff = () => {
  const [bookings, setBookings] = useState([]);

  useEffect(() => {
    fetchBookings();
  }, []);

  const fetchBookings = async () => {
    try {
      const data = await BookingAPI.GetAllBookings();
      setBookings(data);
    } catch (error) {
      console.error('Lỗi khi tải danh sách booking: ', error);
    }
  };

  const handleConfirmReceived = async (id) => {
    try {
      await BookingAPI.ConfirmReceived(id, { isReceived: true }); // Gửi đúng request body
      setBookings(
        bookings.map((booking) =>
          booking.id === id ? { ...booking, isReceived: true } : booking
        )
      );
    } catch (error) {
      console.error("Lỗi khi xác nhận nhận sân:", error);
    }
  };
  
  return (
    <Container className="mt-4">
      <h2 className="mb-4">Quản Lý Đặt Sân (Staff)</h2>
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>STT</th>
            <th>Họ Tên</th>
            <th>Số Điện Thoại</th>
            <th>Chi Tiết Sân</th>
            <th>Ngày Đặt Sân</th>
            <th>Trạng Thái Thanh Toán</th>
            <th>Nhận Sân</th>
          </tr>
        </thead>
        <tbody>
          {bookings.map((booking, index) => (
            <tr key={booking.id}>
              <td>{index + 1}</td>
              <td>{booking.displayName}</td>
              <td>{booking.phoneNumber}</td>
              <td>
                {booking.pitchName} - {booking.pitchTypeName}
              </td>
              <td>{moment(booking.bookingDate).format("DD/MM/YYYY HH:mm")}</td>
              <td>
                <Badge bg={booking.paymentStatus === "Paid" ? "success" : "danger"}>
                  {booking.paymentStatus === "Paid" ? "Đã Thanh Toán" : "Chưa Thanh Toán"}
                </Badge>
              </td>
              <td className="text-center">
                <Form.Check
                  type="checkbox"
                  checked={booking.isReceived}
                  onChange={() => handleConfirmReceived(booking.id)}
                  disabled={booking.isReceived}
                />
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </Container>
  );
}

export default ManageBookingsStaff;