import React, { useContext } from 'react';
import { Navbar, Nav, NavDropdown, Container, Button } from 'react-bootstrap';
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
            {/* Mục chung cho tất cả người dùng */}
            <Nav.Link as={Link} to="/">Trang Chủ</Nav.Link>

            {/* Hiển thị mục theo từng role */}
            {auth.isLoggedIn && auth.role === "Admin" && (
              <NavDropdown title="Quản Trị" id="admin-dropdown">
                <NavDropdown.Item as={Link} to="/manage-pitches-admin">
                  Quản lý sân
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/manage-pitch-types-admin">
                  Quản lý loại sân
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/manage-bookings-admin">
                  Quản lý đặt sân
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/revenue-report-admin">
                  Báo cáo doanh thu
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/create-staff">
                  Đăng ký thông tin nhân viên
                </NavDropdown.Item>
              </NavDropdown>
            )}

            {/* Các mục của các role khác */}
            {auth.isLoggedIn && auth.role === "Staff" && (
              <NavDropdown title="Nhân Viên" id="staff-dropdown">
                <NavDropdown.Item as={Link} to="/manage-pitches-staff">
                  Quản lý sân
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/manage-bookings-staff">
                  Quản lý đặt sân
                </NavDropdown.Item>
              </NavDropdown>
            )}

            {/* {auth.isLoggedIn && auth.role === "Customer" && (
              <NavDropdown title="Khách Hàng" id="customer-dropdown">
                <NavDropdown.Item as={Link} to="/my-bookings">
                  Lịch sử đặt sân
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/profile">
                  Hồ sơ
                </NavDropdown.Item>
              </NavDropdown>
            )} */}
          </Nav>

          {/* Hiển thị nút Đăng Nhập/Đăng Xuất */}
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
