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
  }, [pitchId]);

  // Xử lý khi nhấn "Xác Nhận Đặt Sân"
  const handleConfirmBooking = async () => {
    // Kiểm tra thông tin khách hàng
    const IdCustomer = localStorage.getItem("customerId");
    if (!IdCustomer) {
      alert("Bạn chưa đăng nhập!");
      navigate("/login");
      return;
    }

    // Kiểm tra thông tin sân và thời gian
    if (!pitchId || !date || !time) {
      alert("Thông tin sân hoặc thời gian không hợp lệ.");
      return;
    }

    // Chuẩn bị dữ liệu đặt sân
    const bookingData = {
      IdCustomer: Number(IdCustomer),
      idPitch: Number(pitchId),
      bookingDate: `${date}T${time}:00`, // Đảm bảo định dạng DateTime đúng
      duration: 60,
      paymentStatus: 0,
    };

    console.log("📌 Dữ liệu gửi API:", bookingData); // Kiểm tra dữ liệu trước khi gửi API

    try {
      // Gửi yêu cầu tạo booking
      const response = await BookingAPI.CreateBooking(bookingData);

      if (!response || !response.data) {
        throw new Error("Không nhận được phản hồi hợp lệ từ server");
      }

      console.log("✅ Booking Success:", response);
      alert("Đặt sân thành công!");
      navigate("/customer/bill");

    } catch (error) {
      // Xử lý lỗi từ API
      console.error("❌ Lỗi đặt sân:", error.response ? error.response.data : error.message);

      if (error.response) {
        // Lỗi từ server
        alert(`Lỗi từ server: ${error.response.data}`);
      } else if (error.request) {
        // Không nhận được phản hồi từ server
        alert("Không nhận được phản hồi từ server, vui lòng kiểm tra kết nối mạng.");
      } else {
        // Lỗi khác
        alert(`Lỗi: ${error.message}`);
      }
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
