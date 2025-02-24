import React, { useState } from 'react';
import { Form, Button, Alert, Container, Row, Col, Card, Modal } from 'react-bootstrap';
import { AccountAPI } from '../API';
import { useNavigate } from 'react-router-dom';

const Register = () => {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        displayName: '',
        dateOfBirth: '',
        cccd: '',
        gender: '',
        phoneNumber: '',
        address: '',
    });

    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false); // Sử dụng để kiểm tra trạng thái đăng ký
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess(false);

        const { email, password, displayName, dateOfBirth, cccd, gender, phoneNumber, address } = formData;

        const registerCustomerDto = {
            account: { email, password },
            customer: { displayName, dateOfBirth, cccd, gender, phoneNumber, address },
        };

        try {
            await AccountAPI.registerCustomer(registerCustomerDto); // Gọi API đăng ký
            setSuccess(true); // Đặt trạng thái thành công
            setFormData({
                email: '',
                password: '',
                displayName: '',
                dateOfBirth: '',
                cccd: '',
                gender: '',
                phoneNumber: '',
                address: '',
            });
        } catch (err) {
            setError(err.error || 'Đăng ký thất bại. Vui lòng thử lại.');
        }
    };

    const handleRedirectToLogin = () => {
        navigate('/login'); // Chuyển hướng đến trang đăng nhập
    };

    return (
        <Container className="register-container">
            <Row className="justify-content-center">
                <Col md={8} lg={6}>
                    <Card className="shadow-lg register-card">
                        <Card.Body>
                            <h2 className="text-center mb-4">Đăng ký tài khoản</h2>
                            {error && <Alert variant="danger">{error}</Alert>}
                            <Form onSubmit={handleSubmit}>
                                <Form.Group className="mb-3" controlId="email">
                                    <Form.Label>Email</Form.Label>
                                    <Form.Control
                                        type="email"
                                        placeholder="Nhập email"
                                        name="email"
                                        value={formData.email}
                                        onChange={handleChange}
                                        required
                                    />
                                </Form.Group>

                                <Form.Group className="mb-3" controlId="password">
                                    <Form.Label>Mật khẩu</Form.Label>
                                    <Form.Control
                                        type="password"
                                        placeholder="Nhập mật khẩu"
                                        name="password"
                                        value={formData.password}
                                        onChange={handleChange}
                                        required
                                    />
                                </Form.Group>

                                <Form.Group className="mb-3" controlId="displayName">
                                    <Form.Label>Tên hiển thị</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="Nhập tên hiển thị"
                                        name="displayName"
                                        value={formData.displayName}
                                        onChange={handleChange}
                                        required
                                    />
                                </Form.Group>

                                <Form.Group className="mb-3" controlId="dateOfBirth">
                                    <Form.Label>Ngày sinh</Form.Label>
                                    <Form.Control
                                        type="date"
                                        name="dateOfBirth"
                                        value={formData.dateOfBirth}
                                        onChange={handleChange}
                                        required
                                    />
                                </Form.Group>

                                <Form.Group className="mb-3" controlId="cccd">
                                    <Form.Label>CCCD</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="Nhập CCCD"
                                        name="cccd"
                                        value={formData.cccd}
                                        onChange={handleChange}
                                        required
                                    />
                                </Form.Group>

                                <Form.Group className="mb-3" controlId="gender">
                                    <Form.Label>Giới tính</Form.Label>
                                    <Form.Select
                                        name="gender"
                                        value={formData.gender}
                                        onChange={handleChange}
                                        required
                                    >
                                        <option value="">Chọn giới tính</option>
                                        <option value="Male">Nam</option>
                                        <option value="Female">Nữ</option>
                                    </Form.Select>
                                </Form.Group>

                                <Form.Group className="mb-3" controlId="phoneNumber">
                                    <Form.Label>Số điện thoại</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="Nhập số điện thoại"
                                        name="phoneNumber"
                                        value={formData.phoneNumber}
                                        onChange={handleChange}
                                        required
                                    />
                                </Form.Group>

                                <Form.Group className="mb-3" controlId="address">
                                    <Form.Label>Địa chỉ</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="Nhập địa chỉ"
                                        name="address"
                                        value={formData.address}
                                        onChange={handleChange}
                                    />
                                </Form.Group>

                                <Button variant="primary" type="submit" className="w-100">
                                    Đăng ký
                                </Button>
                            </Form>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            {/* Modal thông báo đăng ký thành công */}
            <Modal show={success} onHide={handleRedirectToLogin} centered>
                <Modal.Header closeButton>
                    <Modal.Title>Đăng ký thành công</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>Bạn đã đăng ký tài khoản thành công! Nhấn "OK" để quay lại trang đăng nhập.</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="primary" onClick={handleRedirectToLogin}>
                        OK
                    </Button>
                </Modal.Footer>
            </Modal>
        </Container>
    );
};

export default Register;
