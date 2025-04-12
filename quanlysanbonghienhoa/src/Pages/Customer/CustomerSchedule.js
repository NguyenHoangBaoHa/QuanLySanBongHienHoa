import React, { useEffect, useState, useCallback } from "react";
import { Container, Table, Form, Spinner, Alert, Button, Row, Col } from "react-bootstrap";
import moment from "moment";
import { useParams, useNavigate } from "react-router-dom";
import { PitchAPI, BookingAPI } from "../../API";

const getToday = () => moment().format("YYYY-MM-DD");

const CustomerSchedule = () => {
  const { pitchId, idPitchType } = useParams();
  const navigate = useNavigate();

  const [pitches, setPitches] = useState([]);
  const [selectedPitch, setSelectedPitch] = useState(pitchId || "");
  const [selectedPitchType, setSelectedPitchType] = useState(idPitchType || "");
  const [weekStart, setWeekStart] = useState(getToday());
  const [schedule, setSchedule] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const timeSlots = [
    "08:00", "09:00", "10:00",
    "14:00", "15:00", "16:00",
    "17:00", "18:00", "19:00",
    "20:00", "21:00"
  ];

  useEffect(() => {
    if (!pitchId || !idPitchType) {
      console.error("⛔ Thiếu thông tin pitchId hoặc idPitchType");
      navigate("/customer/booking");
    }
  }, [pitchId, idPitchType, navigate]);

  const fetchPitches = useCallback(async () => {
    try {
      setLoading(true);
      const response = await PitchAPI.GetAllPitches();
      if (!Array.isArray(response)) throw new Error("Dữ liệu sân không hợp lệ!");

      setPitches(response);
      const currentPitch = response.find(p => p.id === pitchId);
      if (currentPitch) {
        setSelectedPitch(pitchId);
        setSelectedPitchType(currentPitch.idPitchType);
      } else if (response.length > 0) {
        setSelectedPitch(response[0].id);
        setSelectedPitchType(response[0].idPitchType);
      }
    } catch (err) {
      setError("Không thể tải danh sách sân bóng.");
    } finally {
      setLoading(false);
    }
  }, [pitchId]);

  const fetchSchedule = useCallback(async () => {
    if (!selectedPitch) return;
    try {
      setLoading(true);
      const formattedDate = moment(weekStart).format("YYYY-MM-DD");
      const data = await BookingAPI.GetScheduleByWeek(selectedPitch, formattedDate);
      setSchedule(Array.isArray(data) ? data : []);
    } catch (err) {
      setError("Không thể tải lịch đặt sân.");
    } finally {
      setLoading(false);
    }
  }, [selectedPitch, weekStart]);

  useEffect(() => {
    fetchPitches();
  }, [fetchPitches]);

  useEffect(() => {
    fetchSchedule();
  }, [fetchSchedule]);

  const getSlotInfo = (date, time) => {
    return schedule.find(item => {
      const bookingTime = moment(item.bookingDate).format("YYYY-MM-DD HH:mm");
      console.log("Thời gian: " + bookingTime);
      return bookingTime === `${date} ${time}`;
    });
  };


  const isPastTime = (date, time) => {
    const now = moment();
    const selectedDateTime = moment(`${date} ${time}`, "YYYY-MM-DD HH:mm");
    return selectedDateTime.isBefore(now);
  };

  const handleTimeSlotClick = (date, time, isBookedSlot, isPast, bookingId) => {
    if (isPast) {
      alert("Bạn không thể đặt sân trong quá khứ!");
      return;
    }

    if (isBookedSlot) {
      alert(`Khung giờ này đã được đặt (Mã booking: ${bookingId}). Vui lòng chọn khung giờ khác!`);
      return;
    }

    navigate(`/customer/booking/detail/${selectedPitch}/${selectedPitchType}/${date}/${time}`);
  };

  const handlePreviousWeek = () => {
    const previousWeek = moment(weekStart).subtract(7, 'days').format("YYYY-MM-DD");
    setWeekStart(previousWeek);
  };

  const handleNextWeek = () => {
    const nextWeek = moment(weekStart).add(7, 'days').format("YYYY-MM-DD");
    setWeekStart(nextWeek);
  };

  return (
    <Container>
      <h2 className="my-4 text-center">Lịch đặt sân</h2>

      {error && <Alert variant="danger">{error}</Alert>}

      <Row className="align-items-center mb-3">
        <Col md={4}>
          <Form.Select
            value={selectedPitch}
            onChange={(e) => {
              const pitchId = e.target.value;
              setSelectedPitch(pitchId);
              const selected = pitches.find(p => p.id === pitchId);
              if (selected) setSelectedPitchType(selected.idPitchType);
            }}
          >
            {pitches.map(pitch => (
              <option key={pitch.id} value={pitch.id}>
                {pitch.name} - {pitch.pitchTypeName}
              </option>
            ))}
          </Form.Select>
        </Col>

        <Col md={4} className="text-center">
          <Button variant="outline-primary" className="me-2" onClick={handlePreviousWeek}>
            ← Tuần trước
          </Button>
          <Button variant="outline-primary" onClick={handleNextWeek}>
            Tuần sau →
          </Button>
        </Col>

        <Col md={4}>
          <Form.Control
            type="date"
            value={weekStart}
            onChange={(e) => setWeekStart(e.target.value)}
          />
        </Col>
      </Row>

      {loading ? (
        <div className="text-center">
          <Spinner animation="border" variant="primary" />
        </div>
      ) : (
        <Table bordered hover responsive className="text-center align-middle">
          <thead>
            <tr>
              <th>Khung giờ</th>
              {[...Array(7)].map((_, i) => {
                const day = moment(weekStart).add(i, 'days');
                return <th key={i}>{day.format("ddd DD/MM")}</th>;
              })}
            </tr>
          </thead>
          <tbody>
            {timeSlots.map((time) => (
              <tr key={time}>
                <td>{time}</td>
                {[...Array(7)].map((_, i) => {
                  const date = moment(weekStart).add(i, 'days').format("YYYY-MM-DD");
                  const slot = getSlotInfo(date, time);
                  const isBooked = slot?.timeslotStatus === 1;
                  const past = isPastTime(date, time);

                  let bgColor = "";
                  let label = "";

                  if (past) {
                    bgColor = "bg-secondary text-white";
                    label = "Quá khứ";
                  } else if (isBooked) {
                    bgColor = "bg-danger text-white";
                    label = `Đã đặt (${slot.id})`;
                  } else {
                    bgColor = "bg-success text-white";
                    label = "Trống";
                  }

                  return (
                    <td
                      key={`${date}-${time}`}
                      className={bgColor}
                      style={{ cursor: past || isBooked ? "not-allowed" : "pointer" }}
                      onClick={() => handleTimeSlotClick(date, time, isBooked, past, slot?.id)}
                    >
                      {label}
                    </td>
                  );
                })}
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </Container>
  );
};

export default CustomerSchedule;
