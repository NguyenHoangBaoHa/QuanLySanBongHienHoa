import React, { useEffect, useState } from "react";
//import { BillAPI } from "../../API";
import { Button, Table } from "react-bootstrap";

const BillListAdmin = () => {
  const [bills, setBills] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    // const fetchBills = async () => {
    //   try {
    //     const params = { page: 1, pageSize: 10 }; // Giới hạn số lượng hóa đơn mỗi trang
    //     const response = await BillAPI.GetAllBills(params);
    //     setBills(response.items); // Giả sử response trả về có trường items
    //   } catch (err) {
    //     setError("Lỗi khi lấy danh sách hóa đơn.");
    //   }
    // };
    // fetchBills();
  }, []);

  const printBill = async (billId) => {
    // try {
    //   const htmlContent = await BillAPI.GetBillHtml(billId);
    //   const printWindow = window.open("", "", "height=500,width=800");
    //   printWindow.document.write(htmlContent);
    //   printWindow.document.close();
    //   printWindow.print();
    // } catch (err) {
    //   console.error("Lỗi khi xuất bill dạng giấy:", err);
    // }
  };

  return (
    <div>
      <h2>Danh Sách Hóa Đơn</h2>
      {error && <p>{error}</p>}
      <Table striped bordered hover responsive>
        <thead>
          <tr>
            <th>STT</th>
            <th>Tên Khách Hàng</th>
            <th>Tên Sân</th>
            <th>Ngày Đặt Sân</th>
            <th>Phương Thức Thanh Toán</th>
            <th>Trạng Thái Thanh Toán</th>
            <th>Tổng Tiền</th>
            <th>Hành Động</th>
          </tr>
        </thead>
        <tbody>
          {bills.map((bill, index) => (
            <tr key={bill.id}>
              <td>{index + 1}</td>
              <td>{bill.displayName}</td>
              <td>{bill.pitchName}</td>
              <td>{bill.bookingDateFormatted}</td> {/* Định dạng ngày từ DTO */}
              <td>{bill.paymentMethod}</td>
              <td>{bill.paymentStatus}</td>
              <td>{bill.totalPriceFormatted}</td> {/* Hiển thị tổng tiền đã tính toán */}
              <td>
                <Button variant="info" onClick={() => printBill(bill.id)}>
                  In Hóa Đơn
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default BillListAdmin;