import React, { useEffect, useState } from 'react';
import { Table, Container, Badge } from 'react-bootstrap';
import { BookingAPI } from '../../API';

const ManageBookingsAdmin = () => {
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

  return (
    <Container className="mt-4">
      <h2 className="mb-4">Quản Lý Đặt Sân</h2>
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>#</th>
            <th>Họ Tên</th>
            <th>Số Điện Thoại</th>
            <th>Chi Tiết Sân</th>
            <th>Ngày Đặt Sân</th>
            <th>Trạng Thái Thanh Toán</th>
          </tr>
        </thead>
        <tbody>
          {bookings.map((booking, index) => (
            <tr key={booking.id}>
              <td>{index + 1}</td>
              <td>{booking.customerName}</td>
              <td>{booking.customerPhone}</td>
              <td>
                {booking.pitchName} - {booking.pitchTypeName}
              </td>
              <td>{new Date(booking.bookingDate).toLocaleString()}</td>
              <td>
                <Badge bg={booking.paymentStatus === "Paid" ? "success" : "danger"}>
                  {booking.paymentStatus === "Paid" ? "Đã Thanh Toán" : "Chưa Thanh Toán"}
                </Badge>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </Container>
  );
};

export default ManageBookingsAdmin;
