import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Container, Row, Col, Carousel, Button, Alert, Spinner } from "react-bootstrap";
import moment from "moment";
import { PitchAPI, BookingAPI } from "../../API";

const CustomerBookingDetail = () => {
  const { pitchId, date, time } = useParams();
  const [pitch, setPitch] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    console.log("📌 URL Params:", { pitchId, date, time });

    const storedCustomerId = localStorage.getItem("customerId");
    console.log("🔍 customerId từ localStorage:", storedCustomerId);

    if (!storedCustomerId) {
      console.error("❌ Không tìm thấy customerId trong localStorage!");
    }

    const fetchData = async () => {
      try {
        const pitchData = await PitchAPI.GetPitchDetail(pitchId);
        console.log("✅ Dữ liệu sân:", pitchData);
        setPitch(pitchData);
      } catch (error) {
        setError("Không thể tải thông tin, vui lòng thử lại!");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [pitchId, date, time]);

  // Xử lý khi nhấn "Xác Nhận Đặt Sân"
  const handleConfirmBooking = async () => {
    const IdCustomer = localStorage.getItem("customerId"); // Lấy IdCustomer từ localStorage
    const token = localStorage.getItem("token"); // Lấy token nếu có

    if (!IdCustomer) {
      alert("Bạn chưa đăng nhập!");
      navigate("/login");
      return;
    }

    const bookingData = {
      IdCustomer: Number(IdCustomer), // Chuyển sang kiểu số
      idPitch: Number(pitchId), // Đảm bảo rằng tên trường khớp với API
      bookingDate: `${date}T${time}:00`, // Format kiểu DateTime
      duration: 60,
      paymentStatus: 0, // Đảm bảo rằng status đúng với backend
    };

    console.log("📌 Dữ liệu gửi API:", bookingData); // ⚠️ Kiểm tra dữ liệu gửi API
    console.log("📌 Token gửi lên:", token);

    try {
      const response = await BookingAPI.CreateBooking(bookingData, token);
      const bookingId = response.id || response.bookingId; // Lấy bookingId từ phản hồi
      if(!bookingId){
        alert("Không lấy được Booking Id!");
        return;
      }

      const path = `/customer/bill/booking/${bookingId}`;
      console.log("✅ Đặt sân thành công, chuyển hướng đến:", path);
      navigate(path);
    } catch (error) {
      console.error("❌ Lỗi đặt sân:", error.response?.data || error.message);
      alert(`Lỗi khi đặt sân: ${error.response?.data || "Vui lòng thử lại!"}`);
    }
  };

  if (error) return <Alert variant="danger">{error}</Alert>;
  if (loading) return <Spinner animation="border" variant="primary" className="d-block mx-auto" />;

  return (
    <Container fluid>
      <Row>
        {/* Thông Tin Sân Đặt (2/3 giao diện) */}
        <Col md={8} className="p-4" style={{ overflowY: "auto", maxHeight: "90vh" }}>
          <h2 className="mb-3"><strong>{pitch?.name} - {pitch?.pitchTypeName}</strong></h2>

          {/* Carousel Hình Ảnh Sân */}
          <Carousel>
            {pitch?.listImagePath?.length > 0 ? (
              pitch.listImagePath.map((imgPath, index) => (
                <Carousel.Item key={index}>
                  <img
                    className="d-block w-100"
                    src={imgPath}
                    alt={`Hình ${index + 1}`}
                    style={{ maxHeight: "400px", objectFit: "cover" }}
                  />
                </Carousel.Item>
              ))
            ) : (
              <Carousel.Item>
                <img
                  className="d-block w-100"
                  src="/assets/default-pitch.jpg"
                  alt="Hình mặc định"
                  style={{ maxHeight: "400px", objectFit: "cover" }}
                />
              </Carousel.Item>
            )}
          </Carousel>
        </Col>

        {/* Thông Tin Thanh Toán (1/3 giao diện) */}
        <Col md={4} className="p-4 bg-light" style={{ height: "90vh", overflow: "hidden" }}>
          <h3><strong>Thông Tin Thanh Toán</strong></h3>
          <p><strong>Ngày Đặt:</strong> {moment(date).format("DD/MM/YYYY")}</p>
          <p><strong>Giờ Đặt:</strong> {time}</p>
          <p><strong>Giá Tiền:</strong> {pitch?.price ? pitch.price.toLocaleString() : "Chưa cập nhật"} VND</p>

          <Button variant="primary" className="w-100 mt-3" onClick={handleConfirmBooking}>
            Xác Nhận Đặt Sân
          </Button>
        </Col>
      </Row>
    </Container>
  );
};

export default CustomerBookingDetail;
