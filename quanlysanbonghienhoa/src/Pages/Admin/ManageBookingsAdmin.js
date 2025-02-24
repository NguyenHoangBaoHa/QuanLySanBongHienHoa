import React, { useEffect, useState } from 'react';
import { Table, Spinner, Badge, Form } from 'react-bootstrap';
import { BookingAPI } from '../../API';

const ManageBookingsAdmin = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [role, setRole] = useState('');
  const [search, setSearch] = useState('');

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        const token = localStorage.getItem('token');
        const decodedToken = JSON.parse(atob(token.split('.')[1]));
        const userRole = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        setRole(userRole);

        const data = await BookingAPI.GetAllBookings();
        setBookings(data);
      } catch (error) {
        console.error('Lỗi khi tải dữ liệu:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchBookings();
  }, []);

  // const handleConfirmReceived = async (id) => {
  //   try {
  //     await BookingAPI.ConfirmReceived(id);
  //     setBookings((prev) =>
  //       prev.map((booking) =>
  //         booking.id === id ? { ...booking, isReceived: true } : booking
  //       )
  //     );
  //   } catch (error) {
  //     console.error('Lỗi khi xác nhận nhận sân:', error);
  //   }
  // };

  const filteredBookings = bookings.filter(
    (booking) =>
      booking.customerName.toLowerCase().includes(search.toLowerCase()) ||
      booking.phoneNumber.includes(search)
  );

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center mt-5">
        <Spinner animation="border" />
      </div>
    );
  }

  return (
    <div className="p-3">
      <h3 className="mb-4">Quản Lý Đặt Sân</h3>

      <Form className="mb-3">
        <Form.Control
          type="text"
          placeholder="Tìm kiếm theo tên hoặc SĐT..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </Form>

      <Table striped bordered hover responsive className="text-center">
        <thead className="table-dark">
          <tr>
            <th>#</th>
            <th>Họ Tên</th>
            <th>SĐT</th>
            <th>Chi Tiết Sân</th>
            <th>Ngày Đặt Sân</th>
            <th>Trạng Thái Thanh Toán</th>
            {/* {role === 'Staff' && <th>Nhận Sân</th>} */}
          </tr>
        </thead>
        <tbody>
          {filteredBookings.length > 0 ? (
            filteredBookings.map((booking, index) => (
              <tr key={booking.id}>
                <td>{index + 1}</td>
                <td>{booking.customerName}</td>
                <td>{booking.phoneNumber}</td>
                <td>{`${booking.pitchName} - ${booking.pitchTypeName}`}</td>
                <td>{new Date(booking.bookingDate).toLocaleString()}</td>
                <td>
                  <Badge bg={booking.paymentStatus ? 'success' : 'warning'}>
                    {booking.paymentStatus ? 'Đã Thanh Toán' : 'Chưa Thanh Toán'}
                  </Badge>
                </td>
                {/* {role === 'Staff' && (
                  <td>
                    <Form.Check
                      type="checkbox"
                      checked={booking.isReceived}
                      onChange={() => handleConfirmReceived(booking.id)}
                      disabled={booking.isReceived}
                      label={booking.isReceived ? 'Đã Nhận' : ''}
                    />
                  </td>
                )} */}
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan={role === 'Staff' ? 7 : 6}>
                Không có dữ liệu phù hợp.
              </td>
            </tr>
          )}
        </tbody>
      </Table>
    </div>
  );
};

export default ManageBookingsAdmin;
