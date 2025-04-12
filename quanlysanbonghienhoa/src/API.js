import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:8007/api",
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Hàm xử lý lỗi chung
const handleApiError = (error) => {
  if (error.response) {
    return error.response.data || "Server Error";
  } else if (error.request) {
    return "No response from server.";
  } else {
    return error.message || "Unknown error occurred.";
  }
};

const AccountAPI = {
  login: async (email, password) => {
    try {
      const response = await api.post("/Account/login", { email, password });
      const { token, role, username, idCustomer } = response.data;

      localStorage.setItem("token", token);
      localStorage.setItem("role", role);
      localStorage.setItem("username", username);

      if (role === "Customer") {
        localStorage.setItem("customerId", idCustomer?.toString() || "");
      }

      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  createStaff: async (staffData) => {
    try {
      const response = await api.post("/Account/create-staff", staffData);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  registerCustomer: async (customerData) => {
    try {
      const response = await api.post("/Account/register-customer", customerData);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  }
};

// ==================== Pitch Type API ====================
const PitchTypeAPI = {
  GetAll: async () => {
    try {
      const response = await api.get("/PitchType");
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CreatePitchType: async (formData) => {
    try {
      const response = await api.post("/PitchType", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  UpdatePitchType: async (id, formData) => {
    try {
      const response = await api.put(`/PitchType/${id}`, formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  DeletePitchType: async (id) => {
    try {
      const response = await api.delete(`/PitchType/${id}`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },
};

// ==================== Pitch API ====================
const PitchAPI = {
  GetAllPitches: async () => {
    try {
      const response = await api.get("/Pitch");
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  GetPitchDetail: async (pitchId) => {
    try {
      const response = await api.get(`/Pitch/${pitchId}`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CreatePitch: async (data) => {
    try {
      const response = await api.post("/Pitch", data, {
        headers: { "Content-Type": "application/json" },
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  UpdatePitch: async (id, data) => {
    try {
      const response = await api.put(`/Pitch/${id}`, data, {
        headers: { "Content-Type": "application/json" },
      });
      return response.data;
    }
    catch (error) {
      console.error("Lỗi khi cập nhật sân:", error);
      throw error;
    }
  },

  DeletePitch: async (id) => {
    try {
      await api.delete(`/Pitch/${id}`);
      return { success: true };
    } catch (error) {
      throw handleApiError(error);
    }
  },
};

// ==================== Booking API ====================
const BookingAPI = {
  GetAllBookings: async (params) => {
    try {
      const response = await api.get("/Booking", { params });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CreateBooking: async (bookingData) => {
    try {
      const IdCustomer = localStorage.getItem("customerId");
      const token = localStorage.getItem("token");

      if (!token) {
        throw new Error("❌ Không tìm thấy token trong localStorage.");
      }
      if (!IdCustomer) {
        throw new Error("❌ Không tìm thấy IdCustomer trong localStorage.");
      }

      const response = await api.post(`/Booking/CreateBooking`, {
        ...bookingData,
        IdCustomer: Number(IdCustomer),
      }, {
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        },
      });

      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CancelBooking: async (id) => {
    try {
      const response = await api.patch(`/Booking/${id}/cancel`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },
  GetScheduleByWeek: async (pitchId, startDate) => {
    try {
      const response = await api.get(`/Booking/pitch/${pitchId}/week`, {
        params: { startDate }
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },
  ConfirmReceived: async (id) => {
    const response = await api.patch(`/Booking/${id}/confirm-received`, {
      isReceived: true,
    });
    return response.data;
  }
};

const BillAPI = {
  // ==== Admin / Staff ====

  GetAllBills: async () => {
    try {
      const response = await api.get("/Bill");
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  GetBillDetail: async (id) => {
    try {
      const response = await api.get(`/Bill/${id}`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  UpdatePaymentStatus: async (id, status) => {
    try {
      const response = await api.put(`/Bill`, {
        id: id,
        paymentStatus: status,
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  ExportBillToPDF: async (id) => {
    try {
      const response = await api.get(`/Bill/${id}/pdf`, {
        responseType: "blob",
      });

      const blob = new Blob([response.data], { type: "application/pdf" });
      const url = window.URL.createObjectURL(blob);
      window.open(url);

      return { success: true };
    } catch (error) {
      throw handleApiError(error);
    }
  },

  // ==== Customer ====

  GetBillByBookingId: async (bookingId) => {
    try {
      const response = await api.get(`/Bill/booking/${bookingId}`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },
  CreateBill: async (billData) => {
    try {
      const response = await api.post("/Bill/createF", billData, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  GetTransactionHistory: async () => {
    try {
      const response = await api.get(`/Bill/customer/transactions`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },
};

export { AccountAPI, PitchTypeAPI, PitchAPI, BookingAPI, BillAPI };
