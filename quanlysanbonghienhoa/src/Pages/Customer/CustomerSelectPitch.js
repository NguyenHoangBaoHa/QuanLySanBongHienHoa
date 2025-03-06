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
      setPitches(data);
      setLoading(false);
      console.log("Danh sách sân: ", data);
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
        {pitches.map((pitch) => (
          <Col key={pitch.id} md={4} className="mb-4">
            <Card>
              {pitch.image && (
                <Card.Img variant="top" src={pitch.image} alt={pitch.name} />
              )}
              <Card.Body>
                <Card.Title>{pitch.name}</Card.Title>
                <Card.Text>
                  <strong>Loại sân:</strong> {pitch.pitchTypeName || "N/A"} <br />
                  <strong>Sức chứa:</strong> {pitch.limitPerson || "N/A"} người <br />
                  <strong>Giá:</strong> {pitch?.price ? pitch.price.toLocaleString() : "N/A"} VND
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
        ))}
      </Row>
    </Container>
  );
};

export default CustomerSelectPitch;
