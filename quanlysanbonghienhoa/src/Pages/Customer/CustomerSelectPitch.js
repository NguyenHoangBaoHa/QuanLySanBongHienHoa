import React, { useEffect, useState } from "react";
import { Container, Card, Row, Col, Button, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { PitchAPI } from "../../API";

const CustomerSelectPitch = () => {
  const [pitches, setPitches] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchPitches();
  }, []);

  const fetchPitches = async () => {
    try {
      const data = await PitchAPI.GetAllPitches();
      console.log("Danh sách sân: ", data); // Thêm log để kiểm tra dữ liệu trả về từ API
      setPitches(data);
      setLoading(false);
    } catch (error) {
      console.error("Lỗi khi tải danh sách sân: ", error);
      setLoading(false);
    }
  };

  const handleSelectPitch = (pitchId, pitchType) => {
    navigate(`/customer/booking/schedule/${pitchId}/${pitchType}`);
  };

  if (loading) {
    return (
      <Container className="text-center mt-5">
        <Spinner animation="border" variant="primary" />
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
                    onClick={() => handleSelectPitch(pitch.id, pitch.pitchTypeName)}
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