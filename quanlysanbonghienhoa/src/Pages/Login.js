import { useContext, useState } from "react";
import { Alert, Button, Card, Form } from "react-bootstrap";
import { AccountAPI } from "../API";
import { AuthContext } from "../Context/AuthContext";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { login } = useContext(AuthContext);

  const handleLogin = async (e) => {
    e.preventDefault();

    // Kiểm tra email hợp lệ hoặc là tài khoản admin
    const isValidEmail = email.includes("@") || email === "admin";
    if (!isValidEmail) {
      setError("Email phải có định dạng hợp lệ hoặc là tài khoản admin.");
      return;
    }

    try {
      // Gọi API để kiểm tra tài khoản và mật khẩu
      const response = await AccountAPI.login(email, password);

      // Lưu thông tin vào localStorage
      localStorage.setItem('username', response.username);
      localStorage.setItem('token', response.token);
      localStorage.setItem('role', response.role);
      login(response.role);

      // Điều hướng theo role
      switch (response.role) {
        case 'Admin':
          navigate('/manage-pitches-admin');
          break;
        case 'Staff':
          navigate('/manage-pitches-staff');
          break;
        case 'Customer':
          navigate('/my-bookings');
          break;
        default:
          navigate('/');
      }
    } catch (err) {
      console.error(err);
      setError('Đăng nhập thất bại: Vui lòng kiểm tra email hoặc mật khẩu.');
    }
  };

  const redirectToRegister = () => {
    navigate('/register');
  }

  return (
    <Card className="mx-auto" style={{ maxWidth: '400px', marginTop: '50px' }}>
      <Card.Body>
        <h2 className="text-center mb-4">Đăng Nhập</h2>
        {error && <Alert variant="danger">{error}</Alert>}
        <Form onSubmit={handleLogin}>
          <Form.Group className="mb-3" controlId="formEmail">
            <Form.Label>Email:</Form.Label>
            <Form.Control
              type="text"
              placeholder="Nhập email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </Form.Group>
          <Form.Group className="mb-3" controlId="formPassword">
            <Form.Label>Mật Khẩu:</Form.Label>
            <Form.Control
              type="password"
              placeholder="Nhập mật khẩu"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </Form.Group>
          <Button variant="primary" type="submit" className="w-100">
            Đăng Nhập
          </Button>
        </Form>
        <div className="text-center mt-3">
          <span>Bạn chưa có tài khoản? </span>
          <Button
            variant="link"
            className="p-0"
            onClick={redirectToRegister}
            style={{ textDecoration: "none", fontWeight: "bold" }}
          >
            Đăng ký ngay
          </Button>
        </div>
      </Card.Body>
    </Card>
  );
}

export default Login;