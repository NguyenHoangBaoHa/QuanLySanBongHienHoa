import React, { useContext } from 'react';
import { Navbar, Nav, Container, Button } from 'react-bootstrap';
import { AuthContext } from '../Context/AuthContext';
import { Link, useNavigate } from 'react-router-dom';

const NavigationBar = () => {
  const { auth, logout } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={Link} to="/">
          Sân Bóng Hiền Hòa
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            {/* Admin */}
            {auth.isLoggedIn && auth.role === "Admin" && (
              <>
                <Nav.Link as={Link} to="/manage-pitches-admin">Quản lý sân</Nav.Link>
                <Nav.Link as={Link} to="/manage-pitch-types-admin">Quản lý loại sân</Nav.Link>
                <Nav.Link as={Link} to="/manage-bookings-admin">Quản lý đặt sân</Nav.Link>
                {/* <Nav.Link as={Link} to="/list-bill-admin">Danh sách hóa đơn</Nav.Link>
                <Nav.Link as={Link} to="/revenue-report-admin">Báo cáo doanh thu</Nav.Link> */}
                <Nav.Link as={Link} to="/create-staff">Đăng ký nhân viên</Nav.Link>
              </>
            )}

            {/* Staff */}
            {auth.isLoggedIn && auth.role === "Staff" && (
              <>
                <Nav.Link as={Link} to="/manage-pitches-staff">Quản lý sân</Nav.Link>
                <Nav.Link as={Link} to="/manage-bookings-staff">Quản lý đặt sân</Nav.Link>
                <Nav.Link as={Link} to="/manage-pitch-receiving-staff">Quản lý nhận sân</Nav.Link>
              </>
            )}

            {/* Customer */}
            {auth.isLoggedIn && auth.role === "Customer" && (
              <>
                <Nav.Link as={Link} to="/customer/booking">Đặt sân</Nav.Link>
                {/* <Nav.Link as={Link} to="/customer/booking-history">Lịch sử đặt sân</Nav.Link> */}
              </>
            )}
          </Nav>

          {/* Đăng nhập / Đăng xuất */}
          <Nav>
            {auth.isLoggedIn ? (
              <Button variant="outline-light" onClick={handleLogout}>
                Đăng Xuất
              </Button>
            ) : (
              <Nav.Link as={Link} to="/login">
                Đăng Nhập
              </Nav.Link>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default NavigationBar;
