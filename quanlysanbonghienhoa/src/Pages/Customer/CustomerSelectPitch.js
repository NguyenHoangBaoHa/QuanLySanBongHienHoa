import React, { useEffect, useState } from "react";
import { Container, Card, Row, Col, Button, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { PitchAPI } from "../../API";

const CustomerSelectPitch = () => {
  const [pitches, setPitches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchPitches = async () => {
      try {
        const data = await PitchAPI.GetAllPitches();
        console.log("📌 Dữ liệu sân bóng từ API: ", data);
        setPitches(data);
      } catch (error) {
        console.error("Lỗi khi tải danh sách sân: ", error);
        setError("Không thể tải danh sách sân. Vui lòng thử lại!");
      } finally {
        setLoading(false);
      }
    };
    fetchPitches();
  }, []);

  const handleSelectPitch = (pitch) => {
    if (!pitch.idPitchType) {
      console.error("Lỗi: Không tìm thấy pitchTypeId", pitch);
      alert("Lỗi: Không tìm thấy loại sân của sân bóng này!");
      return;
    }
    navigate(`/customer/booking/schedule/${pitch.id}/${pitch.pitchTypeName}`);
  };

  if (loading) {
    return (
      <Container className="text-center mt-5">
        <Spinner animation="border" variant="primary" />
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="text-center mt-5">
        <p className="text-danger">{error}</p>
      </Container>
    );
  }


  return (
    <Container className="mt-4">
      <h2 className="mb-4">Chọn Sân Bóng</h2>
      <Row>
        {pitches.map((pitch) => {
          console.log(`Pitch ID: ${pitch.id}, ImagePath: ${pitch.imagePath}`); // Thêm log để kiểm tra giá trị của imagePath
          return (
            <Col key={pitch.id} md={4} className="mb-4">
              <Card>
                {pitch.imagePath ? (
                  <Card.Img variant="top" src={pitch.imagePath} alt={pitch.name} />
                ) : (
                  <Card.Img variant="top" src="/default-image.jpg" alt="Hình ảnh mặc định" />
                )}
                <Card.Body>
                  <Card.Title>{pitch.name}</Card.Title>
                  <Card.Text>
                    <strong>Loại sân:</strong> {pitch.pitchTypeName || "Không xác định"} <br />
                    <strong>Sức chứa:</strong> {pitch.limitPerson ? pitch.limitPerson : "N/A"} người <br />
                    <strong>Giá:</strong> {pitch.price ? pitch.price.toLocaleString() : "N/A"} VND
                  </Card.Text>
                  <Button
                    variant="primary"
                    onClick={() => handleSelectPitch(pitch)} // ✅ Sửa pitchTypeName → pitchTypeId
                  >
                    Chọn Sân
                  </Button>
                </Card.Body>
              </Card>
            </Col>
          );
        })}
      </Row>
    </Container>
  );
};

export default CustomerSelectPitch;