import React, { useEffect, useState, useCallback } from "react";
import { Container, Table, Button, Form, Spinner, Alert } from "react-bootstrap";
import moment from "moment";
import { useParams, useNavigate } from "react-router-dom";
import { PitchAPI, BookingAPI } from "../../API";

// Lấy ngày hiện tại
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

  // ✅ Khung giờ cố định
  const timeSlots = ["08:00", "09:00", "10:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00"];

  useEffect(() => {
    if (!pitchId || !idPitchType) {
      console.error("⛔ Lỗi: idPitchType bị undefined!", { pitchId, idPitchType });
      navigate("/customer/booking");
    }
  }, [pitchId, idPitchType, navigate]);

  // 📌 Lấy danh sách sân bóng
  const fetchPitches = useCallback(async () => {
    try {
      setLoading(true);
      const response = await PitchAPI.GetAllPitches();
      console.log("✅ Danh sách sân:", response);

      if (!Array.isArray(response)) {
        throw new Error("Dữ liệu sân không hợp lệ!");
      }

      const fixedPitches = response.map(pitch => ({
        ...pitch,
        idPitchType: pitch.idPitchType || "",
      }));

      setPitches(fixedPitches);

      if (pitchId) {
        const selected = fixedPitches.find(p => p.id === pitchId);
        if (selected) {
          setSelectedPitch(selected.id);
          setSelectedPitchType(selected.idPitchType);
        }
      } else if (fixedPitches.length > 0) {
        setSelectedPitch(fixedPitches[0].id);
        setSelectedPitchType(fixedPitches[0].idPitchType);
      }
    } catch (error) {
      console.error("❌ Lỗi khi tải danh sách sân:", error);
      setError("Không thể tải danh sách sân bóng.");
    } finally {
      setLoading(false);
    }
  }, [pitchId]);

  // 📌 Lấy lịch đặt sân theo tuần
  const fetchSchedule = useCallback(async () => {
    if (!selectedPitch) return;
    try {
      setLoading(true);
      setError(null);

      const startDate = moment(weekStart).format("YYYY-MM-DD");

      console.log(`📌 Gọi API lịch sân: pitchId = ${selectedPitch}, startDate = ${startDate}`);

      const data = await BookingAPI.GetScheduleByWeek(selectedPitch, startDate);
      console.log("✅ API Trả về:", data);

      setSchedule(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error("❌ Lỗi khi lấy lịch sân:", error);
      setError("Không thể tải lịch đặt sân. Vui lòng thử lại!");
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

  // 📌 Kiểm tra khung giờ đã đặt chưa
  const isBooked = (date, time) => {
    return schedule.some(b =>
      moment(b.date).format("YYYY-MM-DD") === date && moment(b.time, "HH:mm").format("HH:mm") === time
    );
  };

  // 📌 Xử lý khi nhấn nút "Đặt Sân"
  const handleBookingClick = (date, time) => {
    if (!selectedPitch || !selectedPitchType) {
      alert("Vui lòng chọn sân trước khi đặt!");
      return;
    }

    navigate(`/customer/booking/detail/${selectedPitch}/${selectedPitchType}/${date}/${time}`);
  };

  return (
    <Container>
      <h2 className="my-4">Lịch đặt sân</h2>

      {error && <Alert variant="danger">{error}</Alert>}

      <Form className="d-flex gap-3 mb-4">
        <Form.Select
          value={selectedPitch}
          onChange={(e) => {
            setSelectedPitch(e.target.value);
            const selected = pitches.find(p => p.id === e.target.value);
            if (selected) setSelectedPitchType(selected.idPitchType);
          }}
        >
          {pitches.map(pitch => (
            <option key={pitch.id} value={pitch.id}>
              {pitch.name} - {pitch.pitchTypeName}
            </option>
          ))}
        </Form.Select>

        <Form.Control type="date" value={weekStart} onChange={(e) => setWeekStart(e.target.value)} />
      </Form>

      {loading ? (
        <div className="text-center">
          <Spinner animation="border" variant="primary" />
        </div>
      ) : (
        <Table bordered hover>
          <thead>
            <tr>
              <th>Khung Giờ</th>
              {[...Array(7)].map((_, i) => (
                <th key={i}>{moment(weekStart).add(i, "days").format("DD/MM")}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {timeSlots.map(time => (
              <tr key={time}>
                <td>{time}</td>
                {[...Array(7)].map((_, i) => {
                  const date = moment(weekStart).add(i, "days").format("YYYY-MM-DD");
                  return (
                    <td key={date} className={isBooked(date, time) ? "bg-danger text-white" : "bg-success text-white"}>
                      {isBooked(date, time) ? "Đã đặt" : (
                        <Button variant="primary" size="sm" onClick={() => handleBookingClick(date, time)}>
                          Đặt Sân
                        </Button>
                      )}
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
