namespace QuanLySanBong.Entities.Enums
{
    public enum PaymentMethodEnum
    {
        TienMat,       // 0 - Thanh toán bằng tiền mặt
        ChuyenKhoan,   // 1 - Chuyển khoản ngân hàng
        ViDienTu,      // 2 - Thanh toán qua ví điện tử (Momo, ZaloPay, VNPay, v.v.)
        TheTinDung,    // 3 - Thanh toán bằng thẻ tín dụng/debit
        QRCode         // 4 - Quét mã QR để thanh toán
    }
}
