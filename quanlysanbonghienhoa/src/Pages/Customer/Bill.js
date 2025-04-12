import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Form, Button, Card, Alert, Spinner } from "react-bootstrap";
import { BillAPI } from "../../API";

const Bill = () => {
  const { bookingId } = useParams();
  const navigate = useNavigate();

  const [bill, setBill] = useState(null);
  const [selectedMethod, setSelectedMethod] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  useEffect(() => {
    const fetchBill = async () => {
      try {
        const data = await BillAPI.GetBillByBookingId(bookingId);
        console.log("📜 Dữ liệu hóa đơn:", data);
        setBill(data);
        setSelectedMethod(data.paymentMethod || "");
      } catch (err) {
        setError("Không thể tải hóa đơn. Vui lòng thử lại sau.");
      } finally {
        setLoading(false);
      }
    };

    fetchBill();
  }, [bookingId]);

  const handleSubmit = async () => {
    setError("");
    setSuccessMsg("");

    if (!selectedMethod) {
      setError("Vui lòng chọn phương thức thanh toán.");
      return;
    }

    try {
      const updateData = {
        id: bill.id,
        paymentMethod: selectedMethod,
      };

      await BillAPI.UpdateBillInfo(updateData);
      setSuccessMsg("Chọn phương thức thanh toán thành công. Vui lòng chờ nhân viên xác nhận.");
    } catch (err) {
      setError("Có lỗi xảy ra khi gửi yêu cầu thanh toán.");
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center mt-5">
        <Spinner animation="border" role="status" />
        <span className="ms-2">Đang tải hóa đơn...</span>
      </div>
    );
  }

  if (!bill) {
    return (
      <Alert variant="danger" className="mt-4">
        Không tìm thấy hóa đơn.
      </Alert>
    );
  }

  return (
    <Card className="p-4 shadow rounded-3 mt-4">
      <h3 className="mb-4 text-primary">Thanh toán hóa đơn</h3>

      {error && <Alert variant="danger">{error}</Alert>}
      {successMsg && <Alert variant="success">{successMsg}</Alert>}

      <p><strong>Khách hàng:</strong> {bill.displayName}</p>
      <p><strong>Tên sân:</strong> {bill.pitchName}</p>
      <p><strong>Loại sân:</strong> {bill.pitchTypeName}</p>
      <p><strong>Ngày đặt:</strong> {bill.bookingDateFormatted}</p>
      <p><strong>Thời lượng:</strong> {bill.duration} giờ</p>
      <p><strong>Tổng tiền:</strong> {bill.totalPriceFormatted}</p>
      <p><strong>Trạng thái:</strong> {bill.paymentStatus}</p>

      <Form.Group className="mt-4">
        <Form.Label>Phương thức thanh toán</Form.Label>
        <Form.Select
          value={selectedMethod}
          onChange={(e) => setSelectedMethod(e.target.value)}
        >
          <option value="">-- Chọn phương thức --</option>
          <option value="Cash">Tiền mặt</option>
          <option value="Momo">Momo</option>
          <option value="BankTransfer">Chuyển khoản</option>
        </Form.Select>
      </Form.Group>

      <Button
        className="mt-4 w-100"
        variant="success"
        onClick={handleSubmit}
        disabled={bill.paymentStatus === "Paid"}
      >
        {bill.paymentStatus === "Paid" ? "Đã thanh toán" : "Xác nhận thanh toán"}
      </Button>
    </Card>
  );
};

export default Bill;
