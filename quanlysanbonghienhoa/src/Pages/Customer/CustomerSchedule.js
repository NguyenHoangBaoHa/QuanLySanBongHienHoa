import React, { useCallback, useEffect, useState } from "react";
import { Container, Table, Button, Form, Spinner, Alert } from "react-bootstrap";
import axios from "axios";
import moment from "moment";
import { PitchAPI } from "../../API";

const CustomerSchedule = () => {
  const [pitches, setPitches] = useState([]);
  const [selectedPitch, setSelectedPitch] = useState("");
  const [startDate, setStartDate] = useState(moment().startOf("isoWeek").format("YYYY-MM-DD"));
  const [schedule, setSchedule] = useState([]);
  const [timeSlots] = useState(["08:00", "09:00", "10:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00"]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // 📌 Lấy danh sách sân bóng
  useEffect(() => {
    const fetchPitches = async () => {
      try {
        const data = await PitchAPI.GetAllPitches();
        setPitches(data);
        if (data.length > 0) {
          setSelectedPitch(data[0].id);
        }
      } catch (error) {
        setError("Không thể tải danh sách sân. Vui lòng thử lại!");
      }
    };

    fetchPitches();
  }, []);

  // 📌 Lấy lịch đặt sân theo tuần (Dùng useCallback để tránh re-render không cần thiết)
  const fetchSchedule = useCallback(async () => {
    if (!selectedPitch) return;
    setLoading(true);
    setError(null);
    try {
      const response = await axios.get(`/api/booking/pitch/${selectedPitch}/week?startDate=${startDate}`);
      setSchedule(response.data);
    } catch (error) {
      setError("Không thể tải lịch đặt sân. Vui lòng thử lại!");
    } finally {
      setLoading(false);
    }
  }, [selectedPitch, startDate]);

  // 📌 Gọi API mỗi khi sân hoặc ngày thay đổi
  useEffect(() => {
    fetchSchedule();
  }, [fetchSchedule]);

  // 📌 Kiểm tra khung giờ đã đặt chưa
  const isBooked = (date, time) => {
    return schedule.some((b) => 
      moment(b.date).format("YYYY-MM-DD") === date && 
      moment(b.time, "HH:mm").format("HH:mm") === time
    );
  };

  return (
    <Container>
      <h2 className="my-4">Lịch đặt sân</h2>

      {error && <Alert variant="danger">{error}</Alert>}

      <Form className="d-flex gap-3 mb-4">
        <Form.Select value={selectedPitch} onChange={(e) => setSelectedPitch(e.target.value)}>
          {pitches.map((pitch) => (
            <option key={pitch.id} value={pitch.id}>{pitch.name}</option>
          ))}
        </Form.Select>
        <Form.Control
          type="date"
          value={startDate}
          onChange={(e) => setStartDate(e.target.value)}
        />
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
                <th key={i}>{moment(startDate).add(i, "days").format("DD/MM")}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {timeSlots.map((time) => (
              <tr key={time}>
                <td>{time}</td>
                {[...Array(7)].map((_, i) => {
                  const date = moment(startDate).add(i, "days").format("YYYY-MM-DD");
                  return (
                    <td key={date} className={isBooked(date, time) ? "bg-danger text-white" : "bg-success text-white"}>
                      {isBooked(date, time) ? "Đã đặt" : (
                        <Button variant="primary" size="sm">Đặt sân</Button>
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
